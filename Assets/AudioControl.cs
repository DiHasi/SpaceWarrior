using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioControl : MonoBehaviour
{
    public AudioMixer AudioMixer;
    // Start is called before the first frame update

    public void SetVolumeMaster(float sliderValue)
    {
        AudioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
    }
    public void SetVolumeEffects(float sliderValue)
    {
        AudioMixer.SetFloat("EffectsVolume", Mathf.Log10(sliderValue) * 20);
    }
    public void SetVolumeSounds(float sliderValue)
    {
        AudioMixer.SetFloat("SoundsVolume", Mathf.Log10(sliderValue) * 20);
    }
}
