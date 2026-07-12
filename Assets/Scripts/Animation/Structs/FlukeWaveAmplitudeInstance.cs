namespace FlukeWaveAmplitudeLookUpClass{
/// <summary>
/// Stores one lookup instance of the Fluke Amplitude
/// </summary>
[System.Serializable]
public class FlukeWaveAmplitudeInstance{

    /// <summary>
    /// the speed the whale is moving in m/s 
    /// </summary>
    public float mean_speed;
    /// <summary>
    /// current phase of the whale
    /// </summary>
    public string phase;

    /// <summary>
    /// is mouth open
    /// </summary>
    public bool mouthOpen;

    /// <summary>
    /// Predicted amplitude of the whales fluking motion
    /// </summary>
    public double frequency;

    /// <summary>
    /// Predicted amplitude of the whales fluking motion
    /// </summary>
    public double amplitude;


}
}