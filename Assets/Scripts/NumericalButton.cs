using System;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

namespace UnityStandardAssets._2D
{
    public class NumericalButton : MonoBehaviour
    {
		[SerializeField] public Text varNameUI;
		[SerializeField] public InputField inputBoxUI;

		private string varNameString;
		private float numValue;
		private FieldInfo fieldInfo;

		private SettingEditorButtonCallback levelSelectedCallback = null;

		// Use this for initialization
		private void Start()
		{
		}

		public void SetName(string name)
		{
			varNameString = name;
			varNameUI.text = name;
		}

		public void SetValue(float value)
		{
			numValue = value;
			inputBoxUI.text = value.ToString();
		}

		public void SetSelectCallback(SettingEditorButtonCallback callback, FieldInfo field)
		{
			levelSelectedCallback = callback;
			fieldInfo = field;
		}

		public void numberChanged()
		{
			if (levelSelectedCallback != null)
			{
				numValue = float.Parse(inputBoxUI.text);
				Debug.Log ("number changed = "+numValue);
				levelSelectedCallback(fieldInfo, numValue);
			}
		}
    }
}
