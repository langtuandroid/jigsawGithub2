using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

namespace UnityStandardAssets._2D
{
	public class SettingsManager : MonoBehaviour
    {
		public enum GameMode
		{
			quizType1,
			quizType2,
			quizType3,
			quizType4,
			quizType5,
			quizType6,
			quizType7,
			Jigsaw
		}

//		[NonSerialized] public quizType1QuizSettings quizType1Settings = new quizType1QuizSettings();
//		[NonSerialized] public quizType2QuizSettings quizType2Settings = new quizType2QuizSettings();
//		[NonSerialized] public quizType3QuizSettings quizType3Settings = new quizType3QuizSettings();
//		[NonSerialized] public quizType4QuizSettings quizType4Settings = new quizType4QuizSettings();
//		[NonSerialized] public quizType5QuizSettings quizType5Settings = new quizType5QuizSettings();
//		[NonSerialized] public quizType6Settings quizType6Settings = new quizType6Settings();
//		[NonSerialized] public quizType7QuizSettings quizType7Settings = new quizType7QuizSettings();
		[NonSerialized] public JigsawQuizSettings jigsawSettings = new JigsawQuizSettings();

//		[SerializeField] public quizType1QuizSettings defaultquizType1Settings = new quizType1QuizSettings();
//		[SerializeField] public quizType2QuizSettings defaultquizType2Settings = new quizType2QuizSettings();
//		[SerializeField] public quizType3QuizSettings defaultquizType3Settings = new quizType3QuizSettings();
//		[SerializeField] public quizType4QuizSettings defaultquizType4Settings = new quizType4QuizSettings();
//		[SerializeField] public quizType5QuizSettings defaultquizType5Settings = new quizType5QuizSettings();
//		[SerializeField] public quizType6Settings defaultquizType6Settings = new quizType6Settings();
//		[SerializeField] public quizType7QuizSettings defaultquizType7Settings = new quizType7QuizSettings();
		[SerializeField] public JigsawQuizSettings defaultJigsawSettings = new JigsawQuizSettings();

		private GameMode _currentlySelectedGameMode;


		private void Start()
		{
			ResetSettings();
		}

		public void ResetSettings()
		{
//			quizType1Settings = (quizType1QuizSettings)defaultquizType1Settings.ShallowCopy();
//			quizType2Settings = (quizType2QuizSettings)defaultquizType2Settings.ShallowCopy();
//			quizType3Settings = (quizType3QuizSettings)defaultquizType3Settings.ShallowCopy();
//			quizType4Settings = (quizType4QuizSettings)defaultquizType4Settings.ShallowCopy();
//			quizType5Settings = (quizType5QuizSettings)defaultquizType5Settings.ShallowCopy();
//			quizType6Settings = (quizType6Settings)defaultquizType6Settings.ShallowCopy();
//			quizType7Settings = (quizType7QuizSettings)defaultquizType7Settings.ShallowCopy();
			jigsawSettings = (JigsawQuizSettings)defaultJigsawSettings.ShallowCopy();
		}

		public void SetGameMode(GameMode mode)
		{
			_currentlySelectedGameMode = mode;
		}

		public GameMode GetGameMode()
		{
			return _currentlySelectedGameMode;
		}

		public GameplaySettings GetSettingsForCurrentGameMode()
		{
			return GetSettingsForGameMode(_currentlySelectedGameMode);
		}
		
		public GameplaySettings GetDefaultSettingsForCurrentGameMode()
		{
			return GetDefaultSettingsForGameMode(_currentlySelectedGameMode);
		}

		public GameplaySettings GetSettingsForGameMode(GameMode mode)
		{
			switch(mode)
			{
//			case GameMode.quizType1:
//				return quizType1Settings;
//			case GameMode.quizType2:
//				return quizType2Settings;
//			case GameMode.quizType3:
//				return quizType3Settings;
//			case GameMode.quizType4:
//				return quizType4Settings;
//			case GameMode.quizType5:
//				return quizType5Settings;
//			case GameMode.quizType6:
//				return quizType6Settings;
//			case GameMode.quizType7:
//				return quizType7Settings;
			case GameMode.Jigsaw:
				return jigsawSettings;
			default:
				return null;
			}
		}
		
		public GameplaySettings GetDefaultSettingsForGameMode(GameMode mode)
		{
			switch(mode)
			{
//			case GameMode.quizType1:
//				return defaultquizType1Settings;
//			case GameMode.quizType2:
//				return defaultquizType2Settings;
//			case GameMode.quizType3:
//				return defaultquizType3Settings;
//			case GameMode.quizType4:
//				return defaultquizType4Settings;
//			case GameMode.quizType5:
//				return defaultquizType5Settings;
//			case GameMode.quizType6:
//				return defaultquizType6Settings;
//			case GameMode.quizType7:
//				return defaultquizType7Settings;
			case GameMode.Jigsaw:
				return defaultJigsawSettings;
			default:
				return null;
			}
		}
		
		public string GetNameOfCurrentGameMode()
		{
			return GetNameOfGameMode(_currentlySelectedGameMode);
		}

		public string GetNameOfGameMode(GameMode mode)
		{
			switch(mode)
			{
			case GameMode.quizType1:
				return "quizType1";
			case GameMode.quizType2:
				return "quizType2";
			case GameMode.quizType3:
				return "quizType3";
			case GameMode.quizType4:
				return "quizType4";
			case GameMode.quizType5:
				return "quizType5";
			case GameMode.quizType6:
				return "quizType6";
			case GameMode.quizType7:
				return "quizType7";
			case GameMode.Jigsaw:
				return "Jigsaw";
			default:
				return null;
			}
		}

		public static bool SettingsManagerExists()
		{
			GameObject settingsManagerObject = GameObject.Find ("SettingsManagerPrefab(Clone)");
			return (settingsManagerObject != null);
		}

		public static SettingsManager GetSettingsManager()
		{
			// tries to find question manager in the scene
			// if not, then create one and initialise it.
			GameObject settingsManagerObject = GameObject.Find ("SettingsManagerPrefab(Clone)");
			if (settingsManagerObject)
			{
				// return the existing one
				return settingsManagerObject.GetComponent<SettingsManager>();
			}
			else
			{
				// create a new one
				settingsManagerObject = Instantiate(Resources.Load("SettingsManagerPrefab")) as GameObject;
				SettingsManager settingsManager = settingsManagerObject.GetComponent<SettingsManager>();
				DontDestroyOnLoad(settingsManager.gameObject);	// make it persist between scenes
				return settingsManager;
			}
		}

    }
}
