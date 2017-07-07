using UnityEngine;
using System.Collections;
using UnityEditor;

// This will eventually be used to customize inspector

namespace unitycoder_MobilePaint
{

	[CustomEditor(typeof(MobilePaint),true)]
	public class CustomMobilePaintInspector : Editor 
	{

		public override void OnInspectorGUI()
		{
			// for now, just draw the default inspector
			DrawDefaultInspector();

		}
	}
}