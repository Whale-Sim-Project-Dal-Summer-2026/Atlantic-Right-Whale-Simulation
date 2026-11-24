using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class ButtonPressUtil {
    private static readonly Dictionary<InputAction, double> pressTimes = new Dictionary<InputAction, double>();

    private static float pressBufferMS = 200f;

    public static bool Pressed(InputAction action) {
        if (action == null) {
            return false;
        }

        if (!action.IsPressed()) {
            return false;
        }

        double currentTime = Time.unscaledTimeAsDouble * 1000f;

        if (pressTimes.TryGetValue(action, out double lastTimePressed)) {
            if (currentTime - lastTimePressed <= pressBufferMS) {
                return false;
            }
        }

        pressTimes[action] = currentTime;
        return true;
    }

// MAKE SURE TO CALL THESE!! This will unregister a button to free up memory
    public static void UnRegisterButton(InputAction action){
        if(pressTimes.ContainsKey(action)) pressTimes.Remove(action);
    }

// clears dictionary
    public static void UnRegisterAll(){
        pressTimes.Clear();
    }
}