using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    public enum MusicClipNames { Ginseng, YlangYlang, Mint };
    public static AudioManager Instance { get; private set; }

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    List<AudioClip> musicClips;

    [SerializeField]
    List<AudioClip> sfxClips;

    private void Awake()
    {
        CreateSingleton();
    }

    private void Start()
    {
        PlayMusic(MusicClipNames.YlangYlang);
    }

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
    public void PlayMusic(MusicClipNames trackName)
    {
        audioSource.clip = musicClips[(int)trackName];
        audioSource.Play();
    }


}
