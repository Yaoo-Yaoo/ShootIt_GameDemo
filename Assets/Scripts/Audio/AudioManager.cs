using UnityEngine;

namespace SG
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioManager : MonoBehaviour
    {
        private AudioSource audioSource;
        [SerializeField] private AudioClip[] audioClips;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            EventManager.Instance.OnShotAudioPlayed.RegisterEvent(OnPlayShotAudio);
        }
        
        private void OnDestroy()
        {
            EventManager.Instance.OnShotAudioPlayed.UnRegisterEvent(OnPlayShotAudio);
        }
        
        private void OnPlayShotAudio()
        {
            Play(audioClips[0]);
        }

        private void Play(AudioClip clip, float startPercent = 0f)
        {
            audioSource.clip = clip;
            audioSource.time = clip.length * startPercent;
            audioSource.Play();
        }
    }
}
