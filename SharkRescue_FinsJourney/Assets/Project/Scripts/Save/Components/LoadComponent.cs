using tomi.SaveSystem;
using UnityEngine;

    [CreateAssetMenu(fileName = "LoadBehaviour", menuName = "Behaviours/LoadBehaviour")]
    public class LoadComponent : ScriptableObject, LoadBehaviour
    {
        public void Load()
        {
            SaveData.PlayerProfile = (PlayerProfile)SerializationManager.Load(Application.persistentDataPath + "/saves/playerData.deepbluestudio");
        }
    }
