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
        }
    }

    private void SetAudio()
    {
        SaveData.PlayerProfile.masterVolume = -40;
        SaveData.PlayerProfile.musicVolume = -40;
        SaveData.PlayerProfile.effectsVolume = -40;
    }
}
