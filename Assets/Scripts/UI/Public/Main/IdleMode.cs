using UnityEngine;
using UnityEngine.InputSystem;

public class IdleMode : MonoBehaviour {
    // refs
    public GameObject IdleUI;
    
    // vars
    public float idleTime;
    private float lastInput;
    private bool forceIdle = false;
    
    void Awake() {
        lastInput = Time.time;
    }
        
    private void Update() {
        if (Keyboard.current.anyKey.wasPressedThisFrame) {
            lastInput = Time.time;
        }

        if (Keyboard.current.f10Key.wasPressedThisFrame) {
            forceIdle = !forceIdle;
        }
        IdleUI.SetActive(IsIdle() || forceIdle);
    }
        
    public bool IsIdle(){
        return (Time.time - lastInput > idleTime) || forceIdle;
    }
}