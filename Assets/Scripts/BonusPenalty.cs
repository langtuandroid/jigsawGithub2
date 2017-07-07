using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace UnityStandardAssets._2D
{
    public class BonusPenalty : MonoBehaviour
    {
		[SerializeField] private Movement motion;
		[SerializeField] private Visibility visibility;

		// Use this for initialization
		private void Start()
		{
			//transform.localPosition = new Vector3(0,0,0);
			motion.MoveBy(new Vector2(0,500),2.0f, true);
			visibility.Flash(10, 0.08f, 0.08f, false);
			StartCoroutine(DeleteSelfAfterDelay(0.5f));
		}

		IEnumerator DeleteSelfAfterDelay(float delayTime)
		{
			yield return new WaitForSeconds(delayTime);
			UnityEngine.Object.Destroy(this.gameObject);
		}
    }
}
