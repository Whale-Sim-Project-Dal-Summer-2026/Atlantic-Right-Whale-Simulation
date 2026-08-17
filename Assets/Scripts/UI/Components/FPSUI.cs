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
    [SerializeField] private TextMeshProUGUI fpsText;
    [Header("Options")]
    [SerializeField] private float repeatRate = 0.1f;
    private double lastChangeTime;
    private float deltaTime;
    
    void Start() {
        lastChangeTime = Time.unscaledTimeAsDouble;
    }

    private void Update() {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        
        // update line
        double currentTime = Time.unscaledTimeAsDouble;
        if (currentTime - lastChangeTime >= repeatRate) {
            lastChangeTime = currentTime;
            ShowFPS();
        }
        
    }
    
    /**
     * Update and display FPS.
     */
    private void ShowFPS() {
        if (fpsText) {
            fpsText.text = string.Format(" {0:0.}", 1.0f / deltaTime); // TODO: not sure how correct this is 
        }
    }
}
