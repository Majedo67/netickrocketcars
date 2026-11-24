using Netick;
using Netick.Samples;
using Netick.Unity;
using UnityEngine;

/// <summary>
/// This is the Game Mode script, the parent script of all game modes.
/// </summary>
[ExecutionOrder(-30)]
public abstract class GameMode : NetworkBehaviour
{
  // Networked State ********************
  [Networked] public NetworkBool             DisableInputForAll      { get; set; }
  [Networked] public NetworkBool             DisableCarCamera        { get; set; }
  [Networked] public NetworkBool             DisableCarSimulation    { get; set; }
  [HideInInspector] public GlobalData        GlobalData              { get; private set; }
  [HideInInspector] public bool              Paused                  = false;
  public GameObject                          UIParent;
  public NetworkPlayerId                     SpectatedPlayer         = NetworkPlayerId.Invalid;

  private bool _webGamepadJumpWasDown;
  private bool _konamiNotified;

  public override void NetworkAwake()
  {
    GlobalData                               = Sandbox.GetComponent<GlobalData>();
    GlobalData.Camera                        = Sandbox.FindObjectOfType<Camera>();
    GlobalData.GameMode                      = this;
  }

  public override void NetworkStart()
  {
    if (GlobalData.IsReplay)
      GlobalData.Chat.PushNotificationLocal("Press ` (backquote) to toggle UI during replays.");
    else if (Sandbox.IsPlayer)
      GlobalData.Chat.PushNotificationLocal("Press T for team-only chat, and Y for global chat.");
  }

  private void Update()
  {
    if (GlobalData != null && GlobalData.Sandbox.IsRunning && GlobalData.IsReplay && Input.GetKeyDown(KeyCode.BackQuote))
    {
      Cursor.lockState = GlobalData.HideUI ? CursorLockMode.None : CursorLockMode.Locked;
      GlobalData.HideUI = !GlobalData.HideUI;
      UpdateUIState();
    }

    if (GlobalData != null && NetickWebGLInput.KonamiCodeEntered && !_konamiNotified)
    {
      _konamiNotified = true;
      GlobalData.Chat.PushNotificationLocal("Konami code activated!");
    }
  }

  private void OnGUI()
  {
    if (!NetickWebGLInput.IsMobileBrowser || NetickWebGLInput.HasGamepad)
      return;

    var style = new GUIStyle(GUI.skin.box);
    style.fontSize = 22;
    style.alignment = TextAnchor.MiddleCenter;
    GUI.Box(new Rect(Screen.width * 0.1f, Screen.height * 0.4f, Screen.width * 0.8f, 100f),
      "Connect a controller to play", style);
  }

  public override void NetworkUpdate()
  {
    if (GlobalData.IsReplay)
      ReactToSpectateControls();

    if (Paused || !GlobalData.CanUseInput)
      return;

    // iPhone/iPad browsers require a connected controller. Desktop browsers
    // can continue using the original keyboard controls.
    if (!NetickWebGLInput.CanPlay)
    {
      Sandbox.SetInput<GameInput>(default);
      _webGamepadJumpWasDown = false;
      return;
    }

    var input = Sandbox.GetInput<GameInput>();
    var hasGamepad = NetickWebGLInput.HasGamepad;
    var gamepadMovement = hasGamepad
      ? new Vector3(NetickWebGLInput.Steer, NetickWebGLInput.Throttle, NetickWebGLInput.AirRoll)
      : Vector3.zero;
    var keyboardMovement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), Input.GetAxis("Roll"));
    var movementInput = input.Movement + keyboardMovement + gamepadMovement;

    input.Movement = new Vector3(
      Mathf.Clamp(movementInput.x, -1f, 1f),
      Mathf.Clamp(movementInput.y, -1f, 1f),
      Mathf.Clamp(movementInput.z, -1f, 1f));

    var gamepadJumpDown = hasGamepad && NetickWebGLInput.Jump && !_webGamepadJumpWasDown;
    _webGamepadJumpWasDown = hasGamepad && NetickWebGLInput.Jump;

    input.Rocket |= Input.GetButton("Rocket") || (hasGamepad && NetickWebGLInput.Rocket);
    input.Jump   |= Input.GetButtonDown("Jump") || gamepadJumpDown;
    input.Drift  |= Input.GetButton("Drift") || (hasGamepad && NetickWebGLInput.Drift);

    Sandbox.SetInput(input);
  }

  public virtual void ReactToSpectateControls()
  {
    if (!Sandbox.ContainsPlayer(SpectatedPlayer))
      SpectatedPlayer = NetworkPlayerId.Invalid;

    var players = Sandbox.Players;

    if (Input.GetKeyDown(KeyCode.F))
    {
      if (SpectatedPlayer.IsValid)
        SpectatedPlayer = NetworkPlayerId.Invalid;
      else if (players.Count > 0)
      {
        if (Sandbox.TryGetPlayerObject<Player>(players[0], out var player0) && player0.IsReady)
          SpectatedPlayer = players[0];
        if (players.Count > 1)
        {
          if (Sandbox.TryGetPlayerObject<Player>(players[1], out var player1) && player1.IsReady)
            SpectatedPlayer = players[1];
        }
      }
    }
    else if (Input.inputString.Length > 0 && char.IsDigit(Input.inputString[0]))
    {
      int numValue = (int.Parse(Input.inputString[0].ToString())) - 1;
      if (numValue < players.Count && numValue >= 0)
        SpectatedPlayer = players[numValue];
    }
  }

  public void UpdateUIState()
  {
    UIParent.SetActive(!GlobalData.HideUI);
    GetComponent<NetworkInfo>().enabled    = !GlobalData.HideUI;
    GetComponent<GameStarter>().enabled    = !GlobalData.HideUI;
    GetComponent<ReplayTimeline>().enabled = !GlobalData.HideUI;
  }
}
