using UnityEngine;
using System.Collections;

namespace tomi.SaveSystem
{

    [System.Serializable]
    public class SaveData
    {
        #region SaveData - Other
        private static SaveData _current;

        public static SaveData Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new SaveData();
                }
                return _current;
            }
            set
            {
                if (value != null)
                {
                    _current = value;
                }
            }
        }

        #endregion

        #region Player Profile
        private static PlayerProfile _playerProfile;

        public static PlayerProfile PlayerProfile
        {
            get
            {
                if (_playerProfile == null)
                {
                    _playerProfile = new PlayerProfile();
                }
                return _playerProfile;
            }
            set
            {
                if (value != null)
                {
                    _playerProfile = value;
                }
            }
        }
        #endregion

    }
}