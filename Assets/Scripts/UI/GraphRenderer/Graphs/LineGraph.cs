/**
 * LineGraph.cs: Script which renders
 * a line graph.
 *
 * @author Mars Semenova
*/

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LineGraph : Graph {
    // UI
    private Sprite dashedLine;
    private float dashedLineW = 112;
    private float dashedLineH = 1;
    private TextMeshProUGUI maxText; 
    private TextMeshProUGUI minText; 
    
    void Awake() {
        // get refs
        graphUI = Resources.Load<Sprite>("UI/Graphs/macrographbg");    
        dashedLine = Resources.Load<Sprite>("UI/Graphs/minmaxline");  
    }

    void Start() {
        // create bg
        CreateGraphUI();
        
        // create max line + txt
        float max = -100f;
        GameObject maxUIObj = new GameObject("MaxUI");
        maxUIObj.transform.SetParent(gameObject.transform);
        maxUIObj.transform.localPosition = Vector3.zero; 
        maxUIObj.transform.localScale = Vector3.one;
        
        GameObject maxTextObj = new GameObject("MaxText");
        maxTextObj.transform.SetParent(maxUIObj.transform);
        RectTransform maxTextTransform = maxTextObj.AddComponent<RectTransform>();
        maxTextTransform.sizeDelta = new Vector2(20, 20); // TODO: un-magic number
        maxTextTransform.transform.localPosition = new Vector3(width/2 + 12, height/2 - 10, 0); // TODO: make responsive
        maxTextTransform.localScale = Vector3.one;
        maxText = maxTextObj.AddComponent<TextMeshProUGUI>();
        maxText.verticalAlignment = VerticalAlignmentOptions.Middle;
        maxText.horizontalAlignment = HorizontalAlignmentOptions.Left;
        maxText.fontSize = 8;
        if (font != null) {
            maxText.font = font;
        }
        maxText.text = max.ToString();
        
        GameObject maxLineObj = new GameObject("MaxLine");
        maxLineObj.transform.SetParent(maxUIObj.transform);
        RectTransform maxLineTransform = maxLineObj.AddComponent<RectTransform>();
        maxLineTransform.sizeDelta = new Vector2(dashedLineW, dashedLineH);
        maxLineTransform.transform.localPosition = new Vector3(0, height/2 - 10, 0);
        maxLineTransform.localScale = Vector3.one;
        Image maxLineImage = maxLineObj.AddComponent<Image>();
        maxLineImage.sprite = dashedLine;
        maxLineImage.useSpriteMesh = true;
        
        // create min line + txt (TODO: make a func so less repetition)
        float min = -200f;
        GameObject minUIObj = new GameObject("MinUI");
        minUIObj.transform.SetParent(gameObject.transform);
        minUIObj.transform.localPosition = Vector3.zero; 
        minUIObj.transform.localScale = Vector3.one;
        
        GameObject minTextObj = new GameObject("MinText");
        minTextObj.transform.SetParent(minUIObj.transform);
        RectTransform minTextTransform = minTextObj.AddComponent<RectTransform>();
        minTextTransform.sizeDelta = new Vector2(20, 20); // TODO: un-magic number
        minTextTransform.transform.localPosition = new Vector3(width/2 + 12, -height/2 + 10, 0); // TODO: make responsive
        minTextTransform.localScale = Vector3.one;
        minText = minTextObj.AddComponent<TextMeshProUGUI>();
        minText.verticalAlignment = VerticalAlignmentOptions.Middle;
        minText.horizontalAlignment = HorizontalAlignmentOptions.Left;
        minText.fontSize = 8;
        if (font != null) {
            minText.font = font;
        }
        minText.text = min.ToString();
        
        GameObject minLineObj = new GameObject("MinLine"); 
        minLineObj.transform.SetParent(minUIObj.transform);
        RectTransform minLineTransform = minLineObj.AddComponent<RectTransform>();
        minLineTransform.sizeDelta = new Vector2(dashedLineW, dashedLineH);
        minLineTransform.transform.localPosition = new Vector3(0, -height/2 + 10, 0);
        minLineTransform.localScale = Vector3.one;
        Image minLineImage = minLineObj.AddComponent<Image>();
        minLineImage.sprite = dashedLine;
        minLineImage.useSpriteMesh = true;
    }
}
