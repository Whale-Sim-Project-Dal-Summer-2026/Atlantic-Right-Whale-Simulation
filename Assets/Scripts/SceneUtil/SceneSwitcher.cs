using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public static SceneSwitcher Instance { get; private set; }

    [SerializeField] List<string> scenes;

    InputAction openMenuAction;

    bool openMenu;
    string currScene;

    void Awake()
    {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        openMenuAction = InputSystem.actions.FindAction("OpenMenu"); // TODO
        currScene = SceneManager.GetActiveScene().name;
    }

    void checkForMenuPress() {
        openMenu = (openMenuAction?.ReadValue<float>() ?? 0.0f) == 1.0f; // TODO
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.isPressed) {
            Application.Quit(); // TODO
        }
        checkForMenuPress(); // TODO

        if (openMenu) {
            changeToScenarios();
        }
    }

    void changeScene(string scene)
    {
        if (currScene == scene) return;

        Debug.LogFormat("curr: {0}\nScene {1}", currScene, scene);

        currScene = scene;
        SceneManager.LoadScene(scene);
    }

    public void changeToNoGear() {
        changeScene(scenes[0]);
    }
    public void changeToNoEntangle() {
        changeScene(scenes[1]);
    }

    public void changeToEntangle() {
        changeScene(scenes[2]);
    }

    public void changeToFreeSwim() {
        changeScene(scenes[3]);
    }

    public void changeToScenarios() {
        changeScene(scenes[4]);
    }
}