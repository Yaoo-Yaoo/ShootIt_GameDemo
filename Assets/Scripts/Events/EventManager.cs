using UnityEngine;

namespace SG
{
    public class EventManager
    {
        public SGEvent<float, float, float> OnCameraShake;
        public SGEvent<Vector3, float> OnBombBlast;
        public SGEvent OnGameOver;
        public SGEvent OnShotAudioPlayed;
        
        #region Singleton
        private static EventManager _instance;
        public static EventManager Instance
        {
            get
            {
                if (_instance == null) _instance = new EventManager();
                return _instance;
            }
        }
        #endregion
        
        private EventManager()
        {
            OnCameraShake = new SGEvent<float, float, float>();
            OnBombBlast = new SGEvent<Vector3, float>();
            OnGameOver = new SGEvent();
            OnShotAudioPlayed = new SGEvent();
        }
    }
}
