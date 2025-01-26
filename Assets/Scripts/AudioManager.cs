using System;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField]
    private AudioSource sfxAudioSource;

    [SerializeField]
    private Ambient[] ambientClips;

    [SerializeField]
    private Sound[] sfxClips;

    private void Awake() => CreateSingleton();

    private void Start() => PlayAmbientSounds();

    private void CreateSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void PlayAmbientSounds()
    {
        foreach (Ambient a in ambientClips)
        {
            if (a.clips != null && a.clips.Length > 0)
            {
                AudioSource audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.loop = true;
                audioSource.clip = a.clips[UnityEngine.Random.Range(0, a.clips.Length)];
                audioSource.volume = a.volume;
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("Ambient clips array is empty or null for an Ambient object.");
            }
        }
    }

    public void PlaySFX(string soundName)
    {
        foreach (Sound s in sfxClips)
        {
            if (s.name == soundName)
            {
                sfxAudioSource.PlayOneShot(s.clip, s.volume);
            }
        }
    }

    #region Class
    [Serializable]
    private class Ambient
    {
        public float volume;
        public AudioClip[] clips;
    }

    [Serializable]
    private class Sound
    {
        public string name;
        public float volume;
        public AudioClip clip;
    }
    #endregion
}