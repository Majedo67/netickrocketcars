mergeInto(LibraryManager.library, {
  NRC_GetGamepadCount: function () {
    if (!navigator.getGamepads) return 0;
    var pads = navigator.getGamepads(), count = 0;
    for (var i = 0; i < pads.length; i++) if (pads[i] && pads[i].connected) count++;
    return count;
  },
  NRC_GetGamepadAxis: function (axis) {
    if (!navigator.getGamepads) return 0;
    var pads = navigator.getGamepads();
    for (var i = 0; i < pads.length; i++) {
      if (pads[i] && pads[i].connected) {
        var value = pads[i].axes[axis] || 0;
        return Math.abs(value) < 0.12 ? 0 : value;
      }
    }
    return 0;
  },
  NRC_GetGamepadButton: function (button) {
    if (!navigator.getGamepads) return 0;
    var pads = navigator.getGamepads();
    for (var i = 0; i < pads.length; i++) {
      if (pads[i] && pads[i].connected) {
        var item = pads[i].buttons[button];
        if (item) return item.pressed || item.value > 0.5 ? 1 : 0;
      }
    }
    return 0;
  },
  NRC_IsMobileBrowser: function () {
    return /iPhone|iPad|iPod|Android/i.test(navigator.userAgent) ? 1 : 0;
  },
  NRC_GetKonamiCode: function () {
    return window.__nrcKonamiComplete ? 1 : 0;
  }
});
(function () {
  if (window.__nrcKonamiInstalled) return;
  window.__nrcKonamiInstalled = true;
  window.__nrcKonamiComplete = false;
  var sequence = ["ArrowUp","ArrowUp","ArrowDown","ArrowDown","ArrowLeft","ArrowRight","ArrowLeft","ArrowRight","b","a"];
  var position = 0;
  window.addEventListener("keydown", function (event) {
    var key = event.key.length === 1 ? event.key.toLowerCase() : event.key;
    if (key === sequence[position]) {
      position++;
      if (position === sequence.length) { window.__nrcKonamiComplete = true; position = 0; }
    } else {
      position = key === sequence[0] ? 1 : 0;
    }
  });
}());
