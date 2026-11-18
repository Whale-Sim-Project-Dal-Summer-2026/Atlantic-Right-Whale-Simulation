using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.SceneManagement;

public enum StateMode {
    PLAYBACK,
    FREESWIM,
    MENU
}

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    [SerializeField] List<string> scenes;

    bool openMenu;
    string currScene;
    private static StateMode mode = StateMode.MENU;

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
        currScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    bool changeScene(string scene)
    {
        if (currScene == scene) return false;
        
        currScene = scene;
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
        return true;
    }

    public void changeToNoGear() {
        if (changeScene(scenes[0])) {
            mode = StateMode.PLAYBACK;
        }
    }
    public void changeToNoEntangle() {
        if (changeScene(scenes[1])) {
            mode = StateMode.PLAYBACK;
        }
    }

    public void changeToEntangle() {
        if (changeScene(scenes[2])) {
            mode = StateMode.PLAYBACK;
        }
    }

    public void changeToFreeSwim() {
        if (changeScene(scenes[3])) {
            mode = StateMode.FREESWIM;
        }
    }

    public void changeToScenarios() {
        if (changeScene(scenes[4])) {
            mode = StateMode.MENU;
        }
    }

    public static StateMode GetStateMode() {
        return mode;
    } 
}