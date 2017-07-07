using UnityEngine;
using System.Collections;

public class Scaler : MonoBehaviour {

	private Vector3 startScale;
	private Vector3 destinationScale;
	private float travelTime;
	private float timePassed;
	private bool moving;
	private bool isLinear;
	
	// Use this for initialization
	private void Start()
	{
		
	}
	
	private void Awake()
	{
		moving = false;
	}
	
	// multiplies current scale by given factor, and then interpolates from that back to the currect scale
	public void ScaleBackFrom(float scaleFactor, float time, bool linear = false)
	{
		RectTransform rt = gameObject.GetComponent<RectTransform>();
		destinationScale = rt.localScale;
		startScale = destinationScale * scaleFactor;
		rt.localScale = startScale;
		travelTime = time;
		timePassed = 0.0f;
		moving = true;
		isLinear = linear;
	}

	// multiplies current scale by given factor, and scales toward it
	public void ScaleTo(float scaleFactor, float time, bool linear = false)
	{
		RectTransform rt = gameObject.GetComponent<RectTransform>();
		ScaleToAbsolute(rt.localScale * scaleFactor,time,linear);
	}

	// multiplies current scale by given factor, and scales toward it
	public void ScaleToAbsolute(Vector3 scale, float time, bool linear = false)
	{
		RectTransform rt = gameObject.GetComponent<RectTransform>();
		startScale = rt.localScale;
		destinationScale = scale;
		travelTime = time;
		timePassed = 0.0f;
		moving = true;
		isLinear = linear;
	}

	private void Update()
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

			Vector3 p = Vector3.Lerp(startScale,destinationScale,t);
			RectTransform rt = gameObject.GetComponent<RectTransform>();
			p.z = 1.0f;
			rt.localScale = p;
		}
	}

}
