using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExpandImage : MonoBehaviour {

	// zoom into texture by adjusting uv coords.

	[SerializeField] RawImage rawImage;

	float startZoom, endZoom;
	float zoomDuration;
	float timePassed = 0f;
	bool moving = false;
	bool isLinear = true;

	// Use this for initialization
	void Start () {
		moving = false;
	}

	public void StartZoom(float startZoomIn, float endZoomIn, float time)
	{
		startZoom = startZoomIn;
		endZoom = endZoomIn;
		zoomDuration = time;
		timePassed = 0f;
		moving = true;
	}


	public void StartZoom(float startZoomIn, float startTime, float endZoomIn, float endTime, float currentTime)
	{
		if (currentTime >= endTime)
		{
			currentTime = endTime;
		}

		zoomDuration = endTime - currentTime;
		if (currentTime < startTime)
			startZoom = startZoomIn;
		else
			startZoom = startZoomIn + ((endZoomIn - startZoomIn)*(currentTime - startTime)/(endTime - startTime));
		endZoom = endZoomIn;
		timePassed = 0f;
		moving = true;
	}

	// Update is called once per frame
	void Update () 
	{
		if (moving)
		{
			timePassed += Time.deltaTime;
			float t = timePassed / zoomDuration;
			
			if (t >= 1) 
			{
				t = 1;
				moving = false;
			}
			if (!isLinear)
				t = t*t * (3f - 2f*t);

			float zoom = startZoom + t*(endZoom-startZoom);

			float portionOfImage = 1f/zoom;
			Rect uvRect = new Rect(0.5f-(0.5f*portionOfImage), 0.5f-(0.5f*portionOfImage), portionOfImage, portionOfImage);
			//RawImage raw = GetComponent<RawImage>();
			rawImage.uvRect = uvRect;
		}
	}
}
