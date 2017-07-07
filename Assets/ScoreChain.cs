//using UnityEngine;
//using System.Collections;
//
//public class ScoreChain : MonoBehaviour {
//
//	[SerializeField] public GameObject singleHexPrefab;
//	[SerializeField] private int directionX = 1, directionY = 0;
//
//	enum ChainHexStates
//	{
//		empty = 0,
//		correct,
//		wrong
//	}
//
//	private int numberOfHexes;
//	private GameObject[] hexes = null;
//
//	// Use this for initialization
//	void Awake() 
//	{
//	}
//
//	private void initialise(int numQuestions, float screenWidth)
//	{
//		float hexSize = 250.0f;
//
//		numberOfHexes = numQuestions;
//
//		if (hexes == null)
//			hexes = new GameObject[numberOfHexes];
//
//		float maxChainLength = screenWidth * 0.40f;
//
//		for (int i=0; i<numberOfHexes; i++)
//		{
//			float scaleX = 0.15f;	//0.14f;	//0.16f;
//			float scaleY = 0.15f;
//
//			if (directionY == 0)
//			{
//				// horizontal chain
//				Debug.Log ("maxChainLength = "+maxChainLength);
//				Debug.Log ("numberOfHexes = "+numberOfHexes);
//				if ((numberOfHexes * hexSize * scaleX) > maxChainLength)
//				{
//					scaleX = maxChainLength / (numberOfHexes * hexSize);
//				}
//				Debug.Log ("scaleX = "+scaleX);
//
////				if (numberOfHexes > 8)
////					scale = (0.12f * 10)/numberOfHexes;
//			}
//
//			if (hexes[i] == null)
//			{
//				if (directionY != 0 && directionX != 0)
//				{
//					// grid of hexes. For totup screen.
//					float xCentre = (hexSize * (directionX-1))/2;
//					int x = i % directionX;
//					int y = (i / directionX) * directionY;
//					hexes[i] = CreateHexAtPos((x*hexSize - xCentre)*scaleX, y*hexSize*scaleY, scaleX, scaleY);
//				}
//				else
//				{
//					hexes[i] = CreateHexAtPos(directionX*i*hexSize*scaleX, directionY*i*hexSize*scaleY, scaleX, scaleY);
//				}
//			}
//
//			SetHexState(i, ChainHexStates.empty);
//		}
//	}
//
//	private GameObject CreateHexAtPos(float xPos, float yPos, float scaleX, float scaleY)
//	{
//		GameObject newHex = Instantiate (singleHexPrefab) as GameObject;
//		newHex.transform.SetParent(gameObject.transform);
//		newHex.transform.localPosition = new Vector3(xPos, yPos, 0);
//		newHex.transform.localScale = new Vector3(scaleX,scaleY,1f);
//
//		return newHex;
//	}
//
//	private void SetHexState(int hexIndex, ChainHexStates state)
//	{
//		GameObject hex = hexes[hexIndex];
//		ChainHex ch = hex.GetComponent<ChainHex>();
//
//		switch (state)
//		{
//		case ChainHexStates.empty:
//			ch.SetEmpty();
//			break;
//		case ChainHexStates.correct:
//			ch.SetCorrect();
//			break;
//		case ChainHexStates.wrong:
//			ch.SetWrong();
//			break;
//		}
//	}
//
//	public void AnsweredQuestion(int questionNum, bool correct, float score)
//	{
//		SetHexState(questionNum, correct ? ChainHexStates.correct : ChainHexStates.wrong);
//	}
//
//	public void ResetChain(int numQuestions, float screenWidth)
//	{
//		initialise(numQuestions, screenWidth);
//	}
//}
