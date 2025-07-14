using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private PostProcessVolume volume;
    [SerializeField]
    private float brightnessInScene = 0f;
    private ColorGrading colorGrading;
    [SerializeField]
    private PlayableDirector _fadeOutTimeline;
    // Start is called before the first frame update
    void Start()
    {
        if (volume != null && volume.profile != null)
        {
            volume.profile.TryGetSettings(out colorGrading);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (colorGrading != null)
        {
            colorGrading.gain.value = new Vector4(brightnessInScene, brightnessInScene, brightnessInScene, brightnessInScene);
            colorGrading.gamma.value = new Vector4(brightnessInScene, brightnessInScene, brightnessInScene, brightnessInScene);
            colorGrading.lift.value = new Vector4(brightnessInScene, brightnessInScene, brightnessInScene, brightnessInScene);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void FadeOut()
    {
        if (_fadeOutTimeline != null)
        {
            _fadeOutTimeline.Play();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}