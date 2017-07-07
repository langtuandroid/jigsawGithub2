using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets._2D
{
    public class MenuController : MonoBehaviour
    {
		[SerializeField] private Button startButton;
		[SerializeField] private AudioManager audioManager;

		private QuestionManager _questionManager;
		private AudioManager _audioManager;
		private SettingsManager _settingsManager;

		// Use this for initialization
		private void Start()
		{
			Application.targetFrameRate = 60;

			_questionManager = QuestionManager.GetQuestionManager();
			_audioManager = AudioManager.GetAudioManager();
			_settingsManager = SettingsManager.GetSettingsManager();
			_settingsManager.ResetSettings();
		}
		
		public void ClickedStartquizType2 ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");
			_questionManager.SetSelectedQuizType(QuestionManager.QuizTypes.quizType2);
			Application.LoadLevel("LevelSelect");
		}

		public void ClickedStartquizType1 ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");
			_questionManager.SetSelectedQuizType(QuestionManager.QuizTypes.quizType1);
			Application.LoadLevel("LevelSelect");
		}

		public void ClickedStartquizType3 ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");
			_questionManager.SetSelectedQuizType(QuestionManager.QuizTypes.quizType3);
			Application.LoadLevel("LevelSelect");
		}

		public void ClickedquizType4 ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");
			_questionManager.SetSelectedQuizType(QuestionManager.QuizTypes.quizType4);
			Application.LoadLevel("LevelSelect");
		}
		
		public void ClickedquizType5 ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");
			_questionManager.SetSelectedQuizType(QuestionManager.QuizTypes.quizType5);
			Application.LoadLevel("LevelSelect");
		}
		
		public void ClickedquizType6 ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");
			_questionManager.SetSelectedQuizType(QuestionManager.QuizTypes.quizType6);
			Application.LoadLevel("LevelSelect");
		}
		
		public void ClickedquizType7 ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");
			_questionManager.SetSelectedQuizType(QuestionManager.QuizTypes.quizType7);
			Application.LoadLevel("LevelSelect");
		}
		
		public void ClickedJigsaw ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");
			_questionManager.SetSelectedQuizType(QuestionManager.QuizTypes.Jigsaw);
			Application.LoadLevel("LevelSelect");
		}
		
		public void ClickedquizType2Settings ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");

			_settingsManager.SetGameMode(SettingsManager.GameMode.quizType2);
			Application.LoadLevel("settingsEditor");
		}

		public void ClickedquizType1Settings ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");

			_settingsManager.SetGameMode(SettingsManager.GameMode.quizType1);
			Application.LoadLevel("settingsEditor");
		}
		
		public void ClickedquizType3Settings ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");
			
			_settingsManager.SetGameMode(SettingsManager.GameMode.quizType3);
			Application.LoadLevel("settingsEditor");
		}

		public void ClickedquizType4Settings ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");
			
			_settingsManager.SetGameMode(SettingsManager.GameMode.quizType4);
			Application.LoadLevel("settingsEditor");
		}
		
		public void ClickedquizType5Settings ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");
			
			_settingsManager.SetGameMode(SettingsManager.GameMode.quizType5);
			Application.LoadLevel("settingsEditor");
		}
		
		public void ClickedquizType6Settings ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");
			
			_settingsManager.SetGameMode(SettingsManager.GameMode.quizType6);
			Application.LoadLevel("settingsEditor");
		}

		public void ClickedquizType7Settings ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");
			
			_settingsManager.SetGameMode(SettingsManager.GameMode.quizType7);
			Application.LoadLevel("settingsEditor");
		}

		public void ClickedJigsawSettings ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");
			
			_settingsManager.SetGameMode(SettingsManager.GameMode.Jigsaw);
			Application.LoadLevel("settingsEditor");
		}

	}
}
