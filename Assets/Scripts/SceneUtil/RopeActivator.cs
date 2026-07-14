using UnityEngine;
using UnityEngine.InputSystem;

public class RopeActivator : MonoBehaviour
{
    [SerializeField] GameObject ropeParent;

    bool showing;


    void Start()
    {
        showing = ropeParent.activeInHierarchy;
    }

    void Update()
    {
        if (Keyboard.current.pKey.wasReleasedThisFrame)
        {
            showing = !showing;
            ropeParent.SetActive(showing);
        }
    }
}