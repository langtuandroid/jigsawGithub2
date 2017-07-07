using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets._2D
{
    public class TimerBar : MonoBehaviour
    {
		public enum TimerBarState
		{
			Normal,
			QuickTotUpReduce
		}

		[SerializeField] private MonoBehaviour gameController;
		[SerializeField] private Image _fillImage;
		[SerializeField] private Slider _slider;
		[SerializeField] private float _fastReductionMultiple;

		private AudioManager _audioManager;
		private float _fullTime;
		private float _currentTime;
		private float _visibleTime;
		private bool _paused;
		private bool _internalPaused = false;
		private TimerBarState _state;
		private bool _countdownSounds;

		private Vector2 startPos;
		private Vector2 destinationPos;
		private float travelTime;
		private float timePassed;
		private bool moving;
		private float _barAlpha = 1.0f;

		// Use this for initialization
		private void Start()
		{
			_audioManager = AudioManager.GetAudioManager();
		}

		private void Awake()
		{
			_paused = false;
			_internalPaused = false;
			_state = TimerBarState.Normal;
			_barAlpha = _fillImage.color.a;
			_fillImage.gameObject.GetComponent<Visibility>().SetInitialAlpha(_barAlpha);
		}

		public void SetTimerTime(float time)
		{
			_fullTime = time;
			_currentTime = time;
			_visibleTime = time;
			PositionBar();
			_state = TimerBarState.Normal;
			SetBarAlpha(_barAlpha);
		}

		public void FadeIn(float time)
		{
			_fillImage.gameObject.GetComponent<Visibility>().PulseFadeIn(time, 1.6f);
		}
		
		public void PulseFade(float time)
		{
			_fillImage.gameObject.GetComponent<Visibility>().PulseFade(time, 2.0f);
		}
		
		public void SetCountdownSounds(bool countdownSounds)
		{
			_countdownSounds = countdownSounds;
		}

		public void SetPaused(bool paused)
		{
			_paused = paused;
		}

		public void SetPausedVisibleAndInternal(bool paused)
		{
			_paused = paused;
			_internalPaused = paused;
		}

		private void PositionBar()
		{
			_slider.value = _visibleTime / _fullTime;
			//SetBarAlpha(_barAlpha);
		}

		public void SetBarAlpha(float a)
		{
			Color c = _fillImage.color;
			c.a = a;
			_fillImage.color = c;
		}

		public void ReduceToZeroInGivenTime(float time)
		{
			_fastReductionMultiple = _currentTime/time;
			_state = TimerBarState.QuickTotUpReduce;
		}

		public float GetFractionOfTimeLeft()
		{
			return _currentTime / _fullTime;
		}
		
		public float GetTimeUsed()
		{
			return _fullTime - _currentTime;
		}
		
		public void ShowNormalTime()
		{
			_state = TimerBarState.Normal;
		}

		private void Update()
		{
			int secondsBefore, secondsAfter;

			// _currentTime is used for the 'internal' clock. It keeps ticking down regardless of the visible state of the timer bar.
//			if (/*!_paused && */_currentTime > 0)
			if (!_internalPaused && _currentTime > 0)
			{
				secondsBefore = (int)_currentTime;
				_currentTime -= Time.deltaTime;
				secondsAfter = (int)_currentTime;

				if ((_state == TimerBarState.Normal) && _countdownSounds && (secondsBefore != secondsAfter) && (secondsBefore <= 10))
				{
					if (_audioManager)
						_audioManager.PlayAudioClip("timerCountdown");
					_fillImage.gameObject.GetComponent<Visibility>().Flash (4,0.05f,0.05f,true);
				}

				if (_currentTime < 0)
				{
					// time has run out
					SetBarAlpha(0f);
					_currentTime = 0;
					if (_state == TimerBarState.Normal && !_paused)
						gameController.SendMessage("TimerExpired");
				}

				if (_state == TimerBarState.Normal && !_paused)
				{
					_visibleTime = _currentTime;
					PositionBar();
				}
			}

			// _visibleTime is the length of the timer bar on the screen. It can be reduced quickly during the tot up.
			if (_state == TimerBarState.QuickTotUpReduce)
			{
				if (!_paused && _visibleTime > 0)
				{
					_visibleTime -= Time.deltaTime*_fastReductionMultiple;
					PositionBar();
				}
			}
		}
    }
}
