using System;
using UnityEngine;
using UnityEngine.UI;

public class LerpSlider : MonoBehaviour {

	[SerializeField] private Slider slider;

	private float startPos;
	private float destinationPos;
	private float travelTime;
	private float timePassed;
	private bool moving;
	private bool isLinear;

	// Use this for initialization
	void Start () {
	
	}

	public void LerpTo(float value, float time, bool linear = false)
	{
		startPos = slider.value;
		destinationPos = value;
		travelTime = time;
		timePassed = 0.0f;
		moving = true;
		isLinear = linear;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (moving)
		{
			timePassed += Time.deltaTime;
			float t = timePassed / travelTime;
			if (t >= 1) 
			{
				t = 1;
				moving = false;
			}
			if (!isLinear)
				t = t*t * (3f - 2f*t);
			float p = startPos + (destinationPos-startPos)*t;

			slider.value = p;
		}	
	}
}
