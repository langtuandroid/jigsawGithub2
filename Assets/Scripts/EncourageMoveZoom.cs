using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityStandardAssets._2D
{
	public class EncourageMoveZoom : MonoBehaviour {

		private Vector2 originalPos;
		private AudioManager _audioManager;
		private CallBack finishedCallback = null;

		// Use this for initialization
		void Start () 
		{
			_audioManager = AudioManager.GetAudioManager();
			StartCoroutine(ZoomSequence());
		}
		
		public void SetCallback(CallBack callback)
		{
			finishedCallback = callback;
		}

		IEnumerator ZoomSequence() 
		{
			float movement1time = 0.2f;
			float movement2time = 1.0f;
			float movement3time = 0.2f;

			if (_audioManager)
				_audioManager.PlayAudioClip("speedyMessageComesIn");

			Vector2 pos = transform.localPosition;
			originalPos = pos;
			pos.x += 2000f;
			transform.localPosition = pos;
			Movement mov = gameObject.GetComponent<Movement>();

			mov.MoveTo(originalPos, movement1time);
			yield return new WaitForSeconds(movement1time);

			mov.MoveBy(new Vector2(32f,0f), movement2time, true);
			if (_audioManager)
				_audioManager.PlayAudioClip("speedyMessageSound");
			yield return new WaitForSeconds(movement2time);

			if (_audioManager)
				_audioManager.PlayAudioClip("speedyMessageComesIn");
			mov.MoveBy(new Vector2(-2000f,0f), movement3time);
			yield return new WaitForSeconds(movement3time);

			if (finishedCallback != null)
				finishedCallback();
			Destroy(gameObject, 0.0f);
		}
	}
}
