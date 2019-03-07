using UnityEngine;

public class AudioEffects : MonoBehaviour
{
    public static AudioEffects _audioEffects;
    private AudioSource _audioSource;

    private void Start()
    {
        if (_audioEffects == null)
        {
            _audioEffects = this;
            _audioSource = GetComponent<AudioSource>();
        }
        else if (_audioEffects != this)
        {
            Destroy(this);
        }
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        _audioSource.PlayOneShot(clip);
    }
}
