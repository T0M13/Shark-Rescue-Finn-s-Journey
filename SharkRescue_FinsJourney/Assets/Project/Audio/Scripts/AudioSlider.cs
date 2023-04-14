using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.IO;
using tomi.Audio;
using tomi.SaveSystem;

public class AudioSlider : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioMixerGroup mixerGroup;
    public Slider slider;

    public AudioTyp audioType;


    private void Awake()
    {
        GetSlider();
    }

    private void OnValidate()
    {
        GetSlider();
    }

    private void GetSlider()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {

        AudioManager.instance.LoadAudioData();
        AudioManager.instance.CheckAudioData(); 

        switch (audioType)
        {
            case AudioTyp.Master:
                slider.value = SaveData.PlayerProfile.masterVolume;
                break;
            case AudioTyp.Music:
                slider.value = SaveData.PlayerProfile.musicVolume;
                break;
            case AudioTyp.Effects:
                slider.value = SaveData.PlayerProfile.effectsVolume;
                break;
            default:
                break;
        }


        ChangeVolume();
    }


    public void ChangeVolume()
    {
        UpdateVolume((int)slider.value, mixerGroup);
    }

    public void IncreaseVolume()
    {
        slider.value += 10;
        ChangeVolume();
    }

    public void DecreaseVolume()
    {
        slider.value -= 10;
        ChangeVolume();
    }


    public void UpdateVolume(int volume, AudioMixerGroup mixerGroup)
    {
        audioMixer.SetFloat(mixerGroup.name, volume);
        OnVolumeChange();
    }

    public void OnVolumeChange()
    {
        switch (audioType)
        {
            case AudioTyp.Master:
                SaveData.PlayerProfile.masterVolume = (int)slider.value;
                break;
            case AudioTyp.Music:
                SaveData.PlayerProfile.musicVolume = (int)slider.value;
                break;
            case AudioTyp.Effects:
                SaveData.PlayerProfile.effectsVolume = (int)slider.value;
                break;
            default:
                break;
        }

        SaveData.PlayerProfile.volumeEdited = true;

        AudioManager.instance.SaveAudioData();

    }

}
