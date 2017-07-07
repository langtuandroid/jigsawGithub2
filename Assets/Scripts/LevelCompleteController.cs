//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//
//namespace UnityStandardAssets._2D
//{
//    public class LevelCompleteController : MonoBehaviour
//    {
//		[SerializeField] private ScoreBarsController scoreBarsController;
//		[SerializeField] private Text[] questionScoreText;
//		[SerializeField] private Text wellDoneText;
//		[SerializeField] private Text youWinText;
//		[SerializeField] private Text youLoseText;
//		[SerializeField] private Text otherPlayersTurnText;
//		[SerializeField] private FireworksManager fireworksManager;
//		[SerializeField] private Text[] actualScoreTexts;
//		[SerializeField] private GameObject[] avatars;
//		[SerializeField] private GameObject playAgainButton;
//
//
//		private QuestionManager _questionManager;
//		private int _numberOfQuestions;
//		private int[] totalCount = new int[2];
//		private float avatarBasicScale = 1.5f;
//
//		private bool[] _myAnswers;
//		private bool[] _theirAnswers;
//		private int[] _scores;
//		private AudioManager _audioManager;
//		private bool _otherPlayerIsActive = true;
//
//		private void Start()
//		{
//			_audioManager = AudioManager.GetAudioManager();
//		}
//
//		public void LevelCompleteInitialise(bool[] myAnswers, bool[] theirAnswers, int[] scores, bool otherPlayerIsActive)
//		{
//			_questionManager = QuestionManager.GetQuestionManager();
//			_numberOfQuestions = _questionManager.GetNumberOfQuestions();
//			_otherPlayerIsActive = otherPlayerIsActive;
//			Debug.Log ("LevelCompleteController start");
//			scoreBarsController.EnableScoreBarConfiguration((int)ScoreBarsController.Configurations.ChainGridTotUp);
//			scoreBarsController.ResetBars(_numberOfQuestions);
//			avatars[0].transform.localScale = new Vector3(avatarBasicScale,avatarBasicScale,1f);
//			avatars[1].transform.localScale = new Vector3(avatarBasicScale,avatarBasicScale,1f);
//			avatars[0].SendMessage("ShowNeutral",-1);
//			avatars[1].SendMessage("ShowNeutral",-1);
//			youWinText.gameObject.SetActive(false);
//			youLoseText.gameObject.SetActive(false);
//			otherPlayersTurnText.gameObject.SetActive(false);
//			actualScoreTexts[0].text = "";
//			actualScoreTexts[1].text = "";
//			playAgainButton.SetActive(false);
//			_myAnswers = myAnswers;
//			_theirAnswers = theirAnswers;
//			_scores = scores;
//
//			StartCoroutine(TotupSequence());
//		}
//
//		IEnumerator TotupSequence()
//		{
//			wellDoneText.GetComponent<Visibility>().Hide();
//
//			yield return new WaitForSeconds(1.0f);
//
//			if (_otherPlayerIsActive)
//				StartCoroutine(TotupQuestionScore(1, 0));
//			yield return StartCoroutine(TotupQuestionScore(0, 0));
//			
//			if (totalCount[0] == _numberOfQuestions)
//			{
//				yield return StartCoroutine(PerfectScoreCelebration());
//				questionScoreText[0].gameObject.GetComponent<Scaler>().ScaleToAbsolute(new Vector3(1f,1f,1f),0.1f);
//				questionScoreText[1].gameObject.GetComponent<Scaler>().ScaleToAbsolute(new Vector3(1f,1f,1f),0.1f);
//			}
//
//			yield return new WaitForSeconds(1.0f);
//
//			yield return StartCoroutine(CountUpActualScores(_scores));
//		}
//
//
//		IEnumerator PerfectScoreCelebration()
//		{
//			if (_audioManager)
//				_audioManager.PlayAudioClip("totUpPerfectScore");
//
//			fireworksManager.ManyStarBursts(16);
//			avatars[0].SendMessage("ShowHappy",-1);
//
//			Visibility visibilityComponent = wellDoneText.GetComponent<Visibility>();
//			visibilityComponent.Hide ();
//			visibilityComponent.FadeIn (0.25f);
//			Scaler scaler = wellDoneText.GetComponent<Scaler>();
//			scaler.ScaleBackFrom (0.4f, 0.5f);
//			
//			yield return new WaitForSeconds(0.25f);
//			
//			if (visibilityComponent != null)
//				visibilityComponent.Flash(20,0.05f,0.05f,false);
//
//			yield return new WaitForSeconds(2.1f);
//
//			questionScoreText[0].gameObject.GetComponent<Scaler>().ScaleToAbsolute(new Vector3(1f,1f,1f),0.2f);
//			questionScoreText[1].gameObject.GetComponent<Scaler>().ScaleToAbsolute(new Vector3(1f,1f,1f),0.2f);
//			avatars[0].SendMessage("ShowNeutral",-1);
//		}
//
//		IEnumerator CountUpActualScores(int[] scores)
//		{
//			int[] displayedScores = new int[2];
//			int highestScore, lowestScore;
//			int winner;
//			if (scores[0] > scores[1])
//			{
//				highestScore = scores[0];
//				lowestScore = scores[1];
//				winner = 0;
//			}
//			else
//			{
//				highestScore = scores[1];
//				lowestScore = scores[0];
//				winner = 1;
//			}
//
//			int loser = winner^1;
//
////			float[] scaleFactors = new float[2];
////			if (
//
//			if (_audioManager)
//				_audioManager.PlayAudioClip("totUpAvatarsGetBigger");
//
//			int biggestScore = Math.Max(scores[0], scores[1]);
//			int scoreIncrement = (biggestScore / 200)+1;
//			bool keepGoing = false;
//			do
//			{
//				keepGoing = false;
//				for (int playerNum = 0; playerNum < 2; playerNum++)
//				{
//					if (displayedScores[playerNum] < scores[playerNum])
//					{
//						keepGoing = true;
//						displayedScores[playerNum] += scoreIncrement;
//						actualScoreTexts[playerNum].text = ""+displayedScores[playerNum];
//						//Vector3 scaleVec = avatars[playerNum].transform.localScale;
//						//float s = 1.0f;
//						if (_otherPlayerIsActive)
//						{
//							float startScaleLevel = 0.0f;
//							if (displayedScores[playerNum] > startScaleLevel)
//							{
//								float maxScale = 1.8f;	//1.5f;
//								float scale = ((float)(displayedScores[playerNum] - startScaleLevel))/(highestScore - startScaleLevel);
//								// scale is between 0 and 1
//								scale = (float)Math.Pow(scale,3);	// make the growth of the avatars exponential
//								scale = 1.0f + scale*(maxScale - 1.0f);
//								Debug.Log ("CountUpActualScores playerNum="+playerNum+", highestScore = "+highestScore+", lowestScore = "+lowestScore);
//								avatars[playerNum].transform.localScale = new Vector3(scale*avatarBasicScale,scale*avatarBasicScale,1.0f);
//							}
//						}
//					}
//				}
//				yield return new WaitForSeconds(0.01f);
//			} while(keepGoing);
//
//			if (_otherPlayerIsActive)
//			{
//				avatars[loser].GetComponent<Scaler>().ScaleToAbsolute(new Vector3(avatarBasicScale*0.8f,avatarBasicScale*0.8f,1f),0.2f);
//				Visibility visibilityComponent = avatars[winner].GetComponent<Visibility>();
//				visibilityComponent.Pulse (0.8f);
//				avatars[winner].SendMessage("ShowHappy",-1);
//				avatars[loser].SendMessage("ShowSad",-1);
//				if (winner == 0)
//				{
//					fireworksManager.ManyStarBursts(5);
//					youWinText.gameObject.SetActive(true);
//					if (_audioManager)
//						_audioManager.PlayAudioClip("totUpYouWin");
//				}
//				else
//				{
//					youLoseText.gameObject.SetActive(true);
//					if (_audioManager)
//						_audioManager.PlayAudioClip("totUpYouLose");
//				}
//			}
//			else
//			{
//				otherPlayersTurnText.gameObject.SetActive(true);
//			}
//
//			playAgainButton.SetActive(true);
//		}
//
//		IEnumerator TotupQuestionScore(int playerNum, int score)
//		{
//			totalCount[playerNum] = 0;
//			for (int i=0; i<_numberOfQuestions; i++)
//			{
//				bool correct = _myAnswers[i];
//				if (playerNum == 1)
//					correct = _theirAnswers[i];	//((i&1) == 1);
//				scoreBarsController.AnsweredQuestion(playerNum, i, correct, 0);
//
//				if (correct)
//				{
//					totalCount[playerNum]++;
//				}
//				questionScoreText[playerNum].text = totalCount[playerNum]+" / "+_numberOfQuestions;
//
//
//				if (_audioManager && (playerNum == 0))
//					_audioManager.PlayAudioClip("totUpSingleQuestion");
//
//				yield return new WaitForSeconds(0.15f);
//			}
//
//			Debug.Log ("TotupCount = "+totalCount);
//			Debug.Log ("_numberOfQuestions = "+_numberOfQuestions);
//
//			float scaleAmount = 1.0f;
//			if (totalCount[playerNum] == _numberOfQuestions-1)
//				scaleAmount = 2.0f;
//			if (totalCount[playerNum] == _numberOfQuestions)
//			{
//				scaleAmount = 3.0f;
//				//questionScoreText[playerNum].gameObject.GetComponent<Visibility>().Flash (10,0.1f,0.1f,true);
//			}
//
//			if (playerNum == 0)
//			{
//				if (_audioManager)
//					_audioManager.PlayAudioClip("EndOfQuestonTotUp");
//
//				//yield return new WaitForSeconds(0.5f);
//				questionScoreText[playerNum].gameObject.GetComponent<Scaler>().ScaleTo(scaleAmount,0.1f);
//
//				//yield return new WaitForSeconds(0.1f);
//				//questionScoreText[playerNum].gameObject.GetComponent<Visibility>().Pulse(0.2f);
//				yield return new WaitForSeconds(0.3f);
//				if (totalCount[playerNum] >= _numberOfQuestions-1)
//					questionScoreText[playerNum].gameObject.GetComponent<Visibility>().SineScale(0.12f, 2.0f, 0.15f);
//				else
//					questionScoreText[playerNum].gameObject.GetComponent<Visibility>().SineScale(0.06f, 1.0f, 0.15f);
//			}
//		}
//    }
//}
