using UnityEngine;
using System.Collections;

public class CoinController : MonoBehaviour {

	Vector2 _velocity = new Vector2(0f,10f);
	Vector2 _acceleration = new Vector2(0f,-0.5f);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector2 pos = transform.localPosition;
		pos += _velocity;
		_velocity += _acceleration;
		transform.localPosition = pos;
		if (pos.y < -1000f)
			Destroy(gameObject);
	}
}
