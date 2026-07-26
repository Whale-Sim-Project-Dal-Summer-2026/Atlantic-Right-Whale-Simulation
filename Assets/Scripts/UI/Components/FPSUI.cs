/**
 * FPSUI.cs: Script which handles
 * the display of the FPS.
 *
 * @author Mars Semenova 
 */

using TMPro;
using UnityEngine;

public class FPSUI : MonoBehaviour {
    // params
    private TextMeshProUGUI fpsText;
    
    void Start() {
        InvokeRepeating(nameof(ShowFPS), 0.01f, 0.5f);
    }

    /**
     * Update and display FPS.
     */
    private void ShowFPS() {
        if (fpsText) {
            fpsText.text = (Mathf.RoundToInt(Time.frameCount / Time.time)).ToString(); // TODO: not sure how correct this is
        }
    }
}
