/**
 * ForcesList.cs: Script which implements
 * the list of forces functionality.
 *
 * @author Mars Semenova 
 */

using TMPro;
using UnityEngine;

public class ForcesList : MonoBehaviour {
    // label
    private TextMeshProUGUI forcesText;

    void Awake() {
        // get ref
        forcesText = GetComponent<TextMeshProUGUI>();
    }

    void Update() { 
        forcesText.text = ""; // reset
        
        // force 1
        forcesText.text += "Forces 1: " + "xxx\n"; // TODO
        
        // force 2
        forcesText.text += "Forces 2: " + "xxx\n"; // TODO
    }
}
