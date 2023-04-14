using System;
using System.Collections;
using UnityEngine;

namespace tomi.SaveSystem
{
    [System.Serializable]
    public class PlayerProfile
    {
        public string playerName;

        public bool volumeEdited;
        public int masterVolume;
        public int musicVolume;
        public int effectsVolume;
    }
}