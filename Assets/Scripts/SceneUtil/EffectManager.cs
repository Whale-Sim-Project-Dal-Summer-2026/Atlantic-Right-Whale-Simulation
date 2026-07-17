using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Effectmanager : MonoBehaviour{
    [SerializeField] Camera camera;

    [SerializeField] Volume volume;

    Vignette vignetteComponent;
    FilmGrain filmGrainComponent;

    [SerializeField] bool effectsEnabled;
    [SerializeField] ProcessingSettings settings;

    float seaLevel;


    void Start(){
        volume.profile.TryGet<Vignette>(out vignetteComponent);
        volume.profile.TryGet<FilmGrain>(out filmGrainComponent);

        seaLevel = settings.SeaLevel;
    }

    void Update()
    {

        effectsEnabled = camera.transform.position.y <= seaLevel;
        RenderSettings.fog = effectsEnabled;

        float intensity = effectsEnabled ? 1.0f : 0.0f;

        vignetteComponent.intensity.value = intensity;
        filmGrainComponent.intensity.value = intensity;

        
    }
}