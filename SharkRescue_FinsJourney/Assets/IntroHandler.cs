using System.Collections;
using System.Collections.Generic;
using tomi.SaveSystem;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;

public class IntroHandler : MonoBehaviour
{
    [Header("Video Clip")]
    [SerializeField] private VideoPlayer player;
    [SerializeField] private Transform clipOutput;
    [Header("UI Main Menu")]
    [SerializeField] private TextMeshProUGUI coins;
    [SerializeField] private TextMeshProUGUI score;


    private void Awake()
    {
        player = GetComponent<VideoPlayer>();
    }

    private void OnEnable()
    {
        player.loopPointReached += Deactivate;
    }

    private void OnDisable()
    {
        player.loopPointReached -= Deactivate;
    }

    private void Start()
    {
        if (!SaveData.PlayerProfile.nonFirstTime)
        {
            PlayVideoClip();
        }
        else
        {
            clipOutput.gameObject.SetActive(false);
        }

        coins.text = SaveData.PlayerProfile.coins.ToString();
        score.text = SaveData.PlayerProfile.highscore.ToString();
    }


    private void Deactivate(UnityEngine.Video.VideoPlayer vp)
    {
        AudioManager.instance.GetComponent<AudioSource>().volume = 1;
        SaveData.PlayerProfile.nonFirstTime = true;
        clipOutput.gameObject.SetActive(false);
        player.Stop();
        GameManager.Instance.OnSave?.Invoke();
    }

    public void OpenWebPage()
    {
        Application.OpenURL("https://deepbluestudio.at");
    }

    public void PlayVideoClip()
    {
        AudioManager.instance.GetComponent<AudioSource>().volume = 0;

        clipOutput.gameObject.SetActive(true);
        player.Play();
    }
}
