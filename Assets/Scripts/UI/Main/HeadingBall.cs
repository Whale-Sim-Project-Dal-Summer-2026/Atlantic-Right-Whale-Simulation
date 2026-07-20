/**
 * HeadingBall.cs: Script which implements
 * the heading ball functionality.
 *
 * @author Mars Semenova 
 */

using TMPro;
using UnityEngine;

public class HeadingBall : MonoBehaviour {
    // labels
    private TextMeshProUGUI speedText;
    private TextMeshProUGUI headingText;
    private TextMeshProUGUI rollText;
    private TextMeshProUGUI pitchText;
    private GameObject rollTexture;
    private GameObject pitchTexture;
    private RectTransform pitchTextureRectTransform;
    
    void Awake() {
        // get refs
        speedText = GameObject.Find("HeadingBallSpeed").GetComponent<TextMeshProUGUI>();
        headingText = GameObject.Find("HeadingBallHeading").GetComponent<TextMeshProUGUI>();
        rollText = GameObject.Find("HeadingBallRoll").GetComponent<TextMeshProUGUI>();
        pitchText = GameObject.Find("HeadingBallPitch").GetComponent<TextMeshProUGUI>();
        rollTexture = GameObject.Find("HeadingBallTextureRoll");
        pitchTexture = GameObject.Find("HeadingBallTexturePitch");
        pitchTextureRectTransform = pitchTexture.GetComponent<RectTransform>();
    }

    void Update() {
        // update ball
        // TODO
        
        // set speed
        speedText.text = 100 + " km/h"; // TODO
        
        // set heading
        int heading = 100 % 360; // TODO
        headingText.text = heading + "\u00b0"; 
        headingText.transform.rotation = Quaternion.Euler(headingText.transform.eulerAngles.x, headingText.transform.eulerAngles.y, -1*heading); 
        
        // set roll
        int roll = 45; // TODO
        rollText.text = roll + "\u00b0";
        rollTexture.transform.rotation = Quaternion.Euler(headingText.transform.eulerAngles.x, headingText.transform.eulerAngles.y, roll);

        // set yaw
        int pitch = 90; // TODO
        Vector3 pitchCenter = pitchTexture.transform.position;
        float maxOffsetLen = pitchTextureRectTransform.rect.height / 2;
        pitchText.text = pitch + "\u00b0";
        float offsetLen = (maxOffsetLen-20) * (pitch / 90);
        Vector3 offset = new Vector3(-50, pitch < 0 ? -offsetLen-maxOffsetLen : offsetLen-maxOffsetLen, 0);
        pitchText.transform.position = pitchCenter + offset;
        
    }
}
