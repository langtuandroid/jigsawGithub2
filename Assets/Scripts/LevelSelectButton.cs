using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets._2D
{
    public class LevelSelectButton : MonoBehaviour
    {
		[SerializeField] public Text levelNameUI;
		[SerializeField] public string levelNameString;

		private LevelSelectButtonCallback levelSelectedCallback;

		// Use this for initialization
		private void Start()
		{
		}

		public void SetName(string name)
		{
			levelNameString = name;
			levelNameUI.text = name;
		}

		public void SetSelectCallback(LevelSelectButtonCallback callback)
		{
			levelSelectedCallback = callback;
		}

		public void buttonPressed()
		{
			levelSelectedCallback(levelNameString);
		}
    }
}
