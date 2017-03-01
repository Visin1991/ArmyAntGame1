using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public enum AudioChannel{Master,Sfx,Music}

    [Range(0.0f,1.0f)]
	public float masterVolumePercent = .5f;
    [Range(0.0f, 1.0f)]
    public float sfxVolumePercent = 1f;
    [Range(0.0f, 1.0f)]
    public float musicVolumePercent = .5f;

	AudioSource sfx2DSource;
	AudioSource[] musicSources;
	int activeMusicSourceIndex;

	public static AudioManager instance;
	Transform audioListener;
	Transform playerT;

	SoundLib library;

	void Awake()
	{
		if(instance !=null)
		{
			Destroy(gameObject); //When we reload a new Sence. We get a copy. so we need to Destroy it.
		}else{
			DontDestroyOnLoad(gameObject);

			instance = this;

			library = GetComponent<SoundLib>();

			musicSources = new AudioSource[2];
			for(int i = 0;i<2;i++)
			{
				GameObject newMusicSource = new GameObject("Music source" + (i +1));
				musicSources[i] =  newMusicSource.AddComponent<AudioSource>();
				newMusicSource.transform.parent = transform;
			}
			GameObject newSfx2Dsource = new GameObject("2D sfx source");
			sfx2DSource = newSfx2Dsource.AddComponent<AudioSource>();
			newSfx2Dsource.transform.parent = transform;

			audioListener = FindObjectOfType<AudioListener>().transform;
			playerT = FindObjectOfType<Player>().transform;

			masterVolumePercent = PlayerPrefs.GetFloat("master Volume",masterVolumePercent);
			musicVolumePercent = PlayerPrefs.GetFloat("music Volume",masterVolumePercent);
			sfxVolumePercent = PlayerPrefs.GetFloat("Sfx Volume",sfxVolumePercent);
		}
	}

	void Update()
	{
		if(playerT != null)
		{
			audioListener.position = playerT.position;
		}
	}

	public void SetVolume(float volumePercent,AudioChannel channel)
	{
		switch(channel)
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

		PlayerPrefs.SetFloat("master Volume",masterVolumePercent);
		PlayerPrefs.SetFloat("music Volume",musicVolumePercent);
		PlayerPrefs.SetFloat("Sfx Volume",sfxVolumePercent);
	}

	public void PlayMusic(AudioClip clip,float fadeDuration = 1)
	{
        Debug.Log(activeMusicSourceIndex);
		activeMusicSourceIndex = 1 - activeMusicSourceIndex;
		musicSources[activeMusicSourceIndex].clip = clip;
		musicSources[activeMusicSourceIndex].Play();
		StartCoroutine(AnimateMusicCrossfade(fadeDuration));
	}

	public void PlaySound(AudioClip clip, Vector3 pos)
	{
		if(clip != null)
			AudioSource.PlayClipAtPoint(clip,pos,sfxVolumePercent * masterVolumePercent);
	}

	public void PlaySound(string soundName,Vector3 pos)
	{
		PlaySound(library.GetClipFromName(soundName),pos);
	}

	public void PlaySound2D(string soundName)
	{
		sfx2DSource.PlayOneShot(library.GetClipFromName(soundName),sfxVolumePercent * masterVolumePercent);
	}

	IEnumerator AnimateMusicCrossfade(float duration)
	{
		float percent = 0;
		while(percent < 1)
		{
			percent += Time.deltaTime * 1/duration;
			musicSources[activeMusicSourceIndex].volume =   Mathf.Lerp(0,musicVolumePercent * masterVolumePercent,percent);
			musicSources[1-activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent,0,percent);
			yield return null;
		}
	}

}
