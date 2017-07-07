using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShakeImage : MonoBehaviour {

	[SerializeField] RawImage rawImage;
	float shakeMagnitude = 0.01f;

	// Use this for initialization
	void Start () 
	{
	}

	public void SetShakeMagnitude(float shake)
	{
		shakeMagnitude = shake;
		if (shakeMagnitude == 0)
			StopAllCoroutines();
		else
			StartCoroutine(DoShakeSequence());
	}


	IEnumerator DoShakeSequence()
	{
		Rect uvRect = new Rect(0f,0f,1f,1f);
		while(true)
		{
			if (shakeMagnitude > 0f)
			{
				uvRect.x = -shakeMagnitude;
				rawImage.uvRect = uvRect;
				yield return new WaitForSeconds(0.05f);
				uvRect.x = shakeMagnitude;
				rawImage.uvRect = uvRect;
			}
			yield return new WaitForSeconds(0.05f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
