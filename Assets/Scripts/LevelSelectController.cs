using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets._2D
{
	public delegate void LevelSelectButtonCallback(string levelName);

    public class LevelSelectController : MonoBehaviour
    {
		[SerializeField] private Button startButton;
		[SerializeField] private GameObject scrollViewContainer;
		[SerializeField] private GameObject buttonPrefab;

		private QuestionManager _questionManager;
		private AudioManager _audioManager;
		private SettingsManager _settingsManager;

		// Use this for initialization
		private void Start()
		{
			_questionManager = QuestionManager.GetQuestionManager();
			_audioManager = AudioManager.GetAudioManager();
			_settingsManager = SettingsManager.GetSettingsManager();


//			// make question manager persist between level select and the game
//			if (_questionManager)
//				DontDestroyOnLoad(_questionManager.gameObject);

			PopulateList();
		}

		private void PopulateList()
		{
			string[] allQuizNames = _questionManager.AllQuizNamesInCurrentMode();
			for (int i=0; i<allQuizNames.Length; i++)
			{
				GameObject newButton = Instantiate (buttonPrefab) as GameObject;
				newButton.transform.SetParent(scrollViewContainer.transform);
				LevelSelectButton button = newButton.GetComponent<LevelSelectButton>();
				button.SetName(allQuizNames[i]);
				LevelSelectButtonCallback callback = SelectedLevel;
				button.SetSelectCallback(callback);
			}
		}

		public void SelectedLevel(string levelName)
		{
			Debug.Log ("**** Selected level "+levelName);
			_questionManager.SetQuiz(levelName);
			string sceneName = _questionManager.GetQuizSceneNameForCurrentMode();

			Application.LoadLevel(sceneName);
		}

		public void ClickedBack ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressedBack");
			Application.LoadLevel("menu");
		}
    }
}
