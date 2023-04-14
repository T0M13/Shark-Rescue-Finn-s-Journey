using UnityEngine;
using UnityEngine.Audio;

namespace tomi.Audio
{

    [System.Serializable]
    public class Sound
    {

        public string name;

        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume;

        [Range(-3f, 3f)]
        public float pitch;

        public bool loop = false;
        public float spatialBand;

        public AudioTyp type;

        [HideInInspector]
        public AudioSource source;


    }
}