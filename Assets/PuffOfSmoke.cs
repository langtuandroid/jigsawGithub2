using UnityEngine;
using System.Collections;

namespace UnityStandardAssets._2D
{
	public class PuffOfSmoke : MonoBehaviour {

		// Use this for initialization
		void Start () 
		{
			const float animTime = 3.0f;
			GetComponent<Visibility>().FadeOut (animTime);
			GetComponent<Scaler>().ScaleTo(2.0f, animTime/4);
			GetComponent<Movement>().MoveBy(new Vector2(0,200f),animTime);
		}
	}
}