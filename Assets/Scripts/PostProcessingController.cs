using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;


public class PostProcessingController : MonoBehaviour
{
    [SerializeField]
    private PostProcessVolume volume;
    [SerializeField]
    private float brightnessInScene = 0f;
    private ColorGrading colorGrading;
    [SerializeField]
    private float fadeInBrightness = 0;

    void Start()
    {
        if (volume != null && volume.profile != null)
        {
            volume.profile.TryGetSettings(out colorGrading);
        }
    }

    void Update()
    {
        if (colorGrading != null)
        {
            colorGrading.gain.value = new Vector4(brightnessInScene, brightnessInScene, brightnessInScene, brightnessInScene);
            colorGrading.gamma.value = new Vector4(fadeInBrightness, fadeInBrightness, fadeInBrightness, fadeInBrightness);
            colorGrading.lift.value = new Vector4(fadeInBrightness, fadeInBrightness, fadeInBrightness, fadeInBrightness);
        }
    }

  
}
