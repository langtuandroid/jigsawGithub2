//using UnityEngine;
//using System.Collections;
//
//namespace UnityStandardAssets._2D
//{
//	public class Crosshairs : MonoBehaviour 
//	{
//		[SerializeField] private Do3DTouch touchController;
//		[SerializeField] private IndicatorRing ring;
//		[SerializeField] private GameObject segment;
//		[SerializeField] private float responseTime = 1.0f;
//		Visibility vis = null;
//		Rect hotspot;
//		int hotspotMinLayer, hotspotMaxLayer;
//		int currentLayer = -1;
//		float responseTimer = 0.0f;
//		bool foundTarget = false;
//		bool hoverSelect = true;
//		bool directionIndicatorActive = false;
//		float directionIndicatorDistance = 100f;
//		private CallBack foundTargetCallback = null;
//		private AudioManager _audioManager;
//
//		// Use this for initialization
//		void Start () {
//			vis = GetComponent<Visibility>();
//			Reset ();
//			_audioManager = AudioManager.GetAudioManager();
//			segment.GetComponent<Visibility>().SineFade (0.8f, 0.2f, 100000f, 0.25f);
//		}
//
//		void Reset()
//		{
//			foundTarget = false;
//			responseTimer = 0.0f;
//			segment.SetActive(false);
//			transform.position = new Vector3(float.MinValue,float.MinValue,0f);	// put it right out of the way
//		}
//
//		public void SetHoverSelect(bool hover)
//		{
//			hoverSelect = hover;
//		}
//
//		public void SetDirectionIndicatorActive(bool active)
//		{
//			directionIndicatorActive = active;
//		}
//
//		public void SetDirectionIndicatorDistance(float distance)
//		{
//			directionIndicatorDistance = distance;
//		}
//
//		private Vector2 PositionCrosshairs()
//		{
//			// position the crosshairs
//			Vector2 pos = touchController.GetTouchPositionInScreenSpace();
//			pos += touchController.zoomOffset;
//			transform.position = pos;
//			return pos;
//		}
//
//		// Update is called once per frame
//		void Update ()
//		{
//			if (!foundTarget)
//			{
//				if (touchController.PlayerHasPushed() && !hoverSelect)
//				{
//					Debug.Log ("**** PLAYER HAS PUSHED ***");
//					touchController.ResetPushDetection();
//					StartCoroutine(PulseCrosshairs());
//					Vector2 pos = PositionCrosshairs();
//					Debug.Log ("pos "+pos.x+", "+pos.y);
//					Debug.Log ("rect "+hotspot.x+", "+hotspot.y+", "+hotspot.width+", "+hotspot.height);
//
//					if (hotspot.Contains(pos))
//					{
//						Debug.Log ("hotspot.Contains");
//
//						foundTarget = true;
//						StartCoroutine(FoundWallySequence());
//					}
//				}
//				else
//				{
//					float depth = touchController.GetTouchDepth();
//					if (depth > 0f)
//					{
//						// position the crosshairs
//						Vector2 pos = PositionCrosshairs();
//						vis.SetAlpha(depth);
//
//						// are the crosshairs over the hotspot?
//						if (CursorInHotspot(pos) && hoverSelect)
//						{
//							responseTimer += Time.deltaTime;
//							if (responseTimer > responseTime)
//							{
//								ring.Show();
//								if (ring.IsFull())
//								{
//									foundTarget = true;
//									StartCoroutine(FoundWallySequence());
//								}
//							}
//						}
//						else
//						{
//							HideRing ();
//						}
//
//						if (directionIndicatorActive)
//							UpdateDirectionIndicator();
//					}
//					else
//					{
//						HideRing ();
//						vis.Hide ();
//						segment.SetActive(false);
//					}
//				}
//
//
//			}
//		}
//
//		public void SetCurrentLayer(int layer)
//		{
//			currentLayer = layer;
//		}
//
//		bool CursorInHotspot(Vector2 pos)
//		{
//			if (hotspot.Contains(pos))
//			{
//				if (((hotspotMinLayer < 0) || (currentLayer >= hotspotMinLayer))
//				    && ((hotspotMaxLayer < 0) || (currentLayer <= hotspotMaxLayer)))
//					return true;
//			}
//
//			return false;
//		}
//
//		void UpdateDirectionIndicator()
//		{
//			float indicatorActivationRange = directionIndicatorDistance;
//			const float indicatorDeActivationRange = 16.0f;
//			float distSquaredToHotspot = DistanceSquaredToHotSpot(transform.position);
//			if ((distSquaredToHotspot < (indicatorActivationRange*indicatorActivationRange))
//			    && (distSquaredToHotspot > (indicatorDeActivationRange*indicatorDeActivationRange)))
//			{
//				segment.SetActive(true);
//
//				float directionToHotSpot = DirectionToHotSpot(transform.position) + Mathf.PI;
//
//				int quadrant = (int)(directionToHotSpot/(Mathf.PI/2));
//				float angleDegrees = ((quadrant+1)%4)*90f;
//				segment.transform.localRotation = Quaternion.Euler(0,0,angleDegrees);
//			}
//			else
//			{
//				segment.SetActive(false);
//			}
//		}
//
//		float DistanceSquaredToHotSpot(Vector3 pos)
//		{
//			Vector2 hsp = hotspot.center;
//			Vector2 offset = new Vector2(hsp.y-pos.y, hsp.x-pos.x);
//			float distanceSquared = offset.sqrMagnitude;
//			return distanceSquared;	
//		}
//
//		float DirectionToHotSpot(Vector3 pos)
//		{
//			Vector2 hsp = hotspot.center;
//			float angle = Mathf.Atan2 (hsp.y-pos.y, hsp.x-pos.x);
//			return angle;
//		}
//
//		public void SetCallback(CallBack callback)
//		{
//			foundTargetCallback = callback;
//		}
//
//		public void HideRing()
//		{
//			foundTarget = false;
//			responseTimer = 0.0f;
//			ring.Hide();
//		}
//
//		public void SetHotspot(Rect hs)
//		{
//			Reset();
//			hotspot = hs;
//		}
//
//		public void SetHotspotLayer(int minLayer, int maxLayer)
//		{
//			hotspotMinLayer = minLayer;
//			hotspotMaxLayer = maxLayer;
//		}
//
//		IEnumerator PulseCrosshairs()
//		{
//			vis.Pulse (1.5f);
//			yield return new WaitForSeconds(0.5f);
//		}
//
//
//		IEnumerator FoundWallySequence() 
//		{
//			if (_audioManager)
//				_audioManager.PlayAudioClip("quizType7RingBarComplete");	
//			vis.Flash (3,0.1f,0.1f,false);
//			ring.Flash (3,0.1f,0.1f,false);
//			yield return new WaitForSeconds(0.5f);
//
//			if (foundTargetCallback != null)
//				foundTargetCallback();
//		}
//	}
//}
