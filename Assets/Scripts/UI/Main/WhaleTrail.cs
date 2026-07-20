/**
 * DynamicSpline.cs: Script which implements the whale's trail
 * using a spline.
 *
 * @author Mars Semenova 
 */

using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class WhaleTrail : MonoBehaviour {
    private Spline spline;
    private GameObject whale;
    private Vector3 whalePos;
    private int liveKnot = 1;
    private int interval = 10;
    
    void Awake() {
        spline = GameObject.Find("WhaleTrail").GetComponent<SplineContainer>()[0];
        whale = GameObject.Find("Right Whale SF Mouth Articulation 1");
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
        BezierKnot currKnot = spline[liveKnot], currStaticKnot = spline[liveKnot-1];
        Vector3 newPos = (Vector3) currKnot.Position + whaleOffset;
        currKnot.Position =  newPos;
        whalePos = currWhalePos;
        spline.SetKnot(liveKnot, currKnot);
        Vector3 currStaticSplinePos = currStaticKnot.Position;
        if (Vector3.Distance(currStaticSplinePos, newPos) > interval) {
            BezierKnot newKnotObj = new BezierKnot(newPos);
            newKnotObj.Rotation = whale.transform.rotation; // TODO: make sure this is actually working
            spline.Add(newKnotObj, TangentMode.AutoSmooth);
            liveKnot++;
        }

        spline.EnforceTangentModeNoNotify(new SplineRange(0, liveKnot));
    }
}