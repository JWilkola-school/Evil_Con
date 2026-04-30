using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public void PlaySoundEffect(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
