using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets._2D
{
	[System.Serializable]
	class SoundEffect : System.Object
	{
		public string name;
		public AudioClip audioClip;
		public float volume;
	}

	public class AudioManager : MonoBehaviour
    {
		[SerializeField] private AudioSource audioSource;
		[SerializeField] private AudioSource pitchShiftingAudioSource;
		[SerializeField] private SoundEffect[] sounds2;

		//[SerializeField] private Dictionary<string, Texture2D> textureDictionary = new Dictionary<string, Texture2D>();

		// Use this for initialization
		private void Start()
		{
			//LoadAllTexturesInDirectory ();
		}
		
		public static AudioManager GetAudioManager()
		{
			// tries to find audio manager in the scene
			// if not, then create one and initialise it.
			GameObject audioManagerObject = GameObject.Find ("AudioManagerPrefab(Clone)");
			if (audioManagerObject)
			{
				// return the existing one
				return audioManagerObject.GetComponent<AudioManager>();
			}
			else
			{
				// create a new one
				audioManagerObject = Instantiate(Resources.Load("AudioManagerPrefab")) as GameObject;
				AudioManager audioManager = audioManagerObject.GetComponent<AudioManager>();
				DontDestroyOnLoad(audioManager.gameObject);	// make it persist between scenes
				return audioManager;
			}
		}

		private AudioClip audioClipFromName(string name)
		{
			// yes - this is very inefficient
			foreach (SoundEffect s in sounds2)
			{
				if (s.name == name)
				{
					return s.audioClip;
					break;
				}
			}
			
			return null;
		}
		
		private float volumeFromName(string name)
		{
			// yes - this is very inefficient
			foreach (SoundEffect s in sounds2)
			{
				if (s.name == name)
				{
					return s.volume;
					break;
				}
			}
			
			return 0.0f;
		}
		
		public void PlayAudioClip(string name)
		{
			AudioClip audioClip = audioClipFromName(name);
			if (audioClip)
				audioSource.PlayOneShot(audioClip, volumeFromName (name));
		}

		public void StopSoundEffect()
		{
			audioSource.Stop ();
		}

		public void PlayAudioClip(string name, float pitchShift)
		{
			AudioClip audioClip = audioClipFromName(name);
			if (audioClip)
			{
				pitchShiftingAudioSource.PlayOneShot(audioClip, volumeFromName (name));
				pitchShiftingAudioSource.pitch = pitchShift;
			}
		}

		public int GetNumberOfAudioClips()
		{
			return sounds2.Length;
		}
    }
}
