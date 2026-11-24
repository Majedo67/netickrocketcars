# WebGL controller bridge

This fork now includes a browser Gamepad API bridge for a future Unity WebGL build.

## Files
- Assets/Plugins/WebGL/NetickRocketCars.jslib: reads connected browser gamepads and detects the Konami keyboard sequence.
- Assets/Scripts/NetickWebGLInput.cs: C# accessors for controller state and the mobile controller gate.

## Standard controller mapping
- Left stick X: steering
- Left stick Y: throttle / reverse
- Right stick X: air roll
- A / Cross: jump
- Right trigger or B / Circle: rocket
- Konami code: Up, Up, Down, Down, Left, Right, Left, Right, B, A

## Integration still required
The existing CarController must consume NetickWebGLInput.Steer, Throttle, AirRoll, Jump, and Rocket in its local-player input collection. The scene must use NetickWebGLInput.CanPlay to show a Connect a controller gate on mobile. This bridge is separate so it does not change the original desktop controls or server-authoritative simulation.

A Unity WebGL build must be produced with Unity Editor before this can run as an HTML browser game. Unity's .jslib browser plug-in mechanism is documented at:
https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/webgl/develop/interactingwithbrowserscripting/web-interacting-browser-js
