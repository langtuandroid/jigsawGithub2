//using UnityEngine;
//using System.Collections;
//using UnityEngine.UI;
//
//public class ChainHex : MonoBehaviour 
//{
//	[SerializeField] public GameObject glow;
//	[SerializeField] public GameObject cross;
//	[SerializeField] public GameObject tick;
//
//	// Use this for initialization
//	void Awake () {
//		SetEmpty ();
//	}
//
//	public void SetEmpty()
//	{
//		glow.SetActive(false);
//		cross.SetActive(false);
//		tick.SetActive(false);
//	}
//
//	public void SetCorrect()
//	{
//		RawImage raw = glow.GetComponent<RawImage>();
//		Color col = raw.color;
//		col.r = 58f/255;
//		col.g = 255f/255;
//		col.b = 78f/255;
//		col.a = 0.5f;	//0.6f;	//92f/255;
//		raw.color = col;
//		glow.SetActive(true);
//		cross.SetActive(false);
//		tick.SetActive(true);
//	}
//	
//	public void SetWrong()
//	{
//		RawImage raw = glow.GetComponent<RawImage>();
//		Color col = raw.color;
//		col.r = 255f/255;
//		col.g = 58f/255;
//		col.b = 78f/255;
//		col.a = 0.6f;	//92f/255;
//		raw.color = col;
//		glow.SetActive(true);
//		cross.SetActive(true);
//		tick.SetActive(false);
//	}
//}
