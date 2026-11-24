# WebGL controller integration

The browser controller bridge is now connected to the game's existing input collection in `GameMode.NetworkUpdate()`.

## Controls
- Left stick X: steer
- Left stick Y: accelerate / reverse
- Right stick X: air roll
- A / Cross: jump (press)
- B / Circle: drift
- Right trigger or right bumper: rocket boost
- Keyboard: the game's original Unity input mappings remain enabled on desktop browsers.
- Konami code: Up, Up, Down, Down, Left, Right, Left, Right, B, A. The game displays a notification when detected.

## Mobile browser rule
On iPhone/iPad browsers, gameplay input is held until a controller is connected. A visible “Connect a controller to play” message appears while no controller is detected. Desktop browsers do not require a controller.

## Files
- `Assets/Plugins/WebGL/NetickRocketCars.jslib`: browser Gamepad API and Konami sequence detection.
- `Assets/Scripts/NetickWebGLInput.cs`: C# accessors for controller state.
- `Assets/Rocket Cars/Scripts/Game/GameMode.cs`: merges gamepad input with the existing keyboard input and sends it through Netick's existing network input pipeline.

This is source-code integration only. A Unity WebGL build still needs to be produced in Unity Editor before the game can run as an HTML browser game. Unity's official JavaScript plug-in guide: https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/webgl/develop/interactingwithbrowserscripting/web-interacting-browser-js
