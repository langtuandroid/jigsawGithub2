using System;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

namespace UnityStandardAssets._2D
{
	public delegate void SettingEditorButtonCallback(FieldInfo field, object state);

    public class SettingsEditorController : MonoBehaviour
    {
		[SerializeField] private Button startButton;
		[SerializeField] private GameObject scrollViewContainer;
		[SerializeField] private GameObject boolButtonPrefab;
		[SerializeField] private GameObject numberButtonPrefab;
		[SerializeField] private Text headerText;

		private QuestionManager _questionManager;
		private AudioManager _audioManager;
		private SettingsManager _settingsManager;
		private GameplaySettings _settings;

		// Use this for initialization
		private void Start()
		{
			_questionManager = QuestionManager.GetQuestionManager();
			_audioManager = AudioManager.GetAudioManager();
			_settingsManager = SettingsManager.GetSettingsManager();

			headerText.text = "Edit settings for "+_settingsManager.GetNameOfCurrentGameMode()+" mode...";
			PopulateList(_settingsManager.GetDefaultSettingsForCurrentGameMode());
		}

		private void PopulateList(GameplaySettings settings)
		{
			_settings = settings;
			Debug.Log ("***** interrogate settings ****");
			Debug.Log ("type = "+settings.GetType());
			Debug.Log ("size = "+settings.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Length);
			foreach(var prop in settings.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)) 
			{
				Debug.Log (prop.Name+" = "+prop.GetValue(settings)+", is type "+prop.FieldType);
				//Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(settings, null));
				GameObject newButton = null;
				if (prop.FieldType == typeof(bool))
				{
					newButton = Instantiate (boolButtonPrefab) as GameObject;
					BooleanButton button = newButton.GetComponent<BooleanButton>();
					button.SetName(prop.Name);
					button.SetState((bool)prop.GetValue(settings));
					newButton.transform.SetParent(scrollViewContainer.transform);
					SettingEditorButtonCallback callback = ToggledBool;
					button.SetSelectCallback(callback, prop);
				}
				else if ((prop.FieldType == typeof(float)) || (prop.FieldType == typeof(int)))
				{
					newButton = Instantiate (numberButtonPrefab) as GameObject;
					NumericalButton button = newButton.GetComponent<NumericalButton>();
					button.SetName(prop.Name);
					SettingEditorButtonCallback callback = null;
					if (prop.FieldType == typeof(int)) 
					{
						button.SetValue((float)(int)prop.GetValue(settings));
						callback = ChangedInt;
					}
					else
					{
						button.SetValue((float)prop.GetValue(settings));
						callback = ChangedFloat;
					}
					newButton.transform.SetParent(scrollViewContainer.transform);
					button.SetSelectCallback(callback, prop);
				}
			}
		}

		// TODO : could these two functions actually be the same function?
		public void ToggledBool(FieldInfo field, object state)
		{
			Debug.Log ("ToggledBool "+state);
			field.SetValue(_settings, (bool)state);
		}

		public void ChangedFloat(FieldInfo field, object state)
		{
			Debug.Log ("Changed float "+state);
			field.SetValue(_settings, (float)state);
		}

		public void ChangedInt(FieldInfo field, object state)
		{
			Debug.Log ("Changed int "+state);
			field.SetValue(_settings, (int)(float)state);
		}

		public void ClickedBack ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressedBack");
			Application.LoadLevel("menu");
		}
    }
}
