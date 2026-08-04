using UnityEngine;




[CreateAssetMenu(fileName = "AnimationSettings", menuName = "Scriptable Objects/AnimationSettings")]
public class AnimationSettings : ScriptableObject
{
    [Header("Motion Data CSV")]
    [Tooltip("Note: Data not be uploaded to git \n Headers is column names")]
    public TextAsset MotionData_csv;
    public bool MotionData_ContainsHeaders;

    [Header("Fluke Amplitude LookUp CSV")]
    [Tooltip("Note: Data not be uploaded to git \n Headers is column names")]
    public TextAsset FlukeAmpLookUp_csv;
    public bool FlukeAmp_ContainsHeaders;

}
