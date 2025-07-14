using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    private AudioMixerGroup _mutedAudio;

    [SerializeField]
    private AudioSource _roundMusic;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRoundMusic()
    {

    }

    public void StartBossMusic()
    {

    }

    public void StopAllMusic()
    {

    }

    public void MuteOnGameOver()
    {
        if(_mutedAudio != null){
            _mutedAudio.audioMixer.SetFloat("Volume", -80f);
        }
    }
}