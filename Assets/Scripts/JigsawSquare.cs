using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace UnityStandardAssets._2D
{
	public delegate void JigsawSquarePressCallback(int x, int y, Vector2 touchPos);
	public delegate void JigsawSquareDragCallback(Vector2 touchPos);
	public delegate void JigsawSquareReleaseCallback(Vector2 touchPos);
	public delegate void BombExplodeCallback();

	public class JigsawSquare : MonoBehaviour
    {
		[SerializeField] public Text varNameUI;
		[SerializeField] public Button buttonUI;
		[SerializeField] public Texture _bombTexture;
		[SerializeField] public Texture _bombExtinguishedTexture;
		[SerializeField] public GameObject puffOfSmokePrefab;
		[SerializeField] private GameObject _whiteLayer;
		[SerializeField] public GameObject _correctPositionIndicatorLayer;

		public enum JigsawSquareStates
		{
			Normal = 0,
			Exploding,
			CancelBomb
		}

		private AudioManager _audioManager;
		private JigsawSquarePressCallback _pressedButtonCallback = null;
		private JigsawSquareDragCallback _draggedCallback = null;
		private JigsawSquareReleaseCallback _releasedCallback = null;
		private BombExplodeCallback _bombExplodeCallback = null;
		private string _nameString;
		private bool _alreadyPressed;
		private int gridX, gridY;
		private bool blockRefresh;	// if true then the texture and UV coords will not be refreshed. (used during animations)
		private float timeAtStartOfAnim;
		public JigsawSquareStates state;
		private IEnumerator bombExplodeCoroutine;

		// Use this for initialization
		private void Start()
		{
			_alreadyPressed = false;
			_audioManager = AudioManager.GetAudioManager();
			blockRefresh = false;
		}

		public void SetName(string name)
		{
			_nameString = name;
			varNameUI.text = name;
		}

		public void SetPressedCallback(JigsawSquarePressCallback callback)
		{
			_pressedButtonCallback = callback;
		}

		public void SetDraggedCallback(JigsawSquareDragCallback callback)
		{
			_draggedCallback = callback;
		}
		
		public void SetReleasedCallback(JigsawSquareReleaseCallback callback)
		{
			_releasedCallback = callback;
		}
		
		public void SetGridXY(int x, int y)
		{
			gridX = x;
			gridY = y;
		}

//		public void ClearAfterDelay(float time)
//		{
//			// triggers a keypress after 'time' seconds.
//			StartCoroutine(DelayedKeyPress(time));
//		}

//		IEnumerator DelayedKeyPress(float time)
//		{
//			yield return new WaitForSeconds(time);
//			KeyPressed (false);
//		}

		public void Lockup(bool lockup)
		{
			gameObject.GetComponent<Button>().interactable = !lockup;
			if (!_alreadyPressed)
			{
				Image image = this.GetComponent<Image>();
				image.color = new Color(245.0f/255, 253.0f/255, 255.0f/255);
			}
		}
			
		public void KeyPressed(BaseEventData data)
		{
			RectTransform rt = gameObject.GetComponent<RectTransform>();
			Vector3[] corners = new Vector3[4];
			rt.GetWorldCorners(corners);

			_alreadyPressed = true;

			PointerEventData pointerData = data as PointerEventData;
			Vector2 touchPosition = pointerData.position;
			_pressedButtonCallback(gridX, gridY, touchPosition);
		}


		public void Dragged(BaseEventData data)
		{
			PointerEventData pointerData = data as PointerEventData;
			Vector2 touchPosition = pointerData.position;
			
			_draggedCallback(touchPosition);
		}


		public void TouchRelease(BaseEventData data)
		{
			PointerEventData pointerData = data as PointerEventData;
			Vector2 touchPosition = pointerData.position;

			_releasedCallback(touchPosition);
		}

		public void AnimationCancelBomb()
		{
			CancelAnimations();
			if (_audioManager)
				_audioManager.StopSoundEffect();
			StartCoroutine(BombCancellationAnimSequence());
		}

		IEnumerator BombCancellationAnimSequence()
		{
			if (_audioManager)
				_audioManager.PlayAudioClip("bombExtinguishing");
			
			blockRefresh = true;

			yield return new WaitForSeconds(0.1f);

			RawImage raw = GetComponent<RawImage>();
			for (int f=0; f<5; f++)
			{
				//raw.texture = _bombTexture;
				yield return new WaitForSeconds(0.05f);
				raw.texture = _bombExtinguishedTexture;
				yield return new WaitForSeconds(0.05f);
			}

			Color c = Color.cyan;
			c.a = 0f;
			_whiteLayer.GetComponent<RawImage>().color = c;
			_whiteLayer.GetComponent<Visibility>().FadeIn (0.2f);
			blockRefresh = false;
			yield return new WaitForSeconds(0.2f);
			_whiteLayer.GetComponent<Visibility>().FadeOut (0.5f);

			yield return new WaitForSeconds(0.1f);
			yield return new WaitForSeconds(0.8f);
		}


		public void AnimationBombExplode(BombExplodeCallback callback, float animationTime = 0f)
		{
			state = JigsawSquareStates.Exploding;
			timeAtStartOfAnim = Time.realtimeSinceStartup - animationTime;
			_bombExplodeCallback = callback;
			bombExplodeCoroutine = BombExplodeAnimSequence(animationTime);
			StartCoroutine(bombExplodeCoroutine);
		}

		public float GetAnimationTime()
		{
			return Time.realtimeSinceStartup - timeAtStartOfAnim;
		}

		IEnumerator DelayIfNecessary(float cumulativeDelayTime)
		{
			float currentTime = Time.realtimeSinceStartup - timeAtStartOfAnim;
			if (currentTime < cumulativeDelayTime)
				yield return new WaitForSeconds(cumulativeDelayTime - currentTime);

			yield return null;
		}


		IEnumerator BombExplodeAnimSequence(float currentTime)
		{
			if (_audioManager && currentTime == 0f)
				_audioManager.PlayAudioClip("jigsawBombSwelling");
			blockRefresh = true;

			yield return StartCoroutine(DelayIfNecessary(0.1f));

			GetComponent<ShakeImage>().SetShakeMagnitude(0f);
			GetComponent<ExpandImage>().StartZoom(1.0f, 0.1f, 1.5f, 2.1f, GetAnimationTime());
			//GetComponent<ExpandImage>().StartZoom(1f,1.5f,2.0f);
			Color c = Color.red;
			c.a = 0f;
			_whiteLayer.GetComponent<RawImage>().color = c;
			//_whiteLayer.GetComponent<Visibility>().FadeIn (4.0f);
			_whiteLayer.GetComponent<Visibility>().FadeIn (0.1f, 4.1f, GetAnimationTime());

			yield return StartCoroutine(DelayIfNecessary(1.1f));

			GetComponent<ShakeImage>().SetShakeMagnitude(0.0001f);

			yield return StartCoroutine(DelayIfNecessary(2.0f));

			blockRefresh = false;
			if (_audioManager)
				_audioManager.PlayAudioClip("jigsawBombExplosion");			
			ExplosionParticles();
			_bombExplodeCallback();

			Debug.Log ("After explosion");

			yield return StartCoroutine(DelayIfNecessary(2.1f));

			state = JigsawSquareStates.Normal;
		}

		public void StopExplosion()
		{
			Debug.Log ("StopExplosion");

			StopCoroutine(bombExplodeCoroutine);
			GetComponent<ShakeImage>().SetShakeMagnitude(0f);
			_whiteLayer.GetComponent<Visibility>().FadeOut(0.001f);
			GetComponent<ExpandImage>().StartZoom(1f,1f,0.01f);
			state = JigsawSquareStates.Normal;
			blockRefresh = false;

		}

		public void CancelAnimations()
		{
			Debug.Log ("CancelAnimations");
			if (state == JigsawSquareStates.Exploding)
				StopExplosion();
		}

		void ExplosionParticles()
		{
			// work out the rect (in screen coords) which contains the tile that is exploding
			Vector3[] buttonPanelCorners = new Vector3[4];
			GetComponent<RectTransform>().GetWorldCorners(buttonPanelCorners);
			Vector2 centre = new Vector2((buttonPanelCorners[0].x*1.5f + buttonPanelCorners[3].x*0.5f)/2, (buttonPanelCorners[0].y*0.5f + buttonPanelCorners[1].y*1.5f)/2);
			Vector2 fpos = new Vector2(buttonPanelCorners[0].x, buttonPanelCorners[0].y);
			Vector2 size = new Vector2(buttonPanelCorners[2].x-fpos.x, buttonPanelCorners[2].y-fpos.y);
			
			Rect buttonPanelWorldRect = new Rect(fpos,size);
			// now do the fireworks within that rect.
			if (FireworksManager._instance)
				FireworksManager._instance.ManyStarBursts(14,buttonPanelWorldRect,2450f,0.02f);
		}

		public void PulseBombCount()
		{
			varNameUI.GetComponent<Visibility>().Pulse (0.5f);
		}

		public void Reset()
		{

		}

		public bool RefreshIsBlocked()
		{
			return blockRefresh;
		}
    }
}
