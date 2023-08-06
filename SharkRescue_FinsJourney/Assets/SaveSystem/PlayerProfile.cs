using System;
using System.Collections;
using UnityEngine;

namespace tomi.SaveSystem
{
    [System.Serializable]
    public class PlayerProfile
    {
        public int coins;
        public int highscore;

        public bool nonFirstTime;

        public int masterVolume;
        public int musicVolume;
        public int effectsVolume;
    }
}