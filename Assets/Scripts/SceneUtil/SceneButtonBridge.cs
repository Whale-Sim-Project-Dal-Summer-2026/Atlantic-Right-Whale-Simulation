using UnityEngine;

public class SceneButtonBridge : MonoBehaviour
{
    public void ClickNoGear() {
        if (SceneSwitcher.Instance != null) {
            SceneSwitcher.Instance.changeToNoGear();
        }
    }

    public void ClickNoEntangle() {
        if (SceneSwitcher.Instance != null) {
            SceneSwitcher.Instance.changeToNoEntangle();
        }
    }

    public void ClickEntangle() {
        if (SceneSwitcher.Instance != null) {
            SceneSwitcher.Instance.changeToEntangle();
        }
    }

    public void ClickFreeSwim() {
        if (SceneSwitcher.Instance != null) {
            SceneSwitcher.Instance.changeToFreeSwim();
        }
    }

    public void ClickScenarios() { // TODO
        if (SceneSwitcher.Instance != null) {
            SceneSwitcher.Instance.changeToScenarios();
        }
    }
}