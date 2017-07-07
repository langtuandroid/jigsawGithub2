using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GlintLayer : MonoBehaviour {

	private enum State
	{
		Waiting,
		Active
	}

	private RawImage rawImage;
	private float timer;
	private const float xScale = 0.5f;
	private const float speed = 3f;
	private State state;

	// Use this for initialization
	void Start () {
		Reset ();
		state = State.Waiting;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (state == State.Active)
		{
			timer += Time.deltaTime;
			SetXPos (1f - timer*speed);
		}
	}

	void SetXPos(float x)
	{
		Rect uvRect = new Rect(x,0f,xScale,1f);
		rawImage.uvRect = uvRect;
	}

	public void Reset()
	{
		rawImage = GetComponent<RawImage>();
		timer = 0.0f;
		SetXPos(1f);
	}

	public void DoGlint()
	{
		Reset ();
		state = State.Active;
	}
}
