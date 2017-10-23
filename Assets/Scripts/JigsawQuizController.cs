using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

using Prime31;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityStandardAssets._2D
{
	[System.Serializable]
	public class JigsawQuizSettings : GameplaySettings
	{
		// gameplay parameters
		[Header("gameplay settings")]
		[SerializeField] public int bombLivesShakeStart = 2;
		[SerializeField] public bool useBombsAndHiddenSquares = true;
		[SerializeField] public bool showCorrectTiles = true;
		[SerializeField] public bool moveLumpsOfTiles = true;
		[SerializeField] public bool timerUsed = true;
		[SerializeField] public bool suddenDeath = false;
		[SerializeField] public bool tryAgain = false;
		[SerializeField] public bool infiniteQuestions = false;
		[SerializeField] public bool loseQuestionOnTimerExpire = true;
		[SerializeField] public bool resetTimerOnWrongAnswer = true;
		[SerializeField] public float timeReductionOnCorrectAnswer = 0.9f;
		[SerializeField] public float timerMinimumTime = 0.5f;
		[SerializeField] public float timerMaximumTime = 3.0f;
		[SerializeField] public float bonusForTimeLeft = 50.0f;
		[SerializeField] public int baseScoreForCorrectAnswer = 1;
		[SerializeField] public int maxBonus = 100;
		[SerializeField] public float bonusIncreaseRate = 1.2f;
		[SerializeField] public float minimumHotspotSize = 120.0f;
		[SerializeField] public bool showQuestionAtEndOfMovie = false;
		[SerializeField] public bool randomiseQuestions = true;
		[SerializeField] public int gridX = 0;
		[SerializeField] public int gridY = 0;
	}

	public class JigsawQuizController : MonoBehaviour
    {
		[SerializeField] private RawImage pictureImageUI;
		[SerializeField] private RawImage pictureImage2UI;
		[SerializeField] private GameObject picture1ImageBox;
		[SerializeField] private GameObject picture2ImageBox;
		[SerializeField] private RawImage preScreenImageUI;
		//[SerializeField] private AspectRatioFitter pictureAspectRatioFitterUI;
		[SerializeField] private Text questionTextUI;
		[SerializeField] private Text preScreenTextUI;
		[SerializeField] private Text scoreTextUI;
		[SerializeField] private Text bigWrongText;
		[SerializeField] private Text tryAgainText;
		[SerializeField] private Text bigOutOfTimeText;
		[SerializeField] private Text rightText;
		[SerializeField] private Text bonusText;
		[SerializeField] private Text bonusTextText;
		[SerializeField] private Image bonusTextBackPanel;
		[SerializeField] private Text timeTakenText;
		[SerializeField] private Text timeTakenTextText;
		[SerializeField] private Image timeTakenBackPanel;
		[SerializeField] private Text movesText;
		[SerializeField] private Text movesTextText;
		[SerializeField] private Image movesTextBackPanel;
		[SerializeField] private Text bestPossibleText;
		[SerializeField] private Text bestPossibleTextText;
		[SerializeField] private Image bestPossibleBackPanel;
		[SerializeField] private Text loadingText;
		[SerializeField] private GameObject timerBar;
		[SerializeField] private GameObject gameplayPanel;
		[SerializeField] private GameObject gameOverPanel;
		[SerializeField] private GameObject preScreenPanel;
		[SerializeField] private GameObject levelCompletePanel;
//		[SerializeField] private Button hotspotButtonUI;
//		[SerializeField] private Button backgroundButtonUI;
//		[SerializeField] private GameObject hotspotHighlighterUI;
//		[SerializeField] private Button hotspotButton2UI;
//		[SerializeField] private Button backgroundButton2UI;
//		[SerializeField] private GameObject hotspotHighlighter2UI;
		[SerializeField] private Text numCorrectAnswersTextUI;
		[SerializeField] private Text wellDoneText;

		// jigsaw stuff
		[SerializeField] private GameObject obscuringColumnPrefab;
		[SerializeField] private GameObject obscuringSquarePrefab;
		[SerializeField] private GameObject carriedJigsawPiece;
		[SerializeField] private GameObject carriedJigsawShadow;
		[SerializeField] private GameObject swappedJigsawPiece;
		[SerializeField] private GameObject swappedJigsawShadow;

		[SerializeField] private GameObject allCarriedJigsawPieces;
		[SerializeField] private GameObject allSwappedJigsawPieces;
		[SerializeField] private GameObject carriedJigsawPiecePrefab;

		[SerializeField] private GlintLayer glintLayer;
		[SerializeField] private GameObject whiteLayer;
		[SerializeField] private Text minimumMoves;
		[SerializeField] private Text moveCount;
		[SerializeField] private EncouragementManager encouragementManager;
		[SerializeField] private FireworksManager fireworksManager;
		[SerializeField] private Texture hiddenPieceTexture;
		[SerializeField] private Texture bombTexture;
		[SerializeField] private GameObject puffOfSmokePrefab;
		[SerializeField] private float tileMovementTime = 0.25f;


		
		public enum GameStates
		{
			Waiting = 0,
			Dragging,
			Dropping,
			TotUp
		}

		public enum TileType
		{
			Normal = 0,
			Hidden,
			Bomb
		}
		
		public struct TileData
		{
			public int moveCounter;
			public TileType type;
			public float shakeMagnitude;
		}

		public struct CarriedTileData
		{
			public IntVec2 originalPosition;
			public IntVec2 carriedOffset;	// (0,0) if the tile I clicked on, (1,0) if it is one to the right, etc. etc.
			public GameObject carriedTile;
		}

		private QuestionManager _questionManager;
		private PictureManager _pictureManager;
		private AudioManager _audioManager;
		private SettingsManager _settingsManager;
		private JigsawQuizSettings gameplaySettings;

		private int _currentQuestionNumber = 0;
		private int _currentQuestionCorrectAnswer = 0;
		private int _score = 0;
		private float _timerTime = 0;
		private int _bonusScore = 1;
		private int _questionCount = 0;
		private int _correctAnswersCount = 0;
		private bool _playingAMovie = false;
		private int _numberOfPictures = 1;
//		private Prime31.VideoTexture videoTexture;
		private Texture _imageTexture = null;
		private int gridXDim, gridYDim;
		private Vector2 jigsawCellSize;

		private int[,] _jigsawOrderArray;
		private int[,] _duplicateJigsawOrderArray;		// used to double buffer when moving lumps of tiles
		private GameObject[,] _jigsawSpritesArray;
		private TileData[] _tilesData;
		private IntVec2 _carriedPiecePosition;		// original position of the piece you are carrying
		private List<CarriedTileData> _carriedPieces;
		private Transform carriedJigsawPieceParent;
		private Transform swappedJigsawPieceParent;
		private GameStates gameState;
		private IntVec2 droppedPiecePosition;
		private int _moveCount = 0;
		private int _requiredMoves;
		private bool inputEnabled = false;
		private int _numberOfHiddenTiles = 0;


		// Use this for initialization
		private void Start()
		{
			// go directly to the menu if the game has been run from the game scene in editor.
			if (!SettingsManager.SettingsManagerExists())
				Application.LoadLevel("menu");

			_questionManager = QuestionManager.GetQuestionManager();
			_audioManager = AudioManager.GetAudioManager();
			_settingsManager = SettingsManager.GetSettingsManager();
			_pictureManager = gameObject.GetComponent<PictureManager>();
			gameplaySettings = _settingsManager.jigsawSettings;
			_carriedPieces = new List<CarriedTileData>();

			// set up everything for the 'loading questions' state.
			EnableInput(false);
			loadingText.enabled = true;
			pictureImageUI.enabled = false;
			//pictureImage2UI.enabled = false;
			//picture2ImageBox.SetActive(false);

			// kick off the loading of the textures
			CallBack finishedLoadingTexturesCallback = FinishedLoadingTextures;
			//_questionManager.LoadQuestions();
			string[] pictureNames = _questionManager.AllPictureNames();
			string pictureSubDirectory = _questionManager.PictureSubDirectory();
			_pictureManager.LoadAllSpecifiedTextures(pictureNames, pictureSubDirectory, finishedLoadingTexturesCallback);

			gameplayPanel.SetActive(false);
			gameOverPanel.SetActive(false);
			preScreenPanel.SetActive(false);
			levelCompletePanel.SetActive(false);

			carriedJigsawPieceParent = carriedJigsawPiece.transform.parent;
			swappedJigsawPieceParent = swappedJigsawPiece.transform.parent;
			gameState = GameStates.Waiting;

			timeTakenBackPanel.gameObject.GetComponent<Visibility>().SetInitialAlpha(100f/255);
			movesTextBackPanel.gameObject.GetComponent<Visibility>().SetInitialAlpha(100f/255);
			bestPossibleBackPanel.gameObject.GetComponent<Visibility>().SetInitialAlpha(100f/255);
			bonusTextBackPanel.gameObject.GetComponent<Visibility>().SetInitialAlpha(100f/255);
		}

		public void StartNewGame()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");

			// grab any gameplay settings defined in the json file for this quiz.
			_settingsManager.ResetSettings();
			_questionManager.AdjustSettings(gameplaySettings);

//			if (gameplaySettings.randomiseQuestions)
//				_questionManager.RandomiseQuestions();
			gameplayPanel.SetActive(true);
			preScreenPanel.SetActive(false);
			gameOverPanel.SetActive(false);
			levelCompletePanel.SetActive(false);

			_score = 0;
			_bonusScore = 1;
			_currentQuestionNumber = _questionManager.GetFirstQuestionNum();
			_timerTime = gameplaySettings.timerMaximumTime;
			_questionCount = 0;
			_correctAnswersCount = 0;

			bigWrongText.gameObject.SendMessage("Hide");
			tryAgainText.gameObject.SendMessage("Hide");
			bigOutOfTimeText.gameObject.SendMessage("Hide");
			rightText.gameObject.SendMessage("Hide");
			bonusText.gameObject.SendMessage("Hide");
			bonusTextText.gameObject.SendMessage("Hide");
			bonusTextBackPanel.gameObject.SendMessage("Hide");
			timeTakenBackPanel.gameObject.SendMessage("Hide");
			timeTakenText.gameObject.SendMessage("Hide");
			timeTakenTextText.gameObject.SendMessage("Hide");
			movesText.gameObject.SendMessage("Hide");
			movesTextText.gameObject.SendMessage("Hide");
			movesTextBackPanel.gameObject.SendMessage("Hide");
			bestPossibleText.gameObject.SendMessage("Hide");
			bestPossibleTextText.gameObject.SendMessage("Hide");
			bestPossibleBackPanel.gameObject.SendMessage("Hide");
			scoreTextUI.text = _score.ToString();

			if (_questionManager.IsPreScreen())
			{
				ShowPreScreen();
			}
			else
			{
				LaunchIntoGame();
			}
		}

		private void ShowPreScreen()
		{
			preScreenPanel.SetActive(true);
			gameplayPanel.SetActive(false);

			PreScreen ps = _questionManager.GetPreScreen();
			preScreenTextUI.text = ps.text;

			string pictureFileName = ps.pictureFileName;

			float delayBeforeQuestion = DisplayPicture(pictureFileName, preScreenImageUI);
		}

		public void ClickedLetsGo()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressPlay");
			LaunchIntoGame();
		}

		public void LaunchIntoGame()
		{
			// actually start the game
			gameplayPanel.SetActive(true);
			preScreenPanel.SetActive(false);
			
			if (timerBar)
				timerBar.SetActive (gameplaySettings.timerUsed);
			
			loadingText.enabled = false;

			SetQuestion (_currentQuestionNumber);
			pictureImageUI.enabled = true;
			if (_numberOfPictures > 1)
			{
				pictureImage2UI.enabled = true;		
				picture2ImageBox.SetActive(true);
			}
		}

		private void FinishedLoadingTextures()
		{
			StartNewGame ();
		}

		private void EnableInput(bool enable)
		{
			inputEnabled = enable;

//			hotspotButtonUI.interactable = enable;
//			backgroundButtonUI.interactable = enable;
//			button1UI.interactable = enable;
//			button2UI.interactable = enable;
		}

		public void ClickedCorrectPlace ()
		{
			Debug.Log ("pressed correct");
			Answered(true);
		}
		
		public void ClickedWrongPlace ()
		{
			Debug.Log ("pressed wrong");
			Answered(false);
		}

		public void ClickedBack ()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("pressedBack");
			Application.LoadLevel("jigsaw level select");
		}

		private void ShowNextQuestion()
		{
			int numberOfQuestions = _questionManager.GetNumberOfQuestions();
			_currentQuestionNumber++;
			if (_currentQuestionNumber >= numberOfQuestions)
			{
				if (gameplaySettings.infiniteQuestions)
				{
					// re-shuffle the questions before looping back.
					_questionManager.RandomiseQuestions();
					_currentQuestionNumber = 0;
				}
				else
				{
					AllQuestionsFinished();
					return;
				}
			}

			SetQuestion (_currentQuestionNumber);
		}

//		private void SetupHotspots(Question q)
//		{
//			SetupSingleHotspot(q,pictureImageUI,hotspotButtonUI,hotspotHighlighterUI);
//			if (_numberOfPictures > 1)
//				SetupSingleHotspot(q,pictureImage2UI,hotspotButton2UI,hotspotHighlighter2UI);
//		}

//		private void SetupSingleHotspot(Question q, RawImage image, Button button, GameObject highlighter)
//		{
//			RectTransform rt2 = image.GetComponent<RectTransform>();
//			//Texture tex = pictureImageUI.GetComponent<RawImage>().texture;
//			Texture tex = image.texture;
//			float scaleFactor = rt2.rect.width / tex.width;
//			Vector2 pos = new Vector2(q.tapLocation.position.x * scaleFactor, -q.tapLocation.position.y * scaleFactor);
//			Vector2 size = new Vector2(q.tapLocation.width * scaleFactor, q.tapLocation.height * scaleFactor);
//			_hotspotCentrePosition.x = pos.x+(size.x/2);
//			_hotspotCentrePosition.y = pos.y-(size.y/2);
//
//			if (size.x < gameplaySettings.minimumHotspotSize)
//			{
//				pos.x -= (gameplaySettings.minimumHotspotSize - size.x)/2;
//				size.x = gameplaySettings.minimumHotspotSize;
//			}
//			if (size.y < gameplaySettings.minimumHotspotSize)
//			{
//				pos.y += (gameplaySettings.minimumHotspotSize - size.y)/2;
//				size.y = gameplaySettings.minimumHotspotSize;
//			}
//			RectTransform rt = button.GetComponent<RectTransform>();
//			rt.anchoredPosition = pos;
//			rt.sizeDelta = size;
//			
//			// put the highlighter at the same place - ready for when the player gets it wrong.
//			rt = highlighter.GetComponent<RectTransform>();
//			rt.anchoredPosition = pos;
//			rt.sizeDelta = size;
//			
//			
//			highlighter.SendMessage("Hide");
//		}


		private void SetQuestion(int questionNumber)
		{
			Debug.Log ("questionNumber = "+questionNumber);

			EnableInput(false);

			_moveCount = 0;
			moveCount.text = _moveCount.ToString();

			Question q = _questionManager.GetQuestion(questionNumber);
			//_currentQuestionCorrectAnswer = q.correctAnswer;
			questionTextUI.text = "";
//			hotspotHighlighterUI.SendMessage("Hide");

			string pictureFileName = q.pictureFileName;
			float delayBeforeQuestion = DisplayPicture(pictureFileName, pictureImageUI, q.picWidth, q.picHeight);
			StartCoroutine(DisplayQuestionAfterDelay(questionNumber, delayBeforeQuestion-0.4f));

			GenerateSquareJigsawPieces(questionNumber, _imageTexture);
			whiteLayer.GetComponent<Visibility>().Hide();

			if (q.pictureFileName2 != null)
			{
				DisplayPicture(q.pictureFileName2, pictureImage2UI, q.picWidth, q.picHeight);
				_numberOfPictures = 2;
			}
			else
			{
				_numberOfPictures = 1;
			}

//			pictureImageUI.rectTransform.localScale = new Vector3(1f,1f);
//			pictureImageUI.rectTransform.anchoredPosition = new Vector2(0f,0f);
//			pictureImage2UI.rectTransform.localScale = new Vector3(1f,1f);
//			pictureImage2UI.rectTransform.anchoredPosition = new Vector2(0f,0f);
		}

		private float DisplayPicture(string pictureFileName, RawImage rawImage, int width = 0, int height = 0)
		{
			float aspectRatio = 1.0f;
			float delayBeforeQuestion = 0.0f;
			_playingAMovie = false;

			#if UNITY_EDITOR
			if (EditorApplication.isPlaying)
			{
				string last4chars = 4 > pictureFileName.Length ? pictureFileName : pictureFileName.Substring(pictureFileName.Length -4);
				if ((last4chars.ToUpper() == ".MOV") || (last4chars.ToUpper() == ".M4V"))
				{
					_playingAMovie = true;
					
					string beforeLast4chars = pictureFileName.Substring(0,pictureFileName.Length-4);

					string path = _questionManager.PictureSubDirectory();
					string audiopath = path;
					
					if (path != "") 
					{
						path += "/";
						audiopath += "_audio/";
					}
					string fullMovieName = path + beforeLast4chars;
					Debug.Log ("Movie fullName = "+fullMovieName);
					string fullAudioName = audiopath + beforeLast4chars;	//+"_audio";
					Debug.Log ("Audio fullName = "+fullAudioName);
					
					MovieTexture movie = Resources.Load(fullMovieName) as MovieTexture;
					AudioClip audioClipMP3 = Resources.Load(fullAudioName) as AudioClip;

					pictureImageUI.texture = movie;
					AudioSource aud = GetComponent<AudioSource>();
					//aud.clip = movie.audioClip;
					aud.clip = audioClipMP3;
					movie.loop = !gameplaySettings.showQuestionAtEndOfMovie;
					movie.Stop();
					movie.Play();
					aud.Play();
					aspectRatio = (float)movie.width / movie.height;
					if (gameplaySettings.showQuestionAtEndOfMovie)
						delayBeforeQuestion = movie.duration;
					Debug.Log ("Movie duration = "+delayBeforeQuestion);
					Debug.Log ("Movie width = "+movie.width+", height = "+movie.height);
				}
			}
			else
				#endif
			{
				string last4chars = 4 > pictureFileName.Length ? pictureFileName : pictureFileName.Substring(pictureFileName.Length -4);
//				if ((last4chars.ToUpper() == ".MOV") || (last4chars.ToUpper() == ".M4V"))
//				{
//					_playingAMovie = true;
//					
//					string beforeLast4chars = pictureFileName.Substring(0,pictureFileName.Length-4);
//					string path = _questionManager.PictureSubDirectory();
//					string audiopath = path;
//					
//					if (path != "") 
//					{
//						path += "/";
//						audiopath += "_audio/";
//					}
//					string fullMovieName = path + pictureFileName;
//					string fullAudioName = audiopath + beforeLast4chars;	//+"_audio";
//
//					videoTexture = new VideoTexture( fullMovieName, width, height, !gameplaySettings.showQuestionAtEndOfMovie );
//					AudioClip audioClipMP3 = Resources.Load(fullAudioName) as AudioClip;
//
//					pictureImageUI.texture = videoTexture.texture;
//					pictureImageUI.uvRect = new Rect(0,1,1,-1);
//					AudioSource aud = GetComponent<AudioSource>();
//					aud.clip = audioClipMP3;
//					videoTexture.syncAudioSource( aud );
//
//					//movie.Stop();
//					//movie.Play();
//					aud.Play();
//					aspectRatio = (float)videoTexture.texture.width / videoTexture.texture.height;
//					if (gameplaySettings.showQuestionAtEndOfMovie)
//						delayBeforeQuestion = audioClipMP3.length;
//					Debug.Log ("Movie duration = "+delayBeforeQuestion);
//					Debug.Log ("Movie width = "+videoTexture.texture.width+", height = "+videoTexture.texture.height);
//				}
			}
			
			if (!_playingAMovie)
			{
				Debug.Log ("Picture : "+pictureFileName);
				Texture texture = _pictureManager.GetTextureByName(pictureFileName);
				rawImage.texture = texture;
				aspectRatio = (float)texture.width / texture.height;
				_imageTexture = texture;
			}
			
			AspectRatioFitter pictureAspectRatioFitterUI = rawImage.gameObject.GetComponent<AspectRatioFitter>();
			pictureAspectRatioFitterUI.aspectRatio = aspectRatio;

			return delayBeforeQuestion;
		}


		IEnumerator DisplayQuestionAfterDelay(int questionNumber, float delayTime) 
		{
			yield return new WaitForSeconds(delayTime);
//			if (_playingAMovie && gameplaySettings.showQuestionAtEndOfMovie)
//			{
//				#if UNITY_EDITOR
//				if (EditorApplication.isPlaying)
//				{
//					MovieTexture movie = (MovieTexture)pictureImageUI.texture;
//					movie.Pause();
//				}
//				else
//				#endif
//				{
//					videoTexture.pause();
//				}
//			}
			DisplayQuestion(questionNumber);
		}

		private void DisplayQuestion(int questionNumber)
		{
			// bodge to move the question text up a bit if we are playing a video
			float yOffset = 0;
			if (_playingAMovie)
				yOffset = 50;
			Vector2 pos = questionTextUI.rectTransform.anchoredPosition;
			pos.y = yOffset;
			questionTextUI.rectTransform.anchoredPosition = pos;

			Question q = _questionManager.GetQuestion(questionNumber);
			questionTextUI.text = q.questionText;

			//SetupHotspots(q);

			if (gameplaySettings.timerUsed && timerBar)
			{
				timerBar.SendMessage("SetTimerTime", _timerTime);
				timerBar.SendMessage("SetCountdownSounds", false);
				timerBar.SendMessage("SetPausedVisibleAndInternal", false);
			}

			EnableInput(true);
		}

		private void Answered(bool correct)
		{
			if (gameplaySettings.timerUsed && timerBar)
				timerBar.SendMessage("SetPausedVisibleAndInternal", true);

			if (correct)
			{
                Debug.Log("COREECT!!");
                PlayerProgress playerProgress = PlayerProgress.GetPlayerProgress();
                playerProgress.SetQuestionState(_questionManager.GetQuizNum(), _currentQuestionNumber, true);

                if ((gameplaySettings.bonusForTimeLeft > 0.0f) && gameplaySettings.timerUsed)
					StartCoroutine(RightAnswerSequenceTimeBonus());
				else
					StartCoroutine(RightAnswerSequence());

				_correctAnswersCount++;

                //ZoomIntoHotSpot();
            }
			else
			{
				StartCoroutine(WrongAnswerSequence(false));
			}
		}

//		private void ZoomIntoHotSpot()
//		{
//			RectTransform rt2 = pictureImageUI.GetComponent<RectTransform>();
//
//			float zoomCentreYPos = 0.45f;		// % of screen height, up from the bottom
//			Vector2 imageCentre = new Vector2(rt2.rect.width/2, rt2.rect.height*zoomCentreYPos);
//
//			Vector2 offset = imageCentre - _hotspotCentrePosition;
//			offset.y -= rt2.rect.height;
//
//			float finalScale = 2.0f;
//			float zoomTime = 1.0f;
//			Movement movementComponent = pictureImageUI.GetComponent<Movement>();
//			ScaleMovement scaleMovementComponent = pictureImageUI.GetComponent<ScaleMovement>();
//			movementComponent.MoveTo(offset*finalScale, zoomTime);
//			scaleMovementComponent.ScaleTo(new Vector2(finalScale, finalScale), zoomTime);
//
//			if (_numberOfPictures > 1)
//			{
//				movementComponent = pictureImage2UI.GetComponent<Movement>();
//				scaleMovementComponent = pictureImage2UI.GetComponent<ScaleMovement>();
//				movementComponent.MoveTo(offset*finalScale, zoomTime);
//				scaleMovementComponent.ScaleTo(new Vector2(finalScale, finalScale), zoomTime);
//			}
//		}

//		private void FlashHighlighters(float flashTime, float singleFlashTime = 0.24f)
//		{
//			FlashHighlighter(hotspotHighlighterUI, flashTime, singleFlashTime);
//			if (_numberOfPictures > 1)
//				FlashHighlighter(hotspotHighlighter2UI, flashTime, singleFlashTime);
//		}
//
//		private void FlashHighlighter(GameObject highlighter, float flashTime, float singleFlashTime = 0.24f)
//		{
//			if (singleFlashTime > flashTime)
//			{
//				highlighter.GetComponent<Visibility>().ShowMomentarily (singleFlashTime);
//			}
//			else
//			{
//				int numFlashes = (int)(flashTime/singleFlashTime);
//				highlighter.GetComponent<Visibility>().Flash (numFlashes,singleFlashTime/2,singleFlashTime/2,false);
//			}
//		}

		IEnumerator WrongAnswerSequence(bool outOfTime) 
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("wrong");

			Debug.Log ("WrongAnswerSequence ACTIVE");
			EnableInput(false);
			if (outOfTime)
			{
				bigOutOfTimeText.gameObject.SendMessage("ShowMomentarily", 1.5f);
				//bigOutOfTimeText.gameObject.SendMessage("Pulse", 0.2f);
			}
			else
			{
				bigWrongText.gameObject.SendMessage("ShowMomentarily", 1.5f);
			}

			yield return new WaitForSeconds(0.2f);

			//FlashHighlighters(1.2f,100.0f);

			if (gameplaySettings.tryAgain)
				AddToScoreAndFlash(-20);

			yield return new WaitForSeconds(1.5f);
			EnableInput(true);

			if (gameplaySettings.resetTimerOnWrongAnswer)
				_timerTime = gameplaySettings.timerMaximumTime;

			_bonusScore = 1;

			if (gameplaySettings.suddenDeath)
				GameOver();
			else if (gameplaySettings.tryAgain)
				StartCoroutine(TryAgainSequence());
			else
				ShowNextQuestion();
		}
		
		
		IEnumerator TryAgainSequence() 
		{
			Debug.Log ("TryAgainSequence ACTIVE");

			Visibility visibilityComponentTryAgainText = tryAgainText.GetComponent<Visibility>();
			if (visibilityComponentTryAgainText != null)
				visibilityComponentTryAgainText.Flash(3,0.2f,0.2f,false);

			yield return new WaitForSeconds(1.7f);
			
			EnableInput(true);	
			SetQuestion (_currentQuestionNumber);
		}

		private void ProgressiveBeep()
		{
			if (_audioManager)
			{
				float pitch = 1.0f;
				if (gameplaySettings.timeReductionOnCorrectAnswer < 1.0f)
					pitch += (_questionCount / 3)*0.03f;		// beep gets higher as you progress
				_audioManager.PlayAudioClip("beep",pitch);
			}
		}

		private void AdjustQuestionTimeAfterCorrectAnswer()
		{
			_timerTime = gameplaySettings.timerMinimumTime + (_timerTime - gameplaySettings.timerMinimumTime)*gameplaySettings.timeReductionOnCorrectAnswer;
		}

		IEnumerator RightAnswerSequence() 
		{
			ProgressiveBeep();

			// sequence gradually gets faster the more questions you get right.
			float maxSequenceTime = 0.6f;
			float minSequenceTime = 0.2f;
			float sequenceTime = minSequenceTime + (maxSequenceTime - minSequenceTime) * ((float)Math.Pow (gameplaySettings.timeReductionOnCorrectAnswer,_questionCount));

			float pulseSize = 1.2f;
			if (gameplaySettings.timeReductionOnCorrectAnswer < 1.0f)
				pulseSize += (0.5f * (int)(_questionCount / 5));

			Debug.Log ("RightgAnswerSequence ACTIVE");
			EnableInput(false);
			rightText.gameObject.SendMessage("ShowMomentarily", sequenceTime*0.85f);
			rightText.gameObject.SendMessage("Pulse", pulseSize);

			Visibility visibilityComponentBonusText = bonusText.GetComponent<Visibility>();
			if (visibilityComponentBonusText != null)
				visibilityComponentBonusText.Flash(4,0.05f,0.05f,false);
			Visibility visibilityComponentBackPanel = bonusTextBackPanel.GetComponent<Visibility>();
			if (visibilityComponentBackPanel != null)
				visibilityComponentBackPanel.Flash(4,0.05f,0.05f,false);
			_bonusScore = (int)Math.Pow(gameplaySettings.bonusIncreaseRate, _questionCount) - 1 + gameplaySettings.baseScoreForCorrectAnswer;
			_bonusScore = Mathf.Clamp (_bonusScore, 0, gameplaySettings.maxBonus);
			bonusText.text = "+"+_bonusScore.ToString();

			//FlashHighlighters(0.7f);

			yield return new WaitForSeconds(sequenceTime/2);

			AddToScoreAndFlash(_bonusScore);
			_questionCount++;

			if (_audioManager)
				_audioManager.PlayAudioClip("addBonus");

			yield return new WaitForSeconds(1.1f - sequenceTime/2);

			AdjustQuestionTimeAfterCorrectAnswer();

			EnableInput(true);
			ShowNextQuestion();
		}

		void AddToScoreAndFlash(int points)
		{
			_score = _score + points;
			scoreTextUI.text = _score.ToString();
			Visibility visibilityComponentScoreText = scoreTextUI.GetComponent<Visibility>();
			if (visibilityComponentScoreText != null)
				visibilityComponentScoreText.Flash(3,0.1f,0.1f,true);
		}

		// the right answer sequence for when your bonus is related to how quickly you answer
		IEnumerator RightAnswerSequenceTimeBonus() 
		{
			gameState = GameStates.TotUp;

			ProgressiveBeep();

			if (_audioManager)
				_audioManager.PlayAudioClip("jigsawComplete");

			// sequence gradually gets faster the more questions you get right.
			float maxSequenceTime = 0.6f;
			float minSequenceTime = 0.2f;
			float sequenceTime = minSequenceTime + (maxSequenceTime - minSequenceTime) * ((float)Math.Pow (gameplaySettings.timeReductionOnCorrectAnswer,_questionCount));
			
			float pulseSize = 1.2f;
			if (gameplaySettings.timeReductionOnCorrectAnswer < 1.0f)
				pulseSize += (0.5f * (int)(_questionCount / 5));
			
			EnableInput(false);


			// pulse to white - clearing any hidden tiles
			whiteLayer.GetComponent<Visibility>().PulseFade (0.3f, 0.5f, 0f, 1.0f);
			yield return new WaitForSeconds(0.1f);
			ClearHiddenTiles();
			Debug.Log ("RightAnswerSequenceTimeBonus RefreshJigsawPieces false");
			RefreshJigsawPieces(false);
			yield return new WaitForSeconds(0.5f);


			// slide the shiny glint texture across the completed jigsaw
			glintLayer.DoGlint ();
			if (_audioManager)
				_audioManager.PlayAudioClip("jigsawGlint");
			yield return new WaitForSeconds(1.0f);

			const float fadeInTime = 0.2f;
			Scaler scaler = rightText.gameObject.GetComponent<Scaler>();
			scaler.ScaleBackFrom (2.0f, fadeInTime, true);
			rightText.gameObject.SendMessage("FadeIn", fadeInTime*1.2);
			if (_audioManager)
				_audioManager.PlayAudioClip("jigsawCompleteText");

//			rightText.gameObject.SendMessage("ShowMomentarily", sequenceTime*0.85f);
//			rightText.gameObject.SendMessage("Pulse", pulseSize);

			// bonus tot-up


//			bonusTextBackPanel.transform.localPosition = new Vector3(1000,65,0);
//			Movement movementComponentBackPanel = bonusTextBackPanel.GetComponent<Movement>();
//			if (movementComponentBackPanel != null)
//				movementComponentBackPanel.MoveTo(new Vector2(0,65), 0.2f);

			yield return new WaitForSeconds(0.5f);

			// time taken
			timeTakenText.gameObject.SendMessage("Show");
			timeTakenTextText.gameObject.SendMessage("Show");
			timeTakenBackPanel.gameObject.SendMessage("Show");
			timeTakenText.text = "";
			timeTakenBackPanel.transform.localPosition = new Vector3(-1000,42,0);
			Movement movementComponentBackPanel = timeTakenBackPanel.GetComponent<Movement>();
			if (movementComponentBackPanel != null)
				movementComponentBackPanel.MoveTo(new Vector2(0,42), 0.2f);
			if (_audioManager)
				_audioManager.PlayAudioClip("UISlidesOn");

			yield return new WaitForSeconds(0.5f);
			int score = (int)timerBar.GetComponent<TimerBar>().GetTimeUsed();
			timeTakenText.text = score.ToString()+"s ";
			timeTakenText.SendMessage ("Pulse", 0.25f);
			if (_audioManager)
				_audioManager.PlayAudioClip("totupJigsawTime");

//			int preBonusScore = _score;
//			_bonusScore = 0;
//			bonusText.text = "+"+_bonusScore.ToString();
			yield return new WaitForSeconds(0.7f);





			// time bonus

			bonusText.gameObject.SendMessage("Show");
			bonusTextBackPanel.gameObject.SendMessage("Show");
			bonusTextText.gameObject.SendMessage("Show");

			bonusTextBackPanel.transform.localPosition = new Vector3(1000,-66,0);
			movementComponentBackPanel = bonusTextBackPanel.GetComponent<Movement>();
			if (movementComponentBackPanel != null)
				movementComponentBackPanel.MoveTo(new Vector2(0,-66), 0.2f);
			if (_audioManager)
				_audioManager.PlayAudioClip("UISlidesOn");

			int preBonusScore = _score;
			_bonusScore = 0;
			bonusText.text = "+"+_bonusScore.ToString();
			yield return new WaitForSeconds(0.3f);


			if (_audioManager)
				_audioManager.PlayAudioClip("totupTimeLeft");
			int frameRate = 50;
			float totupTime = 0.7f;
			float bonus = 0.0f;
			if (timerBar)
			{
				bonus = gameplaySettings.bonusForTimeLeft * timerBar.GetComponent<TimerBar>().GetFractionOfTimeLeft();
			}
			float totupStartTime = Time.realtimeSinceStartup;
			timerBar.SendMessage("ReduceToZeroInGivenTime", totupTime);
			timerBar.SendMessage("SetPaused", false);
			while (_bonusScore < (int)bonus)
			{
				float timePassed = Time.realtimeSinceStartup - totupStartTime;
				var bonusScoreFloat = (bonus * timePassed) / totupTime;

				//Debug.Log ("time passed = "+timePassed+", scoreInc = "+scoreInc+", bonusScore = "+bonusScoreFloat);
				if (bonusScoreFloat > bonus) bonusScoreFloat = bonus;
				_bonusScore = (int)bonusScoreFloat;
				bonusText.text = "+"+_bonusScore.ToString();
				_score = preBonusScore + _bonusScore;
				scoreTextUI.text = _score.ToString();
				yield return new WaitForSeconds(1.0f/frameRate);
			}


			_questionCount++;
			Visibility visibilityComponent = scoreTextUI.GetComponent<Visibility>();
			if (visibilityComponent != null)
				visibilityComponent.Flash(3,0.1f,0.1f,true);

			if (_audioManager)
				_audioManager.PlayAudioClip("addBonus");
			
			yield return new WaitForSeconds(sequenceTime/2);

			bonusText.gameObject.SendMessage("Hide");
			bonusTextBackPanel.gameObject.SendMessage("Hide");
			bonusTextText.gameObject.SendMessage("Hide");




			// number of moves taken
			movesTextText.gameObject.SendMessage("Show");
			movesTextBackPanel.gameObject.SendMessage("Show");
			yield return new WaitForSeconds(0.2f);
			movesText.text = _moveCount.ToString();
			//movesText.gameObject.SendMessage("Show");
			const float movesTextFadeInTime = 0.2f;
			scaler = movesText.gameObject.GetComponent<Scaler>();
			scaler.ScaleBackFrom (2.0f, movesTextFadeInTime);
			movesText.gameObject.SendMessage("FadeIn", movesTextFadeInTime*1.2);
			for (int f=0; f<=_moveCount; f++)
			{
				movesText.text = f.ToString();
				if (_audioManager)
					_audioManager.PlayAudioClip("TotupJigsawMoves");
				yield return new WaitForSeconds(0.1f);
			}
			//			if (_audioManager)
			//				_audioManager.PlayAudioClip("removeSquare");
			yield return new WaitForSeconds(0.4f);
			
			
			// best possible moves
			bestPossibleTextText.gameObject.SendMessage("Show");
			bestPossibleBackPanel.gameObject.SendMessage("Show");
			yield return new WaitForSeconds(0.2f);
			bestPossibleText.text = _requiredMoves.ToString();
			const float bestTextFadeInTime = 0.2f;
			scaler = bestPossibleText.gameObject.GetComponent<Scaler>();
			scaler.ScaleBackFrom (2.0f, bestTextFadeInTime, true);
			bestPossibleText.gameObject.SendMessage("FadeIn", bestTextFadeInTime*1.2);
			if (_audioManager)
				_audioManager.PlayAudioClip("removeSquare");
			yield return new WaitForSeconds(0.4f);
			
			if (_moveCount <= _requiredMoves)	// its possible to do it in less that minimum number of moves if you can move lumps of tiles.
			{
				// perfect solution!!
				if (_audioManager)
					_audioManager.PlayAudioClip("correctquizType2Question");
				movesText.SendMessage("Pulse",4f);
				bestPossibleText.SendMessage("Pulse",4f);
				yield return new WaitForSeconds(0.3f);
				fireworksManager.ManyStarBursts(5);
				encouragementManager.ShowEncouragement(EncouragementManager.EncouragementTypes.Perfect);
				if (_audioManager)
					_audioManager.PlayAudioClip("jigsawPerfect");
				yield return new WaitForSeconds(1.7f);
			}
			else
			{
				yield return new WaitForSeconds(0.4f);
			}









			bonusText.gameObject.SendMessage("Hide");
			bonusTextText.gameObject.SendMessage("Hide");
			bonusTextBackPanel.gameObject.SendMessage("Hide");
			timeTakenText.gameObject.SendMessage("Hide");
			timeTakenTextText.gameObject.SendMessage("Hide");
			timeTakenBackPanel.gameObject.SendMessage("Hide");
			rightText.gameObject.SendMessage("Hide");
			movesText.gameObject.SendMessage("Hide");
			movesTextText.gameObject.SendMessage("Hide");
			movesTextBackPanel.gameObject.SendMessage("Hide");
			bestPossibleText.gameObject.SendMessage("Hide");
			bestPossibleTextText.gameObject.SendMessage("Hide");
			bestPossibleBackPanel.gameObject.SendMessage("Hide");

			AdjustQuestionTimeAfterCorrectAnswer();

			gameState = GameStates.Waiting;

			EnableInput(true);
			ShowNextQuestion();
		}

		public void TimerExpired()
		{
			Debug.Log ("TIMER Expired");
			if (gameplaySettings.loseQuestionOnTimerExpire)
				StartCoroutine(WrongAnswerSequence(true));
		}

		private void GameOver()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("gameOver");
			gameplayPanel.SetActive(false);
			gameOverPanel.SetActive(true);
		}

		private void AllQuestionsFinished()
		{
			//if (_audioManager)
			//	_audioManager.PlayAudioClip("allQuestionsComplete");

            Application.LoadLevel("jigsaw level select");

            //gameplayPanel.SetActive(false);
			//levelCompletePanel.SetActive(true);
			//Visibility visibilityComponent = wellDoneText.GetComponent<Visibility>();
			//if (visibilityComponent != null)
			//	visibilityComponent.Flash(1000,0.5f,0.5f,true);
			//numCorrectAnswersTextUI.text = _correctAnswersCount+" out of "+_questionManager.GetNumberOfQuestions();
		}
		
		
		private void SetUpTilesArray(int xDim, int yDim)
		{
			_tilesData = new TileData[xDim*yDim];

			for (int i=0; i<xDim*yDim; i++)
			{
				_tilesData[i].type = TileType.Normal;
				_tilesData[i].moveCounter = 0;
				_tilesData[i].shakeMagnitude = 0f;
			}
		}

		private void SetUpJigsawArray(int xDim, int yDim)
		{
			_jigsawOrderArray = new int[xDim,yDim];
			_duplicateJigsawOrderArray = new int[xDim,yDim];

			int i = 0;
			for (int y = 0; y<yDim; y++)
			{
				for (int x = 0; x<xDim; x++)
				{
					_jigsawOrderArray[x,y] = i++;
				}
			}
		}

		private void DuplicateTheJigsawOrderArray()
		{
			int i = 0;
			for (int y = 0; y<gridYDim; y++)
			{
				for (int x = 0; x<gridXDim; x++)
				{
					_duplicateJigsawOrderArray[x,y] = _jigsawOrderArray[x,y];
				}
			}
		}

		private void RandomiseJigsawArray(int xDim, int yDim)
		{
			List<int> order = new List<int>();
			int i = 0;
			while (i < xDim*yDim)
			{
				order.Add (i++);
			}

			for (int y = 0; y<yDim; y++)
			{
				for (int x = 0; x<xDim; x++)
				{
					int index = UnityEngine.Random.Range(0,order.Count);
					_jigsawOrderArray[x,y] = order[index];
					order.RemoveAt (index);
				}
			}
		}
		
		int NumberOfPiecesInCorrectPlace()
		{
			int count = 0;
			int i = 0;
			for (int y = 0; y<gridYDim; y++)
			{
				for (int x = 0; x<gridXDim; x++)
				{
					if (TileAtPos(_jigsawOrderArray, x, y) == i++)
						count++;
				}
			}
			
			return count;
		}

		void ClearHiddenTiles()
		{
			for (int y = 0; y<gridYDim; y++)
			{
				for (int x = 0; x<gridXDim; x++)
				{
					_tilesData[TileAtPos(_jigsawOrderArray, x, y)].type = TileType.Normal;
				}
			}
		}

		bool CheckForHiddenTilesInCorrectLocation()
		{
			bool revealed = false;
			int i=0;
			for (int y = 0; y<gridYDim; y++)
			{
				for (int x = 0; x<gridXDim; x++)
				{
					TileType tt = _tilesData[TileAtPos(_jigsawOrderArray, x, y)].type;
					if ((tt == TileType.Hidden) || (tt == TileType.Bomb))
					{
						// this is a bomb or a hidden tile.
						// is it in the right position?
						if (TileAtPos(_jigsawOrderArray, x, y) == i)
						{
							// yes. Reveal it.
							RevealHiddenTile(x,y);
							revealed = true;
						}
					}
					i++;
				}
			}
			return revealed;
		}

		void RevealHiddenTile(int x, int y)
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("jigsawRevealHiddenTile");

			TileType tileType = _tilesData[TileAtPos(_jigsawOrderArray, x, y)].type;

			_tilesData[TileAtPos(_jigsawOrderArray, x, y)].type = TileType.Normal;

			GameObject tile = _jigsawSpritesArray[x,y];

			if (tileType == TileType.Hidden)
			{
				// do some fireworks
				// first work out the rect (in screen coords) which contains the tile that is being revealed
				Vector3[] buttonPanelCorners = new Vector3[4];
				tile.GetComponent<RectTransform>().GetWorldCorners(buttonPanelCorners);
				Vector2 fpos = new Vector2(buttonPanelCorners[0].x, buttonPanelCorners[0].y);
				Vector2 size = new Vector2(buttonPanelCorners[2].x-fpos.x, buttonPanelCorners[2].y-fpos.y);
				Rect buttonPanelWorldRect = new Rect(fpos,size);
				// now do the fireworks within that rect.
				if (FireworksManager._instance)
					FireworksManager._instance.ManyStarBursts(4,buttonPanelWorldRect,2500f,0.02f);
				RefreshJigsawPieces();
				if (JigsawFinished() && (gameState != GameStates.TotUp))
				{
					Answered(true);
				}
			}
			else if (tileType == TileType.Bomb)
			{
				Debug.Log ("MAKE PUFF OF SMOKE");
				// puff of smoke
				// first work out the rect (in screen coords) which contains the tile that is being revealed
				Vector3[] buttonPanelCorners = new Vector3[4];
				tile.GetComponent<RectTransform>().GetWorldCorners(buttonPanelCorners);
				Vector2 centre = new Vector2((buttonPanelCorners[0].x*1.5f + buttonPanelCorners[3].x*0.5f)/2, (buttonPanelCorners[0].y*0.5f + buttonPanelCorners[1].y*1.5f)/2);
				Vector2 fpos = new Vector2(buttonPanelCorners[0].x, buttonPanelCorners[0].y);
				Vector2 size = new Vector2(buttonPanelCorners[2].x-fpos.x, buttonPanelCorners[2].y-fpos.y);

				StartCoroutine(CancelBombSequence(tile));

				GameObject puff = Instantiate (puffOfSmokePrefab) as GameObject;
				puff.transform.SetParent(fireworksManager.transform);
				puff.transform.position = centre;
				puff.transform.localScale = new Vector3(1f,1f,1f);
//
//				Rect buttonPanelWorldRect = new Rect(fpos,size);
//				// now do the fireworks within that rect.
//				if (FireworksManager._instance)
//					FireworksManager._instance.ManyStarBursts(4,buttonPanelWorldRect,450f,0.02f);
			}
		}

		IEnumerator CancelBombSequence(GameObject tile)
		{
			EnableInput(false);
			tile.GetComponent<JigsawSquare>().AnimationCancelBomb();
			yield return new WaitForSeconds(1.0f);
			RefreshJigsawPieces(gameState != GameStates.TotUp);
			yield return new WaitForSeconds(0.8f);

			EnableInput(true);
			PostMoveChecks();
		}


		void ExplodeBomb(int x, int y)
		{
			Debug.Log ("ExplodeBomb");
			GameObject tile = _jigsawSpritesArray[x,y];

			//EnableInput(false);
			tile.GetComponentInChildren<Text>().text = "0";
			
			BombExplodeCallback explodecallback = BombHasExploded;
			tile.GetComponent<JigsawSquare>().AnimationBombExplode(explodecallback);
		}

		public void BombHasExploded()
		{
			VanishCarriedTiles();
			CancelAllBombExplosions();
			whiteLayer.GetComponent<Visibility>().FadeIn (0.1f);
			EnableInput(true);
			Answered(false);
		}

		private void CancelAllBombExplosions()
		{
			for (int x=0; x<gridXDim; x++)
			{
				for (int y=0; y<gridYDim; y++)
				{
					GameObject tile = _jigsawSpritesArray[x,y];
					tile.GetComponent<JigsawSquare>().CancelAnimations();
				}
			}
		}


		bool JigsawFinished()
		{
			//return true;

			int numCorrectPieces = NumberOfPiecesInCorrectPlace();
			return (numCorrectPieces == gridXDim*gridYDim);
		}


		int MinimumNumberOfMovesRequired()
		{
			int[,] remainingTiles = new int[gridXDim,gridYDim];
			int moveCount = 0;

			// copy the tiles arrangement into our array, removing any that are already in the correct place
			int i=0;
			for (int y = 0; y<gridYDim; y++)
			{
				for (int x = 0; x<gridXDim; x++)
				{
					if (TileAtPos(_jigsawOrderArray, x, y) == i++)
						remainingTiles[x,y] = -1;
					else
						remainingTiles[x,y] = TileAtPos(_jigsawOrderArray, x, y);
				}
			}

			// go through the tiles, counting the number of tiles in a 'loop'
			// (a loop is, for example, where tile A should replace tile B which should replace tile C which should replace tile A).
			for (int y = 0; y<gridYDim; y++)
			{
				for (int x = 0; x<gridXDim; x++)
				{
					if (remainingTiles[x,y] >= 0)
					{
						moveCount += (LoopCount(remainingTiles, remainingTiles[x,y]) - 1);
					}
				}
			}

			return moveCount;
		}

		int TileAtIndex(int[,] tiles, int index)
		{
			int x = index%gridXDim;
			int y = index/gridXDim;
			return TileAtPos(tiles, x, y);
		}

		int TileAtPos(int[,] tiles, int x, int y)
		{
			// returns tile number at given position in the grid.
			int a = tiles[x,y];
			return a;
		}
		
		void ClearTileAtIndex(int[,] tiles, int index)
		{
			int x = index%gridXDim;
			int y = index/gridXDim;
			tiles[x,y] = -1;
		}
		
		int LoopCount(int[,] tiles, int startTileIndex)
		{
			int currentTile = startTileIndex;
			int count = 0;

			do
			{
				int nextTile = TileAtIndex (tiles, currentTile);
				count++;
				ClearTileAtIndex(tiles, currentTile);
				currentTile = nextTile;
			}
			while (currentTile != startTileIndex);

			return count;
		}

		private void SetHiddenTiles(int numHiddenTiles, int requiredMoves)
		{
			// chooses numHiddenTiles tiles at random and makes them hidden
			List<int> order = new List<int>();
			int i = 0;
			while (i < gridXDim*gridYDim)
			{
				if (_tilesData[i].type == TileType.Normal)
					order.Add (i);
				i++;
			}

			int hiddenTileLifeSpan = requiredMoves;
			for (int f=0; f<numHiddenTiles; f++)
			{
				if (order.Count > 0)
				{				
					int listPos = UnityEngine.Random.Range(0,order.Count);
					int index = order[listPos];
					_tilesData[index].moveCounter = hiddenTileLifeSpan;
					_tilesData[index].type = TileType.Hidden;
					hiddenTileLifeSpan--;
					if (hiddenTileLifeSpan < 1)
						hiddenTileLifeSpan = 1;
					order.RemoveAt (listPos);
				}
			}
		}

		private void SetBombTiles(int numBombTiles, int requiredMoves)
		{
			// chooses numHiddenTiles tiles at random and makes them hidden
			List<int> order = new List<int>();
			int i = 0;
			while (i < gridXDim*gridYDim)
			{
				int x = i%gridXDim;
				int y = i/gridXDim;
				if (TileAtPos(_jigsawOrderArray, x, y) != i)	// don't choose bomb tiles that are already in the right place.
				{
					if (_tilesData[i].type == TileType.Normal)
						order.Add (i);
				}
				i++;
			}
			
			int hiddenTileLifeSpan = requiredMoves;
			for (int f=0; f<numBombTiles; f++)
			{
				if (order.Count > 0)
				{
					int listPos = UnityEngine.Random.Range(0,order.Count);
					int index = order[listPos];

					if (hiddenTileLifeSpan < 1)
						hiddenTileLifeSpan = 1;

					_tilesData[index].moveCounter = hiddenTileLifeSpan;
					_tilesData[index].type = TileType.Bomb;
					if (_tilesData[index].moveCounter <= gameplaySettings.bombLivesShakeStart)
						_tilesData[index].shakeMagnitude = 0.01f;
					else
						_tilesData[index].shakeMagnitude = 0.0f;
					hiddenTileLifeSpan--;

					order.RemoveAt (listPos);
				}
			}
		}
		
		private void ReduceLifeOfHiddenTiles()
		{
			for (int f=0; f<_tilesData.Length; f++)
			{
				if ((_tilesData[f].type == TileType.Hidden) || (_tilesData[f].type == TileType.Bomb))
				{
					if (_tilesData[f].moveCounter > 0)
					{
						_tilesData[f].moveCounter--;
						if (_tilesData[f].moveCounter == 0)
						{
							// reveal the tile
							// first need to find it in _jigsawOrderArray
							for (int x=0; x<gridXDim; x++)
							{
								for (int y=0; y<gridYDim; y++)
								{
									if (TileAtPos(_jigsawOrderArray, x, y) == f)
									{
										if (y*gridXDim + x == f)
										{
											// its in the correct place. Leave it to complete its reveal animation
											_tilesData[f].moveCounter = 1;
										}
										else
										{
											// ok - got the x,y coord of it. Can now do the reveal or explode, depending on type of tile
//											if (_tilesData[f].type == TileType.Hidden)
//												RevealHiddenTile(x,y);
											 
											if (_tilesData[f].type == TileType.Bomb)
												ExplodeBomb(x,y);
										}
									}
								}
							}
						}
						else 
						{
							IntVec2 tilePos = FindTileInGrid(f);
							_jigsawSpritesArray[tilePos.x,tilePos.y].GetComponent<JigsawSquare>().PulseBombCount();
							if (_tilesData[f].moveCounter == gameplaySettings.bombLivesShakeStart)
							{
								_tilesData[f].shakeMagnitude = 0.01f;
							}
						}

					}
				}
			}
		}

		private IntVec2 FindTileInGrid(int tilenum)
		{
			for (int x=0; x<gridXDim; x++)
			{
				for (int y=0; y<gridYDim; y++)
				{
					if (TileAtPos(_jigsawOrderArray, x, y) == tilenum)
					{
						IntVec2 a = new IntVec2(x,y);
						return a;
					}
				}
			}

			return new IntVec2(-1,-1);
		}

		private void SetSpecialTiles(int questionNumber)
		{
			int[] hiddenTilesMoreThan3by3 = { 0,0,1,2,0,1,2,0,1,2 };
			int[] bombTilesMoreThan3by3 = { 0,0,0,0,1,1,1,2,2,2 };
			int[] hiddenTilesLessThan3by3 = { 0,0,1,2,0,1,1,0,0,0 };
			int[] bombTilesLessThan3by3 = { 0,0,0,0,1,1,1,2,2,2 };
			int[] bombDifficulty = { 3,3,2,2,2,1,1,1,0,0 };			// added to the minimum number of moves to give number of bomb 'lives'

			int numQuestions = _questionManager.GetNumberOfQuestions();
			int index = (int)(((float)questionNumber / (numQuestions-1)) * (hiddenTilesMoreThan3by3.Length-1));

			Question q = _questionManager.GetQuestion(questionNumber);
			int numBombs, numHidden, bombDiff;
			if (q.gridX*q.gridY >= 9)
			{
				numHidden = hiddenTilesMoreThan3by3[index];
				numBombs = bombTilesMoreThan3by3[index];
			}
			else
			{
				numHidden = hiddenTilesLessThan3by3[index];
				numBombs = bombTilesLessThan3by3[index];
			}
			bombDiff = bombDifficulty[index];

			SetHiddenTiles(numHidden, _requiredMoves);
			SetBombTiles(numBombs, _requiredMoves+bombDiff-2);
		}


		private void GenerateSquareJigsawPieces(int questionNumber, Texture texture)
		{
			Question q = _questionManager.GetQuestion(questionNumber);

			DeleteObscuringSquares();
			
			Transform squaresParent = picture1ImageBox.transform;

			int xdim = q.gridX;
			int ydim = q.gridY;
			if (gameplaySettings.gridX > 0)
				xdim = gameplaySettings.gridX;
			if (gameplaySettings.gridY > 0)
				ydim = gameplaySettings.gridY;

			gridXDim = xdim;
			gridYDim = ydim;

			SetUpTilesArray(xdim,ydim);
			SetUpJigsawArray(xdim,ydim);

			int maxPiecesAllowedInCorrectPlace = 0;
			if (xdim*ydim <= 4)
				maxPiecesAllowedInCorrectPlace = xdim*ydim - 1;	// ok to have some (but not all) pieces in correct place with small jigsaws
			do
				RandomiseJigsawArray(xdim,ydim);
			while (NumberOfPiecesInCorrectPlace() > maxPiecesAllowedInCorrectPlace);	// make sure it doesn't have too many pieces in the correct place

			_requiredMoves = MinimumNumberOfMovesRequired();

			// hidden tiles
			if (gameplaySettings.useBombsAndHiddenSquares)
				SetSpecialTiles(questionNumber);

//			_numberOfHiddenTiles = 2;	//(int)((((float)_currentQuestionNumber)/_questionManager.GetNumberOfQuestions()) * gridXDim * gridYDim * 0.7f);
//			SetHiddenTiles(_numberOfHiddenTiles, _requiredMoves);
//			SetBombTiles(2, _requiredMoves);

			minimumMoves.text = "/ "+_requiredMoves.ToString();

			_jigsawSpritesArray = new GameObject[xdim,ydim];

			for (int y = 0; y<ydim; y++)
			{
				GameObject newColumn = Instantiate (obscuringColumnPrefab) as GameObject;
				newColumn.transform.SetParent(squaresParent);
				for (int x = 0; x<xdim; x++)
				{
					GameObject newSquare = Instantiate (obscuringSquarePrefab) as GameObject;
					_jigsawSpritesArray[x,y] = newSquare;
					JigsawSquarePressCallback callback = PressedSquare;
					JigsawSquareDragCallback dragcallback = DraggedSquare;
					JigsawSquareReleaseCallback releasedcallback = ReleasedSquare;
					JigsawSquare os = newSquare.GetComponent<JigsawSquare>();
					os.SetPressedCallback(callback);
					os.SetDraggedCallback(dragcallback);
					os.SetReleasedCallback(releasedcallback);
					os.SetGridXY(x,y);
					RawImage ri = newSquare.GetComponent<RawImage>();
					ri.texture = texture;
					ri.uvRect = GetPieceUV(x, y);
					newSquare.transform.SetParent(newColumn.transform);
				}
			}

			RefreshJigsawPieces();
		}

		// update UV coords of the jigsaw pieces in case some pieces have moved.
		private void RefreshJigsawPieces(bool showCorrectTiles = true)
		{
			for (int y = 0; y<gridYDim; y++)
			{
				for (int x = 0; x<gridXDim; x++)
				{
					GameObject square = _jigsawSpritesArray[x,y];
					JigsawSquare js = square.GetComponent<JigsawSquare>();
					if (!js.RefreshIsBlocked())
					{
						RawImage ri = square.GetComponent<RawImage>();
						SetTexAndUVOfPiece(ri,x,y);
						if (gameplaySettings.showCorrectTiles &&
						    showCorrectTiles && 
						    IsInTheCorrectPosition(_jigsawOrderArray[x,y], new IntVec2(x,y)))
						{
						    js._correctPositionIndicatorLayer.GetComponent<Visibility>().SetAlpha(0.4f);
							ri.color = new Color(0.7f,0.7f,0.7f,1.0f);
						}
						else
						{
							js._correctPositionIndicatorLayer.GetComponent<Visibility>().Hide();
							ri.color = new Color(1f,1f,1f,1.0f);
						}
					}
				}
			}		
		}

		private Rect GetPieceUV(int x, int y)
		{
			float cellWidth = 1.0f / gridXDim;
			float cellHeight = 1.0f / gridYDim;
			int n = TileAtPos(_jigsawOrderArray, x, y);
			int cy = n/gridXDim;
			int cx = n%gridXDim;
			Rect uv = new Rect(cx*cellWidth, (gridYDim-1-cy)*cellHeight, cellWidth, cellHeight);
			return uv;
		}


		public void PressedSquare(int x, int y, Vector2 touchPosition)
		{
			if ((gameState == GameStates.Waiting) && inputEnabled)
			{
				_carriedPiecePosition.x = x;
				_carriedPiecePosition.y = y;

				PickUpAllAdjoiningCorrectTiles(new IntVec2(x,y), new IntVec2(0,0));
//				AddTileToCarriedTiles(new IntVec2(x,y), new IntVec2(0,0));
//				//AddTileToCarriedTiles(new IntVec2(x+1,y), new IntVec2(1,0));
//				AddTileToCarriedTiles(new IntVec2(x,y+1), new IntVec2(0,-1));

//				RectTransform rt = picture1ImageBox.GetComponent<RectTransform>();
//				jigsawCellSize = new Vector2( rt.rect.width/gridXDim, rt.rect.height/gridYDim );
//
//				// hide the tile that you have picked up
//				GameObject oldSquare = _jigsawSpritesArray[x,y];
//				oldSquare.GetComponent<RawImage>().color = Color.black;
//				oldSquare.GetComponentInChildren<Text>().text = "";
//				oldSquare.GetComponentInChildren<JigsawSquare>()._correctPositionIndicatorLayer.SendMessage("Hide");
//
//				// create the carried tile
//				GameObject carried = Instantiate(carriedJigsawPiecePrefab) as GameObject;
//				carried.transform.SetParent(allCarriedJigsawPieces.transform);
//				carriedJigsawPieceParent = carried.transform;
//				carriedJigsawPieceParent.localPosition = Vector2.zero;
//				allCarriedJigsawPieces.transform.position = touchPosition;
//				GameObject carriedPiece = carried.GetComponent<CarriedPieceParent>().carriedPiece;
//				GameObject carriedPieceShadow = carried.GetComponent<CarriedPieceParent>().carriedPieceShadow;
//				RawImage raw = carriedPiece.GetComponent<RawImage>();
//				SetTexAndUVOfPiece(raw,x,y);
//				rt = carriedPiece.GetComponent<RectTransform>();
//				rt.sizeDelta = jigsawCellSize;
//				rt = carriedPieceShadow.GetComponent<RectTransform>();
//				rt.sizeDelta = jigsawCellSize;

				allCarriedJigsawPieces.transform.position = touchPosition;

//				carriedJigsawPieceParent.position = touchPosition;
//				carriedJigsawPieceParent.gameObject.SetActive (true);
//				RawImage raw = carriedJigsawPiece.GetComponent<RawImage>();
//				SetTexAndUVOfPiece(raw,x,y);
//				rt = carriedJigsawPiece.GetComponent<RectTransform>();
//				rt.sizeDelta = jigsawCellSize;
//				rt = carriedJigsawShadow.GetComponent<RectTransform>();
//				rt.sizeDelta = jigsawCellSize;

				if (_audioManager)
					_audioManager.PlayAudioClip("jigsawPickUp");

				gameState = GameStates.Dragging;
			}
		}

		public void PickUpAllAdjoiningCorrectTiles(IntVec2 centre, IntVec2 pickupOffset)
		{
			// recursive function.
			// pick up the tile that was clicked on, plus all adjoining tiles that are correctly arranged (relative to the first tile)

			// pseudocode
			// for each adjacent square
			// if adjoining and not already picked up then 
			//		pick up
			//		PickUpAllAdjoiningCorrectTiles
			// return

			AddTileToCarriedTiles(centre, pickupOffset);

			TileType centreType = _tilesData[TileAtPos(_jigsawOrderArray, centre.x, centre.y)].type;
			if ((centreType != TileType.Bomb) &&
			    (centreType != TileType.Hidden) &&
			    (gameplaySettings.moveLumpsOfTiles))
			{
				int[] xoffsets = { 0, 1, 0, -1 };
				int[] yoffsets = { -1, 0, 1, 0 };

				for (int i=0; i<xoffsets.Length; i++)
				{
					int x = centre.x + xoffsets[i];
					int y = centre.y + yoffsets[i];
					if (x>=0 &&
					    x<gridXDim &&
					    y>=0 &&
					    y<gridYDim )
					{
						IntVec2 gridPos = new IntVec2(x,y);
						int correctAdjoiningNum = _jigsawOrderArray[centre.x,centre.y] + xoffsets[i] + yoffsets[i] * gridXDim;

						// check if we have wrapped round horizontally
						if ((yoffsets[i] == 0) && (_jigsawOrderArray[centre.x,centre.y]/gridXDim != correctAdjoiningNum/gridXDim))
							correctAdjoiningNum = -1;

						TileType adjacentType = _tilesData[TileAtPos(_jigsawOrderArray, x, y)].type;

						// check if adjoining tile is a bomb or a hidden tile
						if ((adjacentType == TileType.Bomb) ||
						    (adjacentType == TileType.Hidden))
						{
							// don't add it to the lump
							correctAdjoiningNum = -1;
						}

						if (_jigsawOrderArray[x,y] == correctAdjoiningNum
						    && !AlreadyCarried(gridPos))
						{
							IntVec2 newPickupOffset = new IntVec2(pickupOffset.x+xoffsets[i],pickupOffset.y-yoffsets[i]);
							PickUpAllAdjoiningCorrectTiles(gridPos, newPickupOffset);
						}
					}
				}
			}
		}

		public bool AlreadyCarried(IntVec2 jigsawPos)
		{
			// see if the tile at jigsawPos is already in the list of carried tiles
			foreach(CarriedTileData ct in _carriedPieces)
			{
				if (ct.originalPosition.x == jigsawPos.x &&
				    ct.originalPosition.y == jigsawPos.y)
				{
					return true;
				}
			}
			return false;
		}

		private void CopyBombExplosionStateToCarriedTile(GameObject carriedTile, GameObject bombTile)
		{
			JigsawSquare bomb = bombTile.GetComponent<JigsawSquare>();
			Debug.Log ("***** bomb state = "+bomb.state);
			if (bomb.state == JigsawSquare.JigsawSquareStates.Exploding)
			{
				BombExplodeCallback explodecallback = BombHasExploded;
				carriedTile.GetComponent<JigsawSquare>().AnimationBombExplode(explodecallback, bomb.GetAnimationTime());
			}
		}

		public void AddTileToCarriedTiles(IntVec2 jigsawPos, IntVec2 carriedOffset)
		{
			RectTransform rt = picture1ImageBox.GetComponent<RectTransform>();
			jigsawCellSize = new Vector2( rt.rect.width/gridXDim, rt.rect.height/gridYDim );
			GameObject oldSquare = _jigsawSpritesArray[jigsawPos.x, jigsawPos.y];

			// create the carried tile
			GameObject carried = Instantiate(carriedJigsawPiecePrefab) as GameObject;
			carried.transform.SetParent(allCarriedJigsawPieces.transform);
			carriedJigsawPieceParent = carried.transform;
			carriedJigsawPieceParent.localPosition = new Vector2(jigsawCellSize.x * carriedOffset.x, jigsawCellSize.y * carriedOffset.y);
			GameObject carriedPiece = carried.GetComponent<CarriedPieceParent>().carriedPiece;
			GameObject carriedPieceShadow = carried.GetComponent<CarriedPieceParent>().carriedPieceShadow;
			RawImage raw = carriedPiece.GetComponent<RawImage>();
			SetTexAndUVOfPiece(raw, jigsawPos.x, jigsawPos.y);
			if (_tilesData[TileAtPos(_jigsawOrderArray, jigsawPos.x, jigsawPos.y)].type == TileType.Bomb)
			{
				Debug.Log ("******* Start bomb exploding");
				CopyBombExplosionStateToCarriedTile(carried, oldSquare);
			}
			rt = carriedPiece.GetComponent<RectTransform>();
			rt.sizeDelta = jigsawCellSize;
			rt = carriedPieceShadow.GetComponent<RectTransform>();
			rt.sizeDelta = jigsawCellSize;

			CarriedTileData pieceData = new CarriedTileData();
			pieceData.originalPosition = jigsawPos;
			pieceData.carriedTile = carried;
			pieceData.carriedOffset = carriedOffset;
			_carriedPieces.Add(pieceData);

			// hide the tile that you have picked up
			oldSquare.GetComponent<JigsawSquare>().CancelAnimations();
			oldSquare.GetComponent<RawImage>().color = Color.black;
			oldSquare.GetComponentInChildren<Text>().text = "";
			oldSquare.GetComponentInChildren<JigsawSquare>()._correctPositionIndicatorLayer.SendMessage("Hide");
			
			// re-parent the piece and shadow to the correct part of the hierarchy to get the draw order right
			GameObject allPieces = allCarriedJigsawPieces.GetComponent<AllCarriedJigsawPieces>().AllPieces;
			GameObject allShadows = allCarriedJigsawPieces.GetComponent<AllCarriedJigsawPieces>().AllShadows;
			carriedPiece.transform.SetParent(allPieces.transform);
			carriedPieceShadow.transform.SetParent(allShadows.transform);
		}

		void SetTexAndUVOfPiece(RawImage ri, int x, int y)
		{
			if (_tilesData[TileAtPos(_jigsawOrderArray, x, y)].type == TileType.Normal)
			{
				ShakeImage shake = ri.GetComponent<ShakeImage>();
				if (shake != null)
					shake.SetShakeMagnitude(0f);
				ri.texture = _imageTexture;
				ri.uvRect = GetPieceUV(x, y);
				Text text = ri.GetComponentInChildren<Text>();
				if (text != null)
					text.text = "";
			}
			else if (_tilesData[TileAtPos(_jigsawOrderArray, x, y)].type == TileType.Bomb)
			{
				ri.texture = bombTexture;
				ri.uvRect = new Rect(0f,0f,1f,1f);
				int counter = _tilesData[TileAtPos(_jigsawOrderArray, x, y)].moveCounter;
				Text text = ri.GetComponentInChildren<Text>();
				if (text != null)
					text.text = counter.ToString();
				ShakeImage shake = ri.GetComponent<ShakeImage>();
				if (shake != null)
					shake.SetShakeMagnitude(_tilesData[TileAtPos(_jigsawOrderArray, x, y)].shakeMagnitude);
			}
			else // hidden tile
			{
				ShakeImage shake = ri.GetComponent<ShakeImage>();
				if (shake != null)
					shake.SetShakeMagnitude(0f);
				ri.texture = hiddenPieceTexture;
				ri.uvRect = new Rect(0f,0f,1f,1f);
				int counter = _tilesData[TileAtPos(_jigsawOrderArray, x, y)].moveCounter;
				Text text = ri.GetComponentInChildren<Text>();
				if (text != null)
					text.text = "";
				//ri.GetComponentInChildren<Text>().text = counter.ToString();
			}
		}

		public void DraggedSquare(Vector2 touchPosition)
		{
			if (gameState == GameStates.Dragging)
				allCarriedJigsawPieces.transform.position = touchPosition;
		}

		private IntVec2 ConstrainDropToGrid(IntVec2 dropPos)
		{
//			Debug.Log ("ConstrainDropToGrid droppos = "+dropPos.x+","+dropPos.y);
			// checks if the dropped lump of tiles would extend outside the jigsaw grid, and adjusts the drop position if so.
			int minx = 0;
			int miny = 0;
			int maxx = 0;
			int maxy = 0;
			foreach(CarriedTileData cd in _carriedPieces)
			{
				minx = Math.Min (minx, cd.carriedOffset.x);
				maxx = Math.Max (maxx, cd.carriedOffset.x);
				miny = Math.Min (miny, -cd.carriedOffset.y);
				maxy = Math.Max (maxy, -cd.carriedOffset.y);
			}
			int left = dropPos.x + minx;
			if (left < 0)
				dropPos.x -= left;
			int right = dropPos.x + maxx;
			if (right >= gridXDim)
				dropPos.x -= (right-gridXDim+1);
			int top = dropPos.y + miny;
			if (top < 0)
				dropPos.y -= top;
			int bottom = dropPos.y + maxy;
			if (bottom >= gridYDim)
				dropPos.y -= (bottom-gridYDim+1);

//			Debug.Log ("ConstrainDropToGrid out left = "+left);
//			Debug.Log ("ConstrainDropToGrid out right = "+right);
//			Debug.Log ("ConstrainDropToGrid out top = "+top);
//			Debug.Log ("ConstrainDropToGrid out bottom = "+bottom);
//
//			Debug.Log ("ConstrainDropToGrid out droppos = "+dropPos.x+","+dropPos.y);

			return dropPos;
		}

		private void VanishCarriedTiles()
		{
			// just makes the carried tiles disappear. Doesn't drop them or anything like that.
			// used for when you are carrying a bomb and it goes off.
			if (gameState == GameStates.Dragging)
			{
				foreach(CarriedTileData cd in _carriedPieces)
				{
					GameObject carriedPiece = cd.carriedTile.GetComponent<CarriedPieceParent>().carriedPiece;
					GameObject carriedPieceShadow = cd.carriedTile.GetComponent<CarriedPieceParent>().carriedPieceShadow;
					Destroy(carriedPiece);
					Destroy(carriedPieceShadow);
					Destroy(cd.carriedTile);
				}
				_carriedPieces.Clear ();
				gameState = GameStates.Waiting;
			}
		}

		public void ReleasedSquare(Vector2 touchPosition)
		{
			if (gameState == GameStates.Dragging)
			{
				bool moved = false;

				DuplicateTheJigsawOrderArray();

				droppedPiecePosition = GetPieceCoordsFromTouchPos(touchPosition);			
				droppedPiecePosition = ConstrainDropToGrid(droppedPiecePosition);

				// increment moveCount if we are not just dropping the piece back where it started.
				if (!((_carriedPiecePosition.x == droppedPiecePosition.x) && (_carriedPiecePosition.y == droppedPiecePosition.y)))
				{
					_moveCount++;
					moveCount.text = _moveCount.ToString();
					moved = true;
	
					if (_audioManager)
						_audioManager.PlayAudioClip("jigsawDrop");
	
					if (IsInTheCorrectPosition(_jigsawOrderArray[_carriedPiecePosition.x,_carriedPiecePosition.y], droppedPiecePosition))
					{
						if (_audioManager)
							_audioManager.PlayAudioClip("jigsawDropCorrect");
					}
				}
				else
				{
					if (_audioManager)
						_audioManager.PlayAudioClip("jigsawDropInSamePlace");				
				}

				gameState = GameStates.Dropping;

				// build a list of all the squares that are being vacated by this lump of tiles, after removing those
				// that will be reoccupied by tiles from the lump in its new position
				List<IntVec2> emptySquareList = new List<IntVec2>();
				List<IntVec2> reoccupiedSquareList = new List<IntVec2>();
				foreach(CarriedTileData cd in _carriedPieces)
				{
					bool willBeReoccupied = false;
					foreach(CarriedTileData cd2 in _carriedPieces)
					{
						IntVec2 tileDropPos = droppedPiecePosition;
						tileDropPos.x += cd2.carriedOffset.x;
						tileDropPos.y -= cd2.carriedOffset.y;
						if ((tileDropPos.x == cd.originalPosition.x) && (tileDropPos.y == cd.originalPosition.y))
							willBeReoccupied = true;
					}
					if (willBeReoccupied)
					{
						reoccupiedSquareList.Add(cd.originalPosition);
					}
					else
					{
						emptySquareList.Add(cd.originalPosition);
					}
				}

				int nextEmptySquare = 0;
				foreach(CarriedTileData cd in _carriedPieces)
				{
					IntVec2 tileDropPos = droppedPiecePosition;
					tileDropPos.x += cd.carriedOffset.x;
					tileDropPos.y -= cd.carriedOffset.y;

					Debug.Log ("tileDropPos "+tileDropPos.x+","+tileDropPos.y);
					Debug.Log ("carriedOffset "+cd.carriedOffset.x+","+cd.carriedOffset.y);

//					droppedPiecePosition.x += cd.carriedOffset.x;
//					droppedPiecePosition.y += cd.carriedOffset.y;

					bool reoccupied = false;
					foreach(IntVec2 reoccupiedSquare in reoccupiedSquareList)
					{
						if ((tileDropPos.x == reoccupiedSquare.x) && (tileDropPos.y == reoccupiedSquare.y))
							reoccupied = true;
					}

					if (reoccupied)
						StartCoroutine(DoSinglePieceSwap(tileDropPos, cd.originalPosition, null, cd.carriedTile));
					else
					{
						StartCoroutine(DoSinglePieceSwap(tileDropPos, cd.originalPosition, emptySquareList[nextEmptySquare++], cd.carriedTile));
					}
				}

				_carriedPieces.Clear();

				StartCoroutine(PostMoveActions(moved));

			}
		}


		private bool IsInTheCorrectPosition(int orderNum, IntVec2 position)
		{
			int correctX = orderNum % gridXDim;
			int correctY = orderNum / gridXDim;
			return ((correctX == position.x) && (correctY == position.y));
		}

//		private void printJigsawOrderArray()
//		{
//			for (int y=0; y<gridYDim; y++)
//			{
//				if (gridXDim == 3)
//					Debug.Log (_jigsawOrderArray[0,y]+","+_jigsawOrderArray[1,y]+","+_jigsawOrderArray[2,y]);
//				if (gridXDim == 4)
//					Debug.Log (_jigsawOrderArray[0,y]+","+_jigsawOrderArray[1,y]+","+_jigsawOrderArray[2,y]+","+_jigsawOrderArray[3,y]);
//			}
//		}


		IEnumerator DoSinglePieceSwap(IntVec2 dropJigsawPos, IntVec2 originalPos, IntVec2? displacedDestination, GameObject carriedTile)
		{
//			Debug.Log ("dropJigsawPos "+dropJigsawPos.x+","+dropJigsawPos.y);
//			Debug.Log ("originalPos "+originalPos.x+","+originalPos.y);
//			Debug.Log ("displacedDestination "+displacedDestination.Value.x+","+displacedDestination.Value.y);

			// move carried piece into position
			Vector2 dropPos = GetScreenCoordsFromPieceGridPos(dropJigsawPos);
			dropPos -= (Vector2)allCarriedJigsawPieces.transform.position;
			//dropPos -= (Vector2)carriedTile.transform.position;
			//float movementTime = 0.25f;

			GameObject carriedPiece = carriedTile.GetComponent<CarriedPieceParent>().carriedPiece;
			GameObject carriedPieceShadow = carriedTile.GetComponent<CarriedPieceParent>().carriedPieceShadow;
			carriedPiece.GetComponent<Movement>().MoveTo(dropPos,tileMovementTime);
			carriedPieceShadow.GetComponent<Movement>().MoveTo(dropPos,tileMovementTime);
		
			int temp = _duplicateJigsawOrderArray[originalPos.x,originalPos.y];
			if (displacedDestination != null)
			{
				_jigsawOrderArray[displacedDestination.Value.x,displacedDestination.Value.y] = _duplicateJigsawOrderArray[dropJigsawPos.x,dropJigsawPos.y];
			}
			_jigsawOrderArray[dropJigsawPos.x,dropJigsawPos.y] = temp;

			GameObject swappedMovingTile = null;
			if (displacedDestination != null)
			{
				// move swapped piece into the gap left by the carried piece
				RectTransform rt = picture1ImageBox.GetComponent<RectTransform>();
				jigsawCellSize = new Vector2( rt.rect.width/gridXDim, rt.rect.height/gridYDim );
				GameObject swappedSquare = _jigsawSpritesArray[dropJigsawPos.x, dropJigsawPos.y];
				swappedSquare.GetComponent<RawImage>().color = Color.black;


				swappedMovingTile = Instantiate(carriedJigsawPiecePrefab) as GameObject;
				swappedMovingTile.transform.SetParent(allSwappedJigsawPieces.transform);
				Vector2 startPos = GetScreenCoordsFromPieceGridPos(dropJigsawPos);
				swappedMovingTile.transform.position = startPos;
				GameObject swappedPiece = swappedMovingTile.GetComponent<CarriedPieceParent>().carriedPiece;
				GameObject swappedPieceShadow = swappedMovingTile.GetComponent<CarriedPieceParent>().carriedPieceShadow;

				if (_tilesData[TileAtPos(_jigsawOrderArray, displacedDestination.Value.x, displacedDestination.Value.y)].type == TileType.Bomb)
				{
					Debug.Log ("******* Start bomb exploding");
					CopyBombExplosionStateToCarriedTile(swappedMovingTile, swappedSquare);
				}
				swappedSquare.GetComponent<JigsawSquare>().CancelAnimations();

				RawImage raw = swappedPiece.GetComponent<RawImage>();
				SetTexAndUVOfPiece(raw, displacedDestination.Value.x, displacedDestination.Value.y);
				rt = swappedPiece.GetComponent<RectTransform>();
				rt.sizeDelta = jigsawCellSize;
				rt = swappedPieceShadow.GetComponent<RectTransform>();
				rt.sizeDelta = jigsawCellSize;

	//			Vector2 startPos = GetScreenCoordsFromPieceGridPos(droppedPiecePosition);
	//			swappedJigsawPieceParent.position = startPos;

				dropPos = GetScreenCoordsFromPieceGridPos(displacedDestination.Value);
				dropPos -= (Vector2)swappedMovingTile.transform.position;
//				Debug.Log ("To position "+displacedDestination.Value.x+", "+displacedDestination.Value.y);
//				Debug.Log ("dropPos "+dropPos.x+", "+dropPos.y);

				swappedPiece.GetComponent<Movement>().MoveTo(dropPos,tileMovementTime*0.75f);
				swappedPieceShadow.GetComponent<Movement>().MoveTo(dropPos,tileMovementTime*0.75f);
			}


			yield return new WaitForSeconds(tileMovementTime);

			GameObject oldSquare = _jigsawSpritesArray[originalPos.x,originalPos.y];
			oldSquare.GetComponent<RawImage>().color = Color.white;
			
			RefreshJigsawPieces();
			//swappedSquare.GetComponent<RawImage>().color = Color.white;

			if (_tilesData[TileAtPos(_jigsawOrderArray, dropJigsawPos.x, dropJigsawPos.y)].type == TileType.Bomb)
			{
				Debug.Log ("******* Start bomb exploding");
				CopyBombExplosionStateToCarriedTile(_jigsawSpritesArray[dropJigsawPos.x,dropJigsawPos.y], carriedTile);
			}

			if (displacedDestination != null)
			{
				if (_tilesData[TileAtPos(_jigsawOrderArray, displacedDestination.Value.x, displacedDestination.Value.y)].type == TileType.Bomb)
				{
					Debug.Log ("******* Start displacedDestination bomb exploding");
					CopyBombExplosionStateToCarriedTile(_jigsawSpritesArray[displacedDestination.Value.x,displacedDestination.Value.y], swappedMovingTile);
				}
			}

			Destroy(carriedPiece);
			Destroy(carriedPieceShadow);
			Destroy(carriedTile);
			if (swappedMovingTile)
				Destroy(swappedMovingTile);
			//carriedJigsawPieceParent.gameObject.SetActive (false);
			//swappedJigsawPieceParent.gameObject.SetActive (false);
			gameState = GameStates.Waiting;
		}

		IEnumerator PostMoveActions(bool moved)
		{
			yield return new WaitForSeconds(tileMovementTime*1.1f);

			if (moved)
			{
				CheckForHiddenTilesInCorrectLocation();
				ReduceLifeOfHiddenTiles();
				
				if (inputEnabled)
				{
					RefreshJigsawPieces();
					PostMoveChecks ();
				}
			}
			else
			{
				RefreshJigsawPieces();
			}
		}


//		IEnumerator DoPieceSwap()
//		{
//			bool moved = false;
//
//			// move carried piece into position
//			Vector2 dropPos = GetScreenCoordsFromPieceGridPos(droppedPiecePosition);
//			dropPos -= (Vector2)carriedJigsawPieceParent.position;
//			float movementTime = 0.25f;
//
//			GameObject carriedPiece = carriedJigsawPieceParent.gameObject.GetComponent<CarriedPieceParent>().carriedPiece;
//			GameObject carriedPieceShadow = carriedJigsawPieceParent.gameObject.GetComponent<CarriedPieceParent>().carriedPieceShadow;
//			carriedPiece.GetComponent<Movement>().MoveTo(dropPos,movementTime);
//			carriedPieceShadow.GetComponent<Movement>().MoveTo(dropPos,movementTime);
//
//			// move swapped piece into the gap left by the carried piece
//			RectTransform rt = picture1ImageBox.GetComponent<RectTransform>();
//			jigsawCellSize = new Vector2( rt.rect.width/gridXDim, rt.rect.height/gridYDim );
//			GameObject swappedSquare = _jigsawSpritesArray[droppedPiecePosition.x, droppedPiecePosition.y];
//			swappedSquare.GetComponent<RawImage>().color = Color.black;
//			swappedJigsawPieceParent.gameObject.SetActive (true);
//			RawImage raw = swappedJigsawPiece.GetComponent<RawImage>();
//			SetTexAndUVOfPiece(raw,droppedPiecePosition.x,droppedPiecePosition.y);
//			rt = swappedJigsawPiece.GetComponent<RectTransform>();
//			rt.sizeDelta = jigsawCellSize;
//			rt = swappedJigsawShadow.GetComponent<RectTransform>();
//			rt.sizeDelta = jigsawCellSize;
//
//			Vector2 startPos = GetScreenCoordsFromPieceGridPos(droppedPiecePosition);
//			swappedJigsawPieceParent.position = startPos;
//			dropPos = GetScreenCoordsFromPieceGridPos(_carriedPiecePosition);
//			dropPos -= (Vector2)swappedJigsawPieceParent.position;
//
//			swappedJigsawPiece.GetComponent<Movement>().MoveTo(dropPos,movementTime*0.75f);
//			swappedJigsawShadow.GetComponent<Movement>().MoveTo(dropPos,movementTime*0.75f);
//
//			// increment moveCount if we are not just dropping the piece back where it started.
//			if (!((_carriedPiecePosition.x == droppedPiecePosition.x) && (_carriedPiecePosition.y == droppedPiecePosition.y)))
//			{
//				_moveCount++;
//				moveCount.text = _moveCount.ToString();
//				moved = true;
//
//				if (_audioManager)
//					_audioManager.PlayAudioClip("jigsawDrop");
//
//				if (IsInTheCorrectPosition(_jigsawOrderArray[_carriedPiecePosition.x,_carriedPiecePosition.y], droppedPiecePosition))
//				{
//					if (_audioManager)
//						_audioManager.PlayAudioClip("jigsawDropCorrect");
//				}
//			}
//			else
//			{
//				if (_audioManager)
//					_audioManager.PlayAudioClip("jigsawDropInSamePlace");				
//			}
//
//			int temp = _jigsawOrderArray[_carriedPiecePosition.x,_carriedPiecePosition.y];
//			_jigsawOrderArray[_carriedPiecePosition.x,_carriedPiecePosition.y] = _jigsawOrderArray[droppedPiecePosition.x,droppedPiecePosition.y];
//			_jigsawOrderArray[droppedPiecePosition.x,droppedPiecePosition.y] = temp;
//
//			yield return new WaitForSeconds(movementTime);
//
//			GameObject oldSquare = _jigsawSpritesArray[_carriedPiecePosition.x,_carriedPiecePosition.y];
//			oldSquare.GetComponent<RawImage>().color = Color.white;
//
//			RefreshJigsawPieces();
//			swappedSquare.GetComponent<RawImage>().color = Color.white;
//
//			Destroy(carriedJigsawPieceParent.gameObject);
//			//carriedJigsawPieceParent.gameObject.SetActive (false);
//			swappedJigsawPieceParent.gameObject.SetActive (false);
//			gameState = GameStates.Waiting;
//
//			//carriedJigsawPiece.GetComponent<Movement>().ResetPosition();
//			//carriedJigsawShadow.GetComponent<Movement>().ResetPosition();
//			swappedJigsawPiece.GetComponent<Movement>().ResetPosition();
//			swappedJigsawShadow.GetComponent<Movement>().ResetPosition();
//
//			if (moved)
//			{
//				CheckForHiddenTilesInCorrectLocation();
//				ReduceLifeOfHiddenTiles();
//
//				if (inputEnabled)
//				{
//					RefreshJigsawPieces();
//					PostMoveChecks ();
//				}
//			}
//			else
//			{
//				RefreshJigsawPieces();
//			}
//		}

		private void PostMoveChecks()
		{
			if (JigsawFinished() && (gameState != GameStates.TotUp))
			{
				Answered(true);
			}
		}


		private void DeleteObscuringSquares()
		{
			foreach (Transform child in picture1ImageBox.transform) 
				Destroy(child.gameObject);
		}

		public struct IntVec2
		{
			public int x,y;
			
			public IntVec2(int ix, int iy)
			{
				x = ix;
				y = iy;
			}

			public static IntVec2 operator +(IntVec2 c1, IntVec2 c2) 
			{
				return new IntVec2(c1.x + c2.x, c1.y + c2.y);
			}
		}

		private IntVec2 GetPieceCoordsFromTouchPos(Vector2 touchPos)
		{
			RectTransform rt = pictureImageUI.GetComponent<RectTransform>();
			Vector3[] corners = new Vector3[4];
			rt.GetWorldCorners(corners);

			Vector2 rpos = touchPos - (Vector2)corners[0];
			rpos.x /= (corners[3].x - corners[0].x);
			rpos.y /= (corners[1].y - corners[0].y);
			rpos.y = 1.0f - rpos.y;
			IntVec2 ip = new IntVec2((int)(rpos.x * gridXDim),(int)(rpos.y * gridYDim));

			ip.x = Mathf.Clamp(ip.x, 0, gridXDim-1);
			ip.y = Mathf.Clamp(ip.y, 0, gridYDim-1);

			return ip;
		}

		private Vector2 GetScreenCoordsFromPieceGridPos(IntVec2 gridPos)
		{
			RectTransform rt = pictureImageUI.GetComponent<RectTransform>();
			Vector3[] corners = new Vector3[4];
			rt.GetWorldCorners(corners);

			Vector2 localPos;
			localPos.x = ((gridPos.x + 0.5f) * (corners[3].x - corners[0].x))/gridXDim;
			localPos.y = (((gridYDim - 1 - gridPos.y) + 0.5f) * (corners[1].y - corners[0].y))/gridYDim;

			Vector2 screenPos = localPos + (Vector2)corners[0];

			return screenPos;
		}

	}
}
