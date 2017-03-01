using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeiAudioManager : MonoBehaviour
{

    public enum AudioChannel { Master, Sfx, Music }

    [Range(0.0f, 1.0f)]
    public float masterVolumePercent = .5f;
    [Range(0.0f, 1.0f)]
    public float sfxVolumePercent = 1f;
    [Range(0.0f, 1.0f)]
    public float musicVolumePercent = .5f;

    AudioSource sfx2DSource;
    AudioSource[] musicSources; //
    int activeMusicSourceIndex;

    public static WeiAudioManager instance;
    Transform audioListener;
    Transform playerT;

    WeiSoundLib library;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject); //When we reload a new Sence. We get a copy. so we need to Destroy it.
        }
        else
        {
            DontDestroyOnLoad(gameObject);

            instance = this;

            library = GetComponent<WeiSoundLib>();

            //music Sources used for background Music. use tow music sources for fiding effect
            musicSources = new AudioSource[2];

            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Music source" + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }

            //this is used for 2D sfx effect
            GameObject newSfx2Dsource = new GameObject("2D sfx source");
            sfx2DSource = newSfx2Dsource.AddComponent<AudioSource>();
            newSfx2Dsource.transform.parent = transform;

            audioListener = FindObjectOfType<AudioListener>().transform;
            playerT = FindObjectOfType<Player>().transform;

            masterVolumePercent = PlayerPrefs.GetFloat("master Volume", masterVolumePercent);
            musicVolumePercent = PlayerPrefs.GetFloat("music Volume", masterVolumePercent);
            sfxVolumePercent = PlayerPrefs.GetFloat("Sfx Volume", sfxVolumePercent);
        }
    }

    void Update()
    {
        if (playerT != null)
        {
            //by default audioListener is a component of mainCamera.
            //For more realistic 3D sound effect. we attach the audioListener to Player
            audioListener.position = playerT.position;
        }
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                break;
        }
        musicSources[0].volume = musicVolumePercent * masterVolumePercent;
        musicSources[1].volume = musicVolumePercent * masterVolumePercent;

        PlayerPrefs.SetFloat("master Volume", masterVolumePercent);
        PlayerPrefs.SetFloat("music Volume", musicVolumePercent);
        PlayerPrefs.SetFloat("Sfx Volume", sfxVolumePercent);
    }

    /// <summary>
    ///     activeMusicSourceIndex start at 0; 1 -0 = 1. So The first call. We will use musicSources[1]
    /// the next time when we call this function again. 1 -1 = 0, then we will use musicSources[0].
    /// Note:
    ///     https://docs.unity3d.com/ScriptReference/AudioSource.html
    ///     AudioSource-->can play a single audio clip using Play,Pause and Stop.....volume property...
    /// </summary>
    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();
        StartCoroutine(AnimateMusicCrossfade(fadeDuration));
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(library.GetClipFromName(soundName), pos);
    }

    public void PlaySound(string soundName, Vector3 pos, int index)
    {
        PlaySound(library.GetClipFromName(soundName, index), pos);
    }

    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(library.GetClipFromName(soundName), sfxVolumePercent * masterVolumePercent);
    }

    public void PlaySound2D(string soundName, int index)
    {
        sfx2DSource.PlayOneShot(library.GetClipFromName(soundName,index), sfxVolumePercent * masterVolumePercent);
    }

    IEnumerator AnimateMusicCrossfade(float duration)
    {
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
    }

}