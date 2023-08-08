using tomi.SaveSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "LoadBehaviour", menuName = "Behaviours/LoadBehaviour")]
public class LoadComponent : ScriptableObject, LoadBehaviour
{
    public void Load()
    {
        if (SerializationManager.Load(Application.persistentDataPath + "/saves/playerData.deepbluestudio") != null)
        {
            SaveData.PlayerProfile = (PlayerProfile)SerializationManager.Load(Application.persistentDataPath + "/saves/playerData.deepbluestudio");
        }
        else
        {
            SetAudio();
            SetStats();

            SaveData.PlayerProfile.nonFirstTime = false;
        }
    }

    private void SetAudio()
    {
        SaveData.PlayerProfile.masterVolume = 0;
        SaveData.PlayerProfile.musicVolume = -20;
        SaveData.PlayerProfile.effectsVolume = -20;
    }

    private void SetStats()
    {
        SaveData.PlayerProfile.coins = 0;
        SaveData.PlayerProfile.highscore = 0;
    }
}
