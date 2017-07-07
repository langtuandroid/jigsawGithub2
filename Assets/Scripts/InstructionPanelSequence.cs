using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets._2D
{
	public delegate void InstructionPanelSequenceCompletedCallback();

	public class InstructionPanelSequence : MonoBehaviour
    {
		[SerializeField] private GameObject _whatsBehindText;
		[SerializeField] private GameObject _tapToRevealText;

		private InstructionPanelSequenceCompletedCallback _sequenceCompletedCallback = null;

		// Use this for initialization
		private void Start()
		{

		}

		public void StartSequence(InstructionPanelSequenceCompletedCallback callback)
		{
			_sequenceCompletedCallback = callback;

			StartCoroutine(StartSequence ());
		}

		IEnumerator StartSequence ()
		{
			_whatsBehindText.SendMessage("Show");
			_tapToRevealText.SendMessage("Hide");

			float panelYCoord = _whatsBehindText.transform.localPosition.y;
			_whatsBehindText.transform.localPosition = new Vector3(1000,panelYCoord,0);
			Movement movementComponentBackPanel = _whatsBehindText.GetComponent<Movement>();
			if (movementComponentBackPanel != null)
				movementComponentBackPanel.MoveTo(new Vector2(0,panelYCoord), 0.2f);

			yield return new WaitForSeconds(1.0f);

			_tapToRevealText.GetComponent<Visibility>().Flash (3,0.2f,0.1f,true);

			yield return new WaitForSeconds(1.5f);

			_sequenceCompletedCallback();
		}
	}
}
