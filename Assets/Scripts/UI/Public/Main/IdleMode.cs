using UnityEngine;
using UnityEngine.InputSystem;

public class IdleMode : MonoBehaviour {
    // refs
    public GameObject IdleUI;
    
    // vars
    public float idleTime;
    private float lastInput;
    
    void Awake() {
        lastInput = Time.time;
    }
        
    private void Update() {
        if (Keyboard.current.anyKey.wasPressedThisFrame) {
            lastInput = Time.time;
        }
        IdleUI.SetActive(IsIdle());
    }
        
    public bool IsIdle(){
        return Time.time - lastInput > idleTime;
    }
}