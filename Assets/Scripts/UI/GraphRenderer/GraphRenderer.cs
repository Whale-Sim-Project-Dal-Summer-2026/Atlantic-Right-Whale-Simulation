/**
 * GraphRenderer.cs: Script which renders
 * graphs.
 *
 * @author Mars Semenova 
 */

using TMPro;
using UnityEngine;

public class GraphRenderer : MonoBehaviour {
    // obj
    private GameObject genedGraph;
    private Graph script;
    
    // data
    public TextAsset dataset;
    
    // graph options enum
    public enum GraphType {
        None,
        Line,
        LineRolling
    }
    public GraphType graphType;
    
    // customization
    public TMP_FontAsset font;

    void Start() {
        // check validity
        if (graphType == GraphType.None || dataset == null) {
            gameObject.SetActive(false);
        } else {
            CreateGraph();
        }
    }
    
    /**
     * Set data to be used in the graph.
     * @param data - Data passed as a TextAsset.
     */
    public void SetData(TextAsset data) {
        dataset = data;

        if (dataset == null) {
            gameObject.SetActive(false);
            return;
        }
        
        if (graphType != GraphType.None) {
            if (genedGraph == null) { // if graph doesn't exist create
                CreateGraph();
            } else { // otherwise update data
                script.SetData(data);
            }
        }
    }

    /**
     * Set graph type to be used for the graph.
     * @param type - Type passed as a GraphType enum.
     */
    public void SetGraphType(GraphType type) {
        graphType = type;
        if (dataset != null) {
            CreateGraph();
        }
    }

    /**
     * Create graph based on passed parameters.
     */
    void CreateGraph() {
        // delete if exists
        if (genedGraph != null) {
            Destroy(genedGraph);
        }
        
        // create
        if (graphType == GraphType.None) {
            gameObject.SetActive(false);
            return;
        }

        // line graph
        if (graphType == GraphType.Line) {
            genedGraph = new GameObject("LineGraph");
            script = genedGraph.AddComponent<LineGraph>();
        }

        // rolling line graph
        if (graphType == GraphType.LineRolling) {
            genedGraph = new GameObject("LineRollingGraph");
            script = genedGraph.AddComponent<LineRollingGraph>();
        }
        
        genedGraph.transform.SetParent(gameObject.transform);
        script.SetData(dataset);
        script.SetFont(font);
        gameObject.SetActive(true);
    }
}
