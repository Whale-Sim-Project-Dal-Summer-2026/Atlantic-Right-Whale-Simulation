/**
 * LineRollingGraph.cs: Script which renders
 * a rolling line graph.
 *
 * @author Mars Semenova
*/

using TMPro;
using UnityEngine;

public class LineRollingGraph : Graph {
    private TextMeshProUGUI valText;  
    
    void Awake() {
        // get ref
        graphUI = Resources.Load<Sprite>("UI/Graphs/micrographbg");
    }

    void Start() {
        // create bg
        CreateGraphUI();
        
        // create val txt
        float val = -100f;
        GameObject valTextObj = new GameObject("ValText");
        valTextObj.transform.SetParent(gameObject.transform);
        RectTransform valTextTransform = valTextObj.AddComponent<RectTransform>();
        valTextTransform.sizeDelta = new Vector2(20, 20); // TODO: un-magic number
        valTextTransform.transform.localPosition = new Vector3(width/2 + 12, 0, 0); // TODO: make responsive
        valTextTransform.localScale = Vector3.one;
        valText = valTextObj.AddComponent<TextMeshProUGUI>();
        valText.verticalAlignment = VerticalAlignmentOptions.Middle;
        valText.horizontalAlignment = HorizontalAlignmentOptions.Left;
        valText.fontSize = 8;
        if (font != null) {
            valText.font = font;
        }
        valText.text = val.ToString();
        
    }
}