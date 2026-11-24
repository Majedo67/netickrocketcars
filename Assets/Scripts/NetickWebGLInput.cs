using System.Runtime.InteropServices;
using UnityEngine;

/// Browser Gamepad API bridge for a Unity WebGL build.
public sealed class NetickWebGLInput : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern int NRC_GetGamepadCount();
    [DllImport("__Internal")] private static extern float NRC_GetGamepadAxis(int axis);
    [DllImport("__Internal")] private static extern int NRC_GetGamepadButton(int button);
    [DllImport("__Internal")] private static extern int NRC_IsMobileBrowser();
    [DllImport("__Internal")] private static extern int NRC_GetKonamiCode();
#endif

    public static bool HasGamepad {
        get {
#if UNITY_WEBGL && !UNITY_EDITOR
            return NRC_GetGamepadCount() > 0;
#else
            return Input.GetJoystickNames().Length > 0;
#endif
        }
    }

    public static bool IsMobileBrowser {
        get {
#if UNITY_WEBGL && !UNITY_EDITOR
            return NRC_IsMobileBrowser() != 0;
#else
            return false;
#endif
        }
    }

    public static bool CanPlay => !IsMobileBrowser || HasGamepad;
    public static float Steer => Axis(0);
    public static float Throttle => -Axis(1);
    public static float AirRoll => Axis(2);
    public static bool Jump => Button(0);
    public static bool Drift => Button(1);
    public static bool Rocket => Button(7) || Button(5);

    public static bool KonamiCodeEntered {
        get {
#if UNITY_WEBGL && !UNITY_EDITOR
            return NRC_GetKonamiCode() != 0;
#else
            return false;
#endif
        }
    }

    private static float Axis(int axis) {
#if UNITY_WEBGL && !UNITY_EDITOR
        return NRC_GetGamepadAxis(axis);
#else
        return 0f;
#endif
    }

    private static bool Button(int button) {
#if UNITY_WEBGL && !UNITY_EDITOR
        return NRC_GetGamepadButton(button) != 0;
#else
        return false;
#endif
    }
}
