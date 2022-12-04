using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [SerializeField] AudioMixer audioMixer;
    float masterVolume = 0.75f;
    float musicVolume = 0.3f;
    float effectVolume = 0.75f;

    public float GetMasterVolume() { return masterVolume; }
    public float GetMusicVolume() { return musicVolume; }
    public float GetEffectVolume () { return effectVolume; }

    public void SetMasterVolume(float _volume)
    {;
        masterVolume = _volume;
        _volume = Mathf.Clamp(_volume, 0.01f, 1);
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(_volume) * 20);
    }

    public void SetMusicVolume(float _volume)
    {
        musicVolume = _volume;
        _volume = Mathf.Clamp(_volume, 0.01f, 1);
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(_volume) * 20);
    }

    public void SetEffectVolume(float _volume)
    {
        effectVolume = _volume;
        _volume = Mathf.Clamp(_volume, 0.01f, 1);
        audioMixer.SetFloat("EffectVolume", Mathf.Log10(_volume) * 20);
    }

    public void Play(string _audioName)
    {
        transform.Find(_audioName).GetComponent<AudioSource>().Play();
    }

    public void Stop(string _audioName)
    {
        transform.Find(_audioName).GetComponent<AudioSource>().Stop();
    }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else Destroy(gameObject);
    }

}
