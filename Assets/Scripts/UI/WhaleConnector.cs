/**
 * WhaleConnector.cs: Script which largely handles any interaction
 * with whale data and states.
 *
 * @author Mars Semenova, Nick Bindels
 */

using System;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.InputSystem;

public class WhaleConnector : MonoBehaviour {
    // params
    // scripts
    [Header("Scripts")]
    [SerializeField] private WhaleDriver whaleDriver; 
    [SerializeField] private Scrubber scrubber;
    [SerializeField] private TelemetryUI telemetry;
    
    // event
    public delegate void ResetEvent();
    public static event ResetEvent OnReset;
    
    // vars
    private float currentTimeStepDelta = 0.004f; 
    
    // I like this!

    void Awake() {

        // attach reset function to events
        Scrubber.OnRestart += Reset;
        ControlHints.OnUnstick += Reset;
        
        // attach whale pause/play to scrubber btns
        Scrubber.OnPause += PauseWhale;
        Scrubber.OnPlay += PlayWhale;
    }

    void Update() {
        UpdateTelemetry();
        UpdateTime();
        
        // set scrubber percentage
        float percent = (float)whaleDriver.currentTimestep / whaleDriver.CSV_ResetTimeStep * 100;
        if (scrubber) {
            scrubber.UpdateTimelineProgress(percent);
        } else {
            if (percent >= 100) {
                Reset();
            }
        }
    }

    private void OnDestroy() {
        // unsub
        Scrubber.OnRestart -= Reset;
        ControlHints.OnUnstick -= Reset;
        Scrubber.OnPause -= PauseWhale;
        Scrubber.OnPlay -= PlayWhale;
    }

    /**
     * Function which invokes the reset event.
     */
    public void Reset() {
        OnReset?.Invoke();
    }
    
    /**
     * Function which pauses the whale.
     */
    private void PauseWhale() { // TODO
        Time.timeScale = 0.0f;
    }

    /**
     * Function which resumes the whale.
     */
    private void PlayWhale() { // TODO
        Time.timeScale = 1.0f;
    }
    
    /**
     * Update the time in the scrubber.
     */
    private void UpdateTime() {
        // use if inversion
        if (scrubber) {
            float currentTimeStep = whaleDriver.currentTimestep;
            float secondsConvert = currentTimeStep * currentTimeStepDelta;

            float mins = secondsConvert / 60;

            float secs = secondsConvert % 60;
            int minsInt = Mathf.CeilToInt(mins);
            minsInt--;
            int secsInt = Mathf.CeilToInt(secs);
            secsInt--;

            if (minsInt < 0) {
                minsInt = 0;
            }

            if (secsInt < 0) {
                secsInt = 0;
            }

            scrubber.UpdateTime(minsInt, secsInt);
        }
    }

    /**
     * Update telemetry data.
     */
    private void UpdateTelemetry() {
        // if inversion
        if (telemetry) {
            // speed
            String speed = whaleDriver.whaleSpeed.ToString("F0") + " m/s";
            telemetry.UpdateSpeed(speed);

            // heading
            int heading = Mathf.CeilToInt(whaleDriver.whaleYaw) % 360;
            telemetry.UpdateHeading(heading);

            // roll
            int roll = Mathf.CeilToInt(whaleDriver.whaleRoll);
            telemetry.UpdateRoll(roll);

            // pitch
            int pitch = Mathf.CeilToInt(whaleDriver.whalePitch); // TODO
            telemetry.UpdatePitch(pitch);
        }
    }
}