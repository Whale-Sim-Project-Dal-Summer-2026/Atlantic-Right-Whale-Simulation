/**
 * IGraph.cs: Interface for graph types
 *
 * @author Mars Semenova
*/

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Graph : MonoBehaviour {
    // UI
    protected float width = 120; // TODO: in future can make this adjustable
    protected float height = 50;
    protected Sprite graphUI;
    protected TMP_FontAsset font;
    
    // vars
    protected float[] dataX;
    protected float[] dataY;
    protected int resolution;
    
    /**
     * Set data to be used in the graph.
     * @param data - Data passed as a TextAsset.
     */
    public void SetData(float[] dX, float[] dY) {
        dataX = dX;
        dataY = dY;
    }
    
    /**
     * Set data to be used in the graph.
     * @param data - Data passed as a TextAsset.
     */
    public void SetFont(TMP_FontAsset fontAsset) {
        font = fontAsset;
    }

    /**
     * Create the graph background.
     */
    protected void CreateGraphUI() {
        RectTransform graphUITransform = gameObject.AddComponent<RectTransform>();
        graphUITransform.sizeDelta = new Vector2(width, height);
        graphUITransform.transform.localPosition = Vector3.zero;
        graphUITransform.localScale = Vector3.one;
        Image graphUIImage = gameObject.AddComponent<Image>();
        graphUIImage.sprite = graphUI;
        graphUIImage.useSpriteMesh = true;
    }
    
    public void SetResolution(int res) {
        resolution = res;
    }
}