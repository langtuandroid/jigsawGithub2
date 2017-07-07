using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityStandardAssets._2D
{
	public class EncourageMoveDoubleDown : MonoBehaviour {

		//private Vector2 originalPos;
		private AudioManager _audioManager;
		private CallBack finishedCallback = null;

		// Use this for initialization
		void Start () 
		{
			_audioManager = AudioManager.GetAudioManager();
			StartCoroutine(DoubleDownSequence());
		}
		
		public void SetCallback(CallBack callback)
		{
			finishedCallback = callback;
		}

		IEnumerator DoubleDownSequence() 
		{
			float movement1time = 1.0f;

			if (_audioManager)
				_audioManager.PlayAudioClip("speedyMessageComesIn");

			//gameObject.GetComponent<Visibility>().Flash(100, 0.1f, 0.1f, true);
			yield return new WaitForSeconds(movement1time);

			if (finishedCallback != null)
				finishedCallback();
			Destroy(gameObject, 0.0f);
		}
	}
}
