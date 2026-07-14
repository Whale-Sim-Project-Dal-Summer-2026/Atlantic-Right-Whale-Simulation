using System.Collections.Generic;
using Obi;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{


    [SerializeField] List<string> scenes;

    string currScene;


    void Start()
    {
        currScene = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.isPressed)
        {
            Application.Quit();
        }

        if (Keyboard.current.digit1Key.isPressed)
        {

            changeScene(scenes[0]);
        }
        if (Keyboard.current.digit2Key.isPressed)
        {

            changeScene(scenes[1]);
        }
        if (Keyboard.current.digit3Key.isPressed)
        {

            changeScene(scenes[2]);
        }  
           
    }


    void changeScene(string scene)
    {
        if(currScene == scene) return;

        Debug.LogFormat("curr: {0}\nScene {1}",currScene, scene);

        currScene = scene;
        SceneManager.LoadScene(scene);

    }
}
