using UnityEngine;
using System.Collections;

public class FireworkParticle : MonoBehaviour {

	private Vector2 velocity;
	private Vector2 acceleration;
	private float lifeSpan;

	// Use this for initialization
	void Start () 
	{
	}

	public void SetMotion(Vector2 velocityIn, Vector2 accelerationIn, float lifeSpanIn)
	{
		velocity = velocityIn;
		acceleration = accelerationIn;
		lifeSpan = lifeSpanIn;
		Destroy(gameObject, lifeSpan);
	}

	// Update is called once per frame
	void Update () 
	{
		float dTime = Time.deltaTime;
		Vector2 position = transform.localPosition;
		position.x += velocity.x * dTime;
		position.y += velocity.y * dTime;
		velocity.x += acceleration.x * dTime;
		velocity.y += acceleration.y * dTime;

		// air friction
		//float speed = Mathf.Sqrt(velocity.x*velocity.x + velocity.y*velocity.y);
		float airFriction = 0.98f;
		velocity.x *= airFriction;
		velocity.y *= airFriction;

		transform.localPosition = position;
	}
}
