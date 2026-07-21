/**
 * LineGraph.cs: Script which renders
 * a line graph.
 *
 * @author Mars Semenova
*/

using System.Linq;
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
    Material graphMaterial;
    
    // vars
    private float max;
    private float min;
    
    void Awake() {
        // get refs
        graphUI = Resources.Load<Sprite>("UI/Graphs/macrographbg");    
        dashedLine = Resources.Load<Sprite>("UI/Graphs/minmaxline");  
        graphMaterial = Resources.Load<Material>("UI/Graphs/GraphLineMat");
    }

    void Start() {
        // create bg
        CreateGraphUI();
        
        // create max line + txt
        // TODO: position line above or below sea lvl based on max
        max = dataY.Max();
        GameObject maxUIObj = new GameObject("MaxUI");
        maxUIObj.transform.SetParent(gameObject.transform);
        maxUIObj.transform.localPosition = Vector3.zero; 
        maxUIObj.transform.localScale = Vector3.one;
        
        GameObject maxTextObj = new GameObject("MaxText");
        maxTextObj.transform.SetParent(maxUIObj.transform);
        RectTransform maxTextTransform = maxTextObj.AddComponent<RectTransform>();
        maxTextTransform.sizeDelta = new Vector2(50, 20); // TODO: un-magic number
        maxTextTransform.transform.localPosition = new Vector3(width/2 + 50, height/2 - 10, 0); // TODO: make responsive
        maxTextTransform.localScale = Vector3.one;
        maxText = maxTextObj.AddComponent<TextMeshProUGUI>();
        maxText.verticalAlignment = VerticalAlignmentOptions.Middle;
        maxText.horizontalAlignment = HorizontalAlignmentOptions.Left;
        maxText.fontSize = 8;
        if (font != null) {
            maxText.font = font;
        }
        maxText.text = ((int) max).ToString();
        
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
        min = dataY.Min();
        GameObject minUIObj = new GameObject("MinUI");
        minUIObj.transform.SetParent(gameObject.transform);
        minUIObj.transform.localPosition = Vector3.zero; 
        minUIObj.transform.localScale = Vector3.one;
        
        GameObject minTextObj = new GameObject("MinText");
        minTextObj.transform.SetParent(minUIObj.transform);
        RectTransform minTextTransform = minTextObj.AddComponent<RectTransform>();
        minTextTransform.sizeDelta = new Vector2(50, 20); // TODO: un-magic number
        minTextTransform.transform.localPosition = new Vector3(width/2 + 50, -height/2 + 10, 0); // TODO: make responsive
        minTextTransform.localScale = Vector3.one;
        minText = minTextObj.AddComponent<TextMeshProUGUI>();
        minText.verticalAlignment = VerticalAlignmentOptions.Middle;
        minText.horizontalAlignment = HorizontalAlignmentOptions.Left;
        minText.fontSize = 8;
        if (font != null) {
            minText.font = font;
        }
        minText.text = ((int) min).ToString();
        
        GameObject minLineObj = new GameObject("MinLine"); 
        minLineObj.transform.SetParent(minUIObj.transform);
        RectTransform minLineTransform = minLineObj.AddComponent<RectTransform>();
        minLineTransform.sizeDelta = new Vector2(dashedLineW, dashedLineH);
        minLineTransform.transform.localPosition = new Vector3(0, -height/2 + 10, 0);
        minLineTransform.localScale = Vector3.one;
        Image minLineImage = minLineObj.AddComponent<Image>();
        minLineImage.sprite = dashedLine;
        minLineImage.useSpriteMesh = true;
        
        GraphData();
    }

    private void GraphData() {
        // set up obj
        GameObject graphObj = new GameObject("Graph");
        graphObj.transform.SetParent(gameObject.transform);
        graphObj.transform.localScale = Vector3.one;
        graphObj.layer = LayerMask.NameToLayer("Graph");
        LineRenderer line = graphObj.AddComponent<LineRenderer>();
        line.material = graphMaterial;
        line.positionCount = resolution + 1;
        
        // set positions
        RectTransform graphRect = GameObject.Find("LineGraph").GetComponent<RectTransform>();
        float widthWorld = width*graphRect.lossyScale.x; // TODO: make sure that this works even if multiple
        float heightWorld = height*graphRect.lossyScale.y; // TODO: make sure that this works even if multiple
        Vector3 offset = gameObject.transform.position - new Vector3(widthWorld/2-10, 0, 0);
        float range = max - min;
        int indDiff = dataY.Length / (resolution+1);
        float xDiff = (widthWorld-20) / resolution; 
        float minHeight =  -heightWorld / 2 + 10*graphRect.lossyScale.y;
        float maxHeight =  heightWorld / 2 - 10*graphRect.lossyScale.y;
        float rangeWorld = maxHeight - minHeight;
        float dataPointX, dataPointY, pointH;
        for (int x = 0; x <= resolution; x++) {
            dataPointX = dataX[x*indDiff];
            dataPointY = dataY[x*indDiff];
            pointH = dataPointY / max;
            line.SetPosition(x, new Vector3(xDiff*x, minHeight + rangeWorld*pointH, -1) + offset);
        }
    }
}
