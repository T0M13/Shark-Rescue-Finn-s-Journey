using System;
using System.Collections;
using tomi.Audio;
using tomi.SaveSystem;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("General Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("This Audio Source")]
    public AudioSource audioSource;

    [Header("Playable Songs")]
    public AudioClip[] songs;
    private int currentClip;

    [SerializeField] private bool prevSong = false;
    [SerializeField] private bool nextSong = false;

    private const string masterVolName = "Master";
    private const string musicVolName = "Music";
    private const string effectsVolName = "Effects";

    [Header("Audio Mixer Groups")]
    public AudioMixerGroup masterGroup;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup effectsGroup;

    [Header("Playable Sounds")]
    public Sound[] sounds;

    [Header("Save Load")]
    public SaveComponent saveBehaviour;
    public LoadComponent loadBehaviour;


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        GetAudioSource();
        SetAudioSourceMixer();
        SetSoundsSettings();

        prevSong = false;
        nextSong = false;
    }

    private void OnValidate()
    {
        GetAudioSource();
        SetAudioSourceMixer();

        if (nextSong)
        {
            ChangeToNextSong();
            nextSong = false;
        }
        if (prevSong)
        {
            ChangeToPrevSong();
            prevSong = false;
        }
        

    }

    private void Start()
    {

        LoadAudioData();

        CheckAudioData();

        PlayRandomSong();


    }

    private void Update()
    {
        if (songs.Length <= 0) return;
        if (audioSource.isPlaying) return;
        else
        {
            ChangeToNextSong();
        }

    }

    /// <summary>
    /// Plays a sound
    /// </summary>
    /// <param name="sound"></param>
    public void Play(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.Log("Can't find Sound");
        }
        s.source.Play();
        if (s.loop) return;
        StartCoroutine(RemoveAfterPlayed(s.clip.length, s.source));
    }

    IEnumerator RemoveAfterPlayed(float length, AudioSource audioSource)
    {
        yield return new WaitForSeconds(length);
        Destroy(audioSource);
    }

    /// <summary>
    /// Play Sound on Object
    /// </summary>
    /// <param name="sound"></param>
    /// <param name="obj"></param>
    public void PlayOnObject(string sound, GameObject obj)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        AudioSource objectAudioSource = GetAudioSource(s, obj);

        switch (s.type)
        {
            case AudioTyp.Master:
                objectAudioSource.outputAudioMixerGroup = masterGroup;
                break;
            case AudioTyp.Music:
                objectAudioSource.outputAudioMixerGroup = musicGroup;
                break;
            case AudioTyp.Effects:
                objectAudioSource.outputAudioMixerGroup = effectsGroup;
                break;
            default:
                break;
        }
        objectAudioSource.clip = s.clip;
        objectAudioSource.volume = s.volume;
        objectAudioSource.pitch = s.pitch;
        objectAudioSource.loop = s.loop;
        objectAudioSource.spatialBlend = s.spatialBand;
        objectAudioSource.Play();

        if (s.loop) return;
        StartCoroutine(RemoveAfterPlayed(s.clip.length, s.source));
        StartCoroutine(RemoveAfterPlayed(objectAudioSource.clip.length, objectAudioSource));
    }

    /// <summary>
    /// Gets n Adds AudioSource
    /// </summary>
    /// <param name="s"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public AudioSource GetAudioSource(Sound s, GameObject obj)
    {
        if (obj.GetComponents<AudioSource>().Length <= 0)
        {
            AudioSource objectAudioSource = obj.AddComponent<AudioSource>();
            return objectAudioSource;
        }
        else
        {
            AudioSource[] audioSourcesOnObject = obj.GetComponents<AudioSource>();
            AudioSource auds = Array.Find(audioSourcesOnObject, item => item.clip == s.clip);
            if (auds == null)
            {
                Debug.Log("No AudioSource w that clip found - Creating one");
                AudioSource objectAudioSource = obj.AddComponent<AudioSource>();
                return objectAudioSource;
            }
            else
            {
                Debug.Log("AudioSource found - using this one");
                AudioSource objectAudioSource = auds;
                return objectAudioSource;
            }

        }
    }

    /// <summary>
    /// Gets the current songs name
    /// </summary>
    /// <returns></returns>
    public string GetSongName()
    {
        return audioSource.clip.name;
    }

    /// <summary>
    /// Changes to the next song
    /// </summary>
    /// <returns></returns>
    public string ChangeToNextSong()
    {
        if (songs.Length <= 0) return null;
        currentClip++;
        if (currentClip > songs.Length - 1)
        {
            currentClip = 0;
        }
        audioSource.clip = songs[currentClip];
        audioSource.Play();

        return GetSongName();
    }

    /// <summary>
    /// Changes to the prev song
    /// </summary>
    /// <returns></returns>
    public string ChangeToPrevSong()
    {
        if (songs.Length <= 0) return null;
        currentClip--;
        if (currentClip < 0)
        {
            currentClip = songs.Length - 1;
        }
        audioSource.clip = songs[currentClip];
        audioSource.Play();

        return GetSongName();
    }

    /// <summary>
    /// Sets the settings of sound
    /// </summary>
    private void SetSoundsSettings()
    {
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            switch (s.type)
            {
                case AudioTyp.Master:
                    s.source.outputAudioMixerGroup = masterGroup;
                    break;
                case AudioTyp.Music:
                    s.source.outputAudioMixerGroup = musicGroup;
                    break;
                case AudioTyp.Effects:
                    s.source.outputAudioMixerGroup = effectsGroup;
                    break;
                default:
                    break;
            }
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }


    /// <summary>
    /// Gets the AudioSource of gameObject
    /// </summary>
    private void GetAudioSource()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    /// <summary>
    /// Sets audioSourceMixer Settings
    /// </summary>
    private void SetAudioSourceMixer()
    {
        audioSource.outputAudioMixerGroup = musicGroup;
        audioSource.playOnAwake = false;
    }



    /// <summary>
    /// Plays a random song from songs
    /// </summary>
    public void PlayRandomSong()
    {
        if (songs.Length <= 0) return;

        currentClip = UnityEngine.Random.Range(0, songs.Length);

        audioSource.clip = songs[currentClip];
        audioSource.Play();
    }

    /// <summary>
    /// Plays songs from the beginning
    /// </summary>
    public void PlaySongs()
    {
        if (songs.Length <= 0) return;

        currentClip = 0;

        audioSource.clip = songs[currentClip];
        audioSource.Play();
    }

    /// <summary>
    /// Plays songs from an index
    /// </summary>
    public void PlaySongAtIndex(int index)
    {
        if (songs.Length <= 0) return;

        currentClip = index;

        audioSource.clip = songs[currentClip];
        audioSource.Play();
    }

    /// <summary>
    /// Pauses current song
    /// </summary>
    public void PauseSong()
    {
        audioSource.Pause();
    }

    /// <summary>
    /// Resumes current song
    /// </summary>
    public void ResumeSong()
    {
        audioSource.Play();
    }

    /// <summary>
    /// Stops current song
    /// </summary>
    public void StopSong()
    {
        audioSource.Stop();
    }


    /// <summary>
    /// Sets the Volume of a mixer
    /// </summary>
    /// <param name="volume"></param>
    /// <param name="name"></param>
    private void SetVolumeOfMixer(int volume, string name)
    {
        audioMixer.SetFloat(name, volume);
    }

    public void CheckAudioData()
    {
        if (SaveData.PlayerProfile.volumeEdited)
        {
            SetVolumeOfMixer(SaveData.PlayerProfile.masterVolume, masterVolName);
            SetVolumeOfMixer(SaveData.PlayerProfile.musicVolume, musicVolName);
            SetVolumeOfMixer(SaveData.PlayerProfile.effectsVolume, effectsVolName);

        }
        else
        {
            SaveData.PlayerProfile.masterVolume = -40;
            SaveData.PlayerProfile.musicVolume = -40;
            SaveData.PlayerProfile.effectsVolume = -40;

            SaveAudioData();
            LoadAudioData();

            SetVolumeOfMixer(SaveData.PlayerProfile.masterVolume, masterVolName);
            SetVolumeOfMixer(SaveData.PlayerProfile.musicVolume, musicVolName);
            SetVolumeOfMixer(SaveData.PlayerProfile.effectsVolume, effectsVolName);
        }
    }


    /// <summary>
    /// Saves file 
    /// </summary>
    public void SaveAudioData()
    {
        saveBehaviour.Save();
    }

    /// <summary>
    /// Gets the SaveData file 
    /// </summary>
    public void LoadAudioData()
    {
        loadBehaviour.Load();
    }

}

