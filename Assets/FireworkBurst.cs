using UnityEngine;
using System.Collections;

namespace UnityStandardAssets._2D
{
	public class FireworkBurst : MonoBehaviour {

		private Visibility vis = null;
		private float lifeSpan = 0.5f;

		// Use this for initialization
		void Start () {
			vis = gameObject.GetComponent<Visibility>();
			vis.Show();
			vis.FadeOut(lifeSpan);
			Destroy (gameObject,lifeSpan);
		}
	}
}
