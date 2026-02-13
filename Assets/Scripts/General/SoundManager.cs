using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private bool sfxState = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayMusic(AudioClip audioClip)
    {
        if (musicSource.isPlaying)
        {
            return;
        }

        musicSource.clip = audioClip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void TurnMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        else
        {
            musicSource.Play();
        }
    }

    public void TurnSFX()
    {
        if (sfxState)
        {
            sfxState = false;
        }
        else
        {
            sfxState = true;
        }
    }

    public void PlaySFX(AudioClip audioClip)
    {
        if (!sfxState)
        {
            return;
        }

        sfxSource.PlayOneShot(audioClip);
    }
}
