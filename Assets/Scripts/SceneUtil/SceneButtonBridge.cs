using UnityEngine;

public class SceneButtonBridge : MonoBehaviour
{
    public void ClickNoGear() {
        if (SceneManager.Instance != null) {
            SceneManager.Instance.changeToNoGear();
        }
    }

    public void ClickNoEntangle() {
        if (SceneManager.Instance != null) {
            SceneManager.Instance.changeToNoEntangle();
        }
    }

    public void ClickEntangle() {
        if (SceneManager.Instance != null) {
            SceneManager.Instance.changeToEntangle();
        }
    }

    public void ClickFreeSwim() {
        if (SceneManager.Instance != null) {
            SceneManager.Instance.changeToFreeSwim();
        }
    }
}