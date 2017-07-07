using System;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

namespace UnityStandardAssets._2D
{
    public class BooleanButton : MonoBehaviour
    {
		[SerializeField] public Text varNameUI;
		[SerializeField] public Toggle toggleUI;

		private string varNameString;
		private bool boolState;
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

		public void SetState(bool state)
		{
			boolState = state;
			toggleUI.isOn = state;
		}

		public void SetSelectCallback(SettingEditorButtonCallback callback, FieldInfo field)
		{
			levelSelectedCallback = callback;
			fieldInfo = field;
		}

		public void boolChanged()
		{
			if (levelSelectedCallback != null)
			{
				boolState = toggleUI.isOn;
				Debug.Log ("boolChanged = "+boolState);
				levelSelectedCallback(fieldInfo, boolState);
			}
		}
    }
}
