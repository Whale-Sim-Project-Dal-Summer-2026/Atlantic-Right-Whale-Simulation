/**
 * TelemetryUI.cs: Script which implements
 * the telemetry UI functionality.
 *
 * @author Mars Semenova 
 */

using System;
using TMPro;
using UnityEngine;

public class TelemetryUI : MonoBehaviour {
    // params
    // labels
    [Header("Text Fields")]
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI headingText;
    [SerializeField] private TextMeshProUGUI rollText;
    [SerializeField] private TextMeshProUGUI pitchText;
    // texture params
    [Header("Textures")]
    [SerializeField] private GameObject rollTexture;
    [SerializeField] private GameObject pitchTexture;
    
    // vars
    private RectTransform pitchTextureRectTransform;
    private int counter;
    private String speed = "0 m/s";
    private int heading;
    private int roll;
    private int pitch;
    
    void Awake() {
        pitchTextureRectTransform = pitchTexture.GetComponent<RectTransform>();
    }

    void Update() {
        // set speed
        counter++;
        if (counter >= 50){
            speedText.text = speed; 
            counter = 0; 
        }
        
        // set heading
        headingText.text = heading + "\u00b0"; 
        headingText.transform.rotation = Quaternion.Euler(headingText.transform.eulerAngles.x, headingText.transform.eulerAngles.y, -1*heading); 
        
        // set roll
        rollText.text = roll + "\u00b0";
        rollTexture.transform.rotation = Quaternion.Euler(headingText.transform.eulerAngles.x, headingText.transform.eulerAngles.y, roll);

        // set pitch
        Vector3 pitchCenter = pitchTexture.transform.position;
        float maxOffsetLen = pitchTextureRectTransform.rect.height*pitchTextureRectTransform.lossyScale.y / 2;
        pitchText.text = pitch + "\u00b0";
        float offsetLen = (maxOffsetLen-20) * (Math.Abs(pitch) / 90.0f);
        float xOffset = pitchTextureRectTransform.rect.width*pitchTextureRectTransform.lossyScale.x*(-0.3f);
        Vector3 offset = new Vector3(xOffset, -maxOffsetLen + (pitch < 0 ? -offsetLen : offsetLen), 0);
        pitchText.transform.position = pitchCenter + offset;
    }

    /**
     * Update speed.
     * @param s - String with new speed.
     */
    public void UpdateSpeed(String s) {
        speed = s;
    }
    
    /**
     * Update heading.
     * @param h - New heading.
     */
    public void UpdateHeading(int h) {
       heading = h;
    }
    
    /**
     * Update roll.
     * @param r - New roll.
     */
    public void UpdateRoll(int r) {
        roll = r;
    }
    
    /**
     * Update pitch.
     * @param p - New pitch.
     */
    public void UpdatePitch(int p) {
        pitch = p;
    }
}
