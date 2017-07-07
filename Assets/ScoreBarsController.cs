//using UnityEngine;
//using System.Collections;
//using UnityEngine.UI;
//
//namespace UnityStandardAssets._2D
//{
//	public class ScoreBarsController : MonoBehaviour {
//
//		public enum Configurations
//		{
//			None = 0,
//			SideUp,
//			SideDown,
//			TopHalfWidth,
//			TopTugOfWar,
//			ChainSidedown,
//			ChainSideup,
//			ChainHorizontal,
//			ChainHorizontalTugOfWar,
//			ChainGridTotUp
//		};
//
//		[SerializeField] private GameObject[] _scoreBarConfigurations;
//		[SerializeField] private ScoreBar[] _scoreBarsMe;
//		[SerializeField] private ScoreBar[] _scoreBarsThem;
//		[SerializeField] private ScoreChain[] _scoreChainsMe;
//		[SerializeField] private ScoreChain[] _scoreChainsThem;
//		[SerializeField] private RectTransform _imageContainer;
//		[SerializeField] private RectTransform _imageContainerContainer;
//
//		private ScoreBar _scoreBarMe;
//		private ScoreBar _scoreBarThem;
//		private ScoreChain _scoreChainMe;
//		private ScoreChain _scoreChainThem;
//		private int _configuration;
//		private bool _chainMode;
//		private Rect _questionImageContainer;
//
//		void Awake () 
//		{
//			// disable all score bars. Keeps everything neat and tidy as the game is starting.
//			for (int f=0; f<_scoreBarConfigurations.Length; f++)
//			{
//				GameObject config = _scoreBarConfigurations[f];
//				config.SetActive(false);
//			}
//		}
//
//		public void EnableScoreBarConfiguration(int type, GameObject imageContainer = null)
//		{
//			Debug.Log ("LevelCompleteController type "+type);
//
//			_configuration = type;
//			if (imageContainer != null)
//			{
//				_questionImageContainer = imageContainer.GetComponent<RectTransform>().rect;
//			}
//
//			// adjust size of image container to make way for bars
//			Vector2 vmax = _imageContainer.offsetMax;
//			Vector2 vmin = _imageContainer.offsetMin;
//			Debug.Log ("EnableScoreBarConfiguration vmax = "+vmax.x+", "+vmax.y);
//			Debug.Log ("EnableScoreBarConfiguration vmin = "+vmin.x+", "+vmin.y);
//			if (type == 1 || type == 2)		// bar graphs down the sides of the screen
//			{
//				vmax.x = -20f;
//				vmin.x = 20f;
//
//			}
//			else if (type == 5 || type == 6)				// chains down the sides of the screen
//			{
//				vmax.x = -48f;
//				vmin.x = 48f;
//			}
//			else if (type == 3 || type == 4)
//			{
//				vmax.y = -61.5f;
//			}
//			else if (type == 7)
//			{
//				vmax.y = -80.0f;
//			}
//			_imageContainer.offsetMax = vmax;
//			_imageContainer.offsetMin = vmin;
//
//			// treat type 0 as a special case for showing no score bars.
//			bool showScoreBars = true;
//			if (type == 0)
//				showScoreBars = false;
//			else
//				type = type-1;
//
//			if (type >= _scoreBarConfigurations.Length)
//				type = _scoreBarConfigurations.Length-1;
//			if (type < 0)
//				type = 0;
//
//			if (type < _scoreBarsMe.Length)
//			{
//				// continuous bar graph
//				_chainMode = false;
//				_scoreBarMe = _scoreBarsMe[type];
//				_scoreBarThem = _scoreBarsThem[type];
//				_scoreChainMe = _scoreChainThem = null;
//			}
//			else
//			{
//				// a chain of hexagons
//				int chainType = type - _scoreBarsMe.Length;
//				_chainMode = true;
//				_scoreChainMe = _scoreChainsMe[chainType];
//				_scoreChainThem = _scoreChainsThem[chainType];
//				_scoreBarMe = _scoreBarThem = null;
//			}
//
//			// disable all other score bars.
//			for (int f=0; f<_scoreBarConfigurations.Length; f++)
//			{
//				GameObject config = _scoreBarConfigurations[f];
//				config.SetActive((f == type) && showScoreBars);
//			}
//		}
//
//		void Update()
//		{
//			// force the height of the score bars to be the same as the imageContainer.
//			// sorry for doing this every frame, but it takes a while for the height of the imagecontainer to be set up properly. (i.e. can't be done in Start or Awake)
//			if (_configuration == 0 || _configuration == 1)
//			{
//				if (!_scoreBarMe)
//					return;
//
//				float height = _imageContainer.rect.height;
//
//				GameObject config = _scoreBarConfigurations[_configuration];
//				RectTransform rtMe = _scoreBarMe.gameObject.GetComponent<RectTransform>();
//				RectTransform rtThem = _scoreBarThem.gameObject.GetComponent<RectTransform>();
//				rtMe.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,height);
//				rtThem.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,height);	
//
//				Vector3[] corners = new Vector3[4];
//				_imageContainerContainer.GetWorldCorners(corners);
//				Rect rect = new Rect();
//				rect.height = corners[1].y - corners[0].y;
//				Vector2 pos = new Vector2(corners[0].x + rect.width/2, corners[0].y + rect.height/2);
//				rect.position = pos;
//				RectTransform rt = gameObject.GetComponent<RectTransform>();
//				rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, corners[0].y, rect.height);
//				rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0f, transform.parent.GetComponent<RectTransform>().rect.width);
//			}
//		}
//
//		public void ResetBars(int numQuestions)
//		{
//			if (_chainMode)
//			{
//				int numBubbles = numQuestions;
//				if (_configuration == 8)
//					numBubbles = 10;
//				_scoreChainMe.ResetChain(numBubbles, _questionImageContainer.width);
//				_scoreChainThem.ResetChain(numBubbles, _questionImageContainer.width);
//			}
//			else
//			{
//				_scoreBarMe.ResetBar(numQuestions);
//				_scoreBarThem.ResetBar(numQuestions);
//			}
//		}
//
//		public void AnsweredQuestion(int playerNum, int questionNum, bool correct, float score)
//		{
//			if (_configuration == (int)Configurations.TopTugOfWar)
//			{
//				// do tug-of-war (i.e. reduce other player's bar if the bars have met in the middle)
//				float totalBarPos = _scoreBarMe.GetBarPos() + _scoreBarThem.GetBarPos();
//				float excess = (totalBarPos + score) - 1.0f; 
//				if (excess > 0.0f)
//				{
//					switch(playerNum)
//					{
//					case 0:
//						_scoreBarThem.MoveBarBack(-excess);
//						break;
//					case 1:
//						_scoreBarMe.MoveBarBack(-excess);
//						break;
//					}
//				}
//			}
//
//			switch(playerNum)
//			{
//			case 0:
//				if (_chainMode)
//					_scoreChainMe.AnsweredQuestion(questionNum, correct, score);
//				else
//					_scoreBarMe.AnsweredQuestion(questionNum, correct, score);
//				break;
//			case 1:
//				if (_chainMode)
//					_scoreChainThem.AnsweredQuestion(questionNum, correct, score);
//				else
//					_scoreBarThem.AnsweredQuestion(questionNum, correct, score);
//				break;
//			}
//		}
//	}
//}
