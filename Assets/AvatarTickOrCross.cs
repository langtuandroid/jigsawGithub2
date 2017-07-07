using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AvatarTickOrCross : MonoBehaviour {

	public enum TickOrCross
	{
		none = 0,
		tick,
		cross
	}

	[SerializeField] private RawImage rawImage;
	[SerializeField] public Texture tickTexture;
	[SerializeField] public Texture crossTexture;

	void Awake () {
		rawImage.enabled = false;
	}

	public void Show(TickOrCross type)
	{
		if (type == TickOrCross.none)
		{
			rawImage.enabled = false;
			return;
		}

		rawImage.enabled = true;
		float displayTime = 2.0f;
		if (type == TickOrCross.tick)
		{
			StartCoroutine(ShowSequence(tickTexture, displayTime));
		}
		else if (type == TickOrCross.cross)
		{
			StartCoroutine(ShowSequence(crossTexture, displayTime));
		}
	}

	IEnumerator ShowSequence(Texture texture, float time)
	{
		rawImage.texture = texture;
		yield return new WaitForSeconds(time);
		//rawImage.enabled = false;
	}
}
