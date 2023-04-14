using tomi.SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

    [CreateAssetMenu(fileName = "SaveBehaviour", menuName = "Behaviours/SaveBehaviour")]
    public class SaveComponent : ScriptableObject, SaveBehaviour
    {
        public void Save()
        {
            SerializationManager.Save("playerData", SaveData.PlayerProfile);
        }
    }
