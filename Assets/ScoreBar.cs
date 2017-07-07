//using UnityEngine;
//using System.Collections;
//using UnityEngine.UI;
//
//public class ScoreBar : MonoBehaviour {
//
//	public enum BarType
//	{
//		Bar,
//		Chain
//	}
//
//	[SerializeField] private Slider sliderBack;
//	[SerializeField] private Slider sliderFore;
//	[SerializeField] private LerpSlider lerpSliderBack;
//	[SerializeField] private LerpSlider lerpSliderFore;
//
//	private float _angle = 0f;
//	private int _numQuestions = 1;
//	private float score = 0.0f;
//
//	// Use this for initialization
//	void Start () {
//		score = 0.0f;
//	}
//	
//	// Update is called once per frame
//	void Update () {
//	
//	}
//
//	public void Initialise(int numberOfQuestions)
//	{
//		_numQuestions = numberOfQuestions;
//		ResetBar(numberOfQuestions);
//	}
//
//	public void ResetBar(int numQuestions)
//	{
//		score = 0.0f;
//		sliderBack.value = 0.0f;
//		sliderFore.value = 0.0f;
//	}
//
//
//	public float GetBarPos()
//	{
//		return score;
//	}
//
//	public void MoveBarBack(float dScore)
//	{
//		score += dScore;
//		if (score < 0f) score = 0f;
//		StopAllCoroutines();
//		//StopCoroutine("AnsweredQuestionBarSequence");
//		StartCoroutine(AnsweredQuestionBarSequence(score, false));
//	}
//
//	public void AnsweredQuestion(int questionNum, bool correct, float dScore)
//	{
//		score += dScore;
//		StartCoroutine(AnsweredQuestionBarSequence(score, true));
//	}
//
//	IEnumerator AnsweredQuestionBarSequence(float score, bool doTwoStageMove = true)
//	{
//		if (doTwoStageMove)
//		{
//			lerpSliderBack.LerpTo(score, 0.1f);
//			yield return new WaitForSeconds(0.7f);
//			lerpSliderFore.LerpTo(score, 0.6f);
//		}
//		else
//		{
//			lerpSliderBack.LerpTo(score, 0.1f);
//			lerpSliderFore.LerpTo(score, 0.1f);
//		}
//	}
//
//	private void AnsweredQuestionChain(int questionNum, bool correct, float dScore)
//	{
//
//	}
//
//}
