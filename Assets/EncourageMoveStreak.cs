using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UnityStandardAssets._2D
{
	public class EncourageMoveStreak : MonoBehaviour {
		
		[SerializeField] private Visibility[] visibilityComponents;
		[SerializeField] private Text messageText;
		[SerializeField] private GameObject[] secondaryMessages;

		private Vector2 originalPos;
		private AudioManager _audioManager;
		private QuestionManager _questionManager;
		private int _streakCount;
		private CallBack finishedCallback = null;

		public void StartAnimation () 
		{
			_audioManager = AudioManager.GetAudioManager();
			_questionManager = QuestionManager.GetQuestionManager();
			StartCoroutine(ZoomSequence());
		}

		public void SetCount(int count)
		{
			_streakCount = count;
		}

		public void SetCallback(CallBack callback)
		{
			finishedCallback = callback;
		}

		private void EnableSpecificSecondaryMessage(int index)
		{
			for (int i=0; i<secondaryMessages.Length; i++)
			{
				GameObject parent = secondaryMessages[i].transform.parent.gameObject;
				parent.SetActive(i == index);
			}
		}

		IEnumerator ZoomSequence() 
		{
			float fadeInTime = 0.2f;
			float pauseTime = 1.0f;
			float fadeOutTime = 0.35f;

			messageText.text = "streak x"+_streakCount;

			//yield return new WaitForSeconds(0.9f);

			int secondaryMessageNum = 0;
			if (_streakCount == _questionManager.GetNumberOfQuestions())
			{
				secondaryMessageNum = 3;
				if (_audioManager)
					_audioManager.PlayAudioClip("streakPerfectRun");
			}
			else if (_streakCount >= 6)
			{
				secondaryMessageNum = 2;
				if (_audioManager)
					_audioManager.PlayAudioClip("streakAmazing");
			}
			else if (_streakCount >= 4)
			{
				secondaryMessageNum = 1;
				if (_audioManager)
					_audioManager.PlayAudioClip("streakExcellent");
			}
			else
			{
				if (_audioManager)
					_audioManager.PlayAudioClip("streakNormal");
			}



			EnableSpecificSecondaryMessage(secondaryMessageNum);

			foreach (Visibility vis in visibilityComponents)
			{
				vis.Hide ();
				vis.FadeIn (0.25f);
			}
			secondaryMessages[secondaryMessageNum].SendMessage("Hide");
			secondaryMessages[secondaryMessageNum].SendMessage("FadeIn",0.25f);
			Visibility parentVis = secondaryMessages[secondaryMessageNum].transform.parent.gameObject.GetComponent<Visibility>();
			parentVis.SetInitialAlpha(100f/255);
			parentVis.Hide ();
			parentVis.FadeIn (0.25f);

//			float aspectRatio = (float)texture.width / texture.height;
//			pictureAspectRatioFitterUI.aspectRatio = aspectRatio;
			
			Scaler scaler = gameObject.GetComponent<Scaler>();
			scaler.ScaleBackFrom (0.8f, fadeInTime);

			yield return new WaitForSeconds(fadeInTime);
			yield return new WaitForSeconds(pauseTime);

			scaler.ScaleTo (2.0f, fadeOutTime);

			foreach (Visibility vis in visibilityComponents)
			{
				vis.FadeOut (fadeOutTime/2);
			}
			secondaryMessages[secondaryMessageNum].SendMessage("FadeOut",fadeOutTime/2);
			parentVis.FadeOut (fadeOutTime/2);

			yield return new WaitForSeconds(fadeOutTime);

			if (finishedCallback != null)
				finishedCallback();
			Destroy(gameObject, 0.0f);
		}
	}
}

