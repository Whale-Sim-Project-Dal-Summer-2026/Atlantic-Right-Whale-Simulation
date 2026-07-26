/**
 * WhaleTrail.cs: Script which implements the whale's trail
 * using a spline.
 *
 * @author Mars Semenova 
 */

using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class WhaleTrail : MonoBehaviour {
    // params
    // options
    [Header("Options")]
    [SerializeField] private int interval = 10;
    // refs
    [Header("References")]
    [SerializeField] private GameObject whale;
    [SerializeField] private GameObject whaleTrail;
    
    // vars
    private Spline spline;
    private MeshRenderer whaleTrailMesh;
    private Vector3 whalePos;
    private int liveKnot = 1;
    private bool isVisible = true;
    
    void Awake() {
        whaleTrailMesh = GetComponent<MeshRenderer>();
        whaleTrail.transform.position = whale.transform.position;
        spline = whaleTrail.GetComponent<SplineContainer>()[0];
        
        // events
        WhaleConnector.OnReset += ResetPath;
        Toggles.OnToggleUIOn += SetPathVisibilityOn; // TODO: only for pub side
        Toggles.OnToggleUIOff += SetPathVisibilityOff; // TODO: only for pub side
        Toggles.OnTogglePathOn += SetPathVisibilityOn;
        Toggles.OnTogglePathOff += SetPathVisibilityOff;
    }

    void Start() {
        whalePos = whale.transform.position;
        
        // fix initial knots
        spline.SetTangentMode(TangentMode.AutoSmooth);
        BezierKnot newKnotObj = new BezierKnot(new float3(0,0,0));
        newKnotObj.Rotation = whale.transform.rotation; // TODO: make sure this is actually working
        spline.SetKnot(0, newKnotObj);
        spline.SetKnot(1, newKnotObj);
    }

    void Update() {
        Vector3 currWhalePos = whale.transform.position;
        Vector3 whaleOffset = currWhalePos - whalePos;
        BezierKnot currKnot = spline[liveKnot], currStaticKnot = spline[liveKnot - 1];
        Vector3 newPos = (Vector3)currKnot.Position + whaleOffset;
        currKnot.Position = newPos;
        whalePos = currWhalePos;
        spline.SetKnot(liveKnot, currKnot);
        Vector3 currStaticSplinePos = currStaticKnot.Position;
        if (Vector3.Distance(currStaticSplinePos, newPos) > interval) {
            BezierKnot newKnotObj = new BezierKnot(newPos);
            newKnotObj.Rotation = whale.transform.rotation; // TODO: make sure this is actually working
            spline.Add(newKnotObj, TangentMode.AutoSmooth);
            liveKnot++;
        }
    }

    /**
     * Reset path on whale reset.
     */
    private void ResetPath() {
        whalePos = whale.transform.position;
        spline.Clear();
        spline.SetTangentMode(TangentMode.AutoSmooth);
        BezierKnot newKnotObj = new BezierKnot(new float3(0,0,0));
        newKnotObj.Rotation = whale.transform.rotation; // TODO: make sure this is actually working
        spline.Add(newKnotObj, TangentMode.AutoSmooth);
        spline.Add(newKnotObj, TangentMode.AutoSmooth);
        liveKnot = 1;
    }

    /**
     * Set the path visibility.
     * @param on - Whether to set it on or off.
     */
    private void SetPathVisibility(bool on) {
        isVisible = on;
        if (whaleTrailMesh) { 
            whaleTrailMesh.enabled = on;
        }
    }
    private void SetPathVisibilityOn() { // TODO
        SetPathVisibility(true);
    }
    private void SetPathVisibilityOff() { // TODO
        SetPathVisibility(false);
    }

    /**
     * Check whether the path is visible.
     * @return Whether the path is visible.
     */
    public bool IsVisible() {
        return isVisible;
    }
}