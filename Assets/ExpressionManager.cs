using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExpressionManager : MonoBehaviour {

	[SerializeField] public Texture expressionNeutral;
	[SerializeField] public Texture expressionHappy;
	[SerializeField] public Texture expressionSad;
	[SerializeField] public RawImage image;

	// Use this for initialization
	void Start () {
		image.texture = expressionNeutral;
	}
	
	public void ShowHappy(float time = -1.0f)
	{
		StartCoroutine(ExpressionSequence(expressionHappy, time));
	}
	
	public void ShowSad(float time = -1.0f)
	{
		StartCoroutine(ExpressionSequence(expressionSad, time));
	}

	public void ShowNeutral(float time = -1.0f)
	{
		StartCoroutine(ExpressionSequence(expressionNeutral, time));
	}

	IEnumerator ExpressionSequence(Texture texture, float time)
	{
		image.texture = texture;
		if (time > 0)
		{
			yield return new WaitForSeconds(time);
			image.texture = expressionNeutral;
		}
	}
}
