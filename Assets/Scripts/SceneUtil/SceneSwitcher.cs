using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public static SceneSwitcher Instance { get; private set; }

    [SerializeField] List<string> scenes;

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
        currScene = SceneManager.GetActiveScene().name;
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