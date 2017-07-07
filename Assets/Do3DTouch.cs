//using UnityEngine;
//using UnityEngine.EventSystems;
//using System.Collections;
//using UnityEngine.UI;
//
//namespace UnityStandardAssets._2D
//{
//	public class Do3DTouch : MonoBehaviour {
//
//		public enum Modes3DTouch
//		{
//			PictureZoom,
//			ButtonAfterTouch,
//			quizType5
//		};
//
//		[SerializeField] public Modes3DTouch mode = Modes3DTouch.PictureZoom;
//		[SerializeField] private EncouragementManager encouragementManager;
//		[SerializeField] public Vector2 zoomOffset = Vector2.zero;
//
//		private SettingsManager _settingsManager;
//		private quizType6Settings gameplaySettings;
//		bool touching = false;
//		int touchCount = 0;
//		int fingerID = 0;
//		Vector2 touchPosition;
//		Vector3 originalPicPos;
//		bool gotOriginalPicPos = false;
//		Color originalColor;
//		float touchStartTime;
//		float maxTouchDepth;
//		bool hardSelect = false;
//		ScratchablePlane scratchablePlane = null;
//		float touchZoomFactor = 2.0f;
//		float touchZoomPressureCutoff = 1.0f;
//		bool touchZoomActive = true;
//
//		float lastpForce = 0f;
//		bool pushDone = false;
//		float highestPush = 0f;
//		float pushTimer = 0f;
//
//
//		// Use this for initialization
//		void Start () 
//		{
//			touching = false;
//			hardSelect = false;
//			gotOriginalPicPos = false;
//
//			_settingsManager = SettingsManager.GetSettingsManager();
//			gameplaySettings = _settingsManager.quizType6Settings;
//
//			if (mode == Modes3DTouch.ButtonAfterTouch)
//			{
//				Button b = gameObject.GetComponent<Button>();
//				ColorBlock cb = b.colors;
//				originalColor = cb.normalColor;
//				originalColor.a = 1.0f;
//				//originalColor = cb.pressedColor;
//			}
//
//			scratchablePlane = gameObject.GetComponent<ScratchablePlane>();
//		}
//
//
//		public void SetZoomParameters(float inTouchZoomFactor, float inTouchZoomPressureCutoff)
//		{
//			touchZoomFactor = inTouchZoomFactor;
//			touchZoomPressureCutoff = inTouchZoomPressureCutoff;
//		}
//
//
//		public void SetTouchZoomActive(bool active)
//		{
//			touchZoomActive = active;
//		}
//
//		// TODO: It gets a bit confused with multiple touches.
//		// really what I need to do here is maintain a list of touches. Any new ones go on the end and that is the touch in control.
//		// Any time I have a touchup event, I remove that touch from the list.
//		// I'll need to hold the system touchID with each touch, so that I can recognise it when I get a touchup event, and delete it using that.
//		// Note that the touchID in BaseEventData is different from the touchID used in the 3D touch system. Hence I need
//		// so search for new touches in update, using the position of a new touch.
//		public void TouchDown(BaseEventData data)
//		{
//			if (touchCount == 0)
//			{
//				touchStartTime = Time.realtimeSinceStartup;
//				maxTouchDepth = 0f;
//				hardSelect = false;
//			}
//
//			touchCount++;
//			touching = true;
//			PointerEventData pointerData = data as PointerEventData;
//
//			// force this new touch to be the monitored touch
//			fingerID = 0;	//pointerData.pointerId;
//			touchPosition = pointerData.pressPosition;
//		}
//
//		public void TouchUp(BaseEventData data)
//		{
//			Debug.Log ("TouchUp count = "+touchCount+", mode = "+mode);
//			touchCount--;
//			if (touchCount == 0)
//			{
//				touching = false;
//				//PointerEventData pointerData = data as PointerEventData;
//				fingerID = 0;	//pointerData.pointerId;
//			}
//
//			switch(mode)
//			{
//			case Modes3DTouch.ButtonAfterTouch:
//				ScaleColorOtherButtons(0f);
//				ScaleColorThisButton(0f);
//				break;
//			case Modes3DTouch.PictureZoom:
//				if (touchZoomActive)
//					ApplyForcePicture(0f, Vector2.zero);
//				break;
//			case Modes3DTouch.quizType5:
//				if (scratchablePlane)
//					scratchablePlane.SetPressure(0f);
//				break;
//			}
//		}
//
//		public void ZoomOut()
//		{
//			if (gotOriginalPicPos)
//				ApplyForcePicture(0f, Vector2.zero);
//		}
//
//		// Update is called once per frame
//		void Update () 
//		{
//			if (!gotOriginalPicPos)
//			{
//				originalPicPos = new Vector3(transform.position.x,transform.position.y);		// doing this here because sometimes it isnt ready in start
//				gotOriginalPicPos = true;
//			}
//
//			if (touching)
//			{
//				var inputTouches = InputManager.instance.touches;
//
//				// try to find the touch in the 3D touch manager
//				if (fingerID == 0)
//				{
//					foreach(var p in inputTouches) 
//					{
//						Vector2 offset = p.pos - touchPosition;
//						if (offset.sqrMagnitude < 100.0f)
//						{
//							fingerID = p.id;
//						}
//					}
//				}
//
//				NativeTouch? touch2 = GetDataOfCurrentTouch();
//				if (touch2 != null)
//				{
//					NativeTouch touch = (NativeTouch)touch2;
//					if (touch.maxforce < 0)
//					{
//						// no 3d touch available
//						//Debug.Log ("no 3d touch available");
//
//						// AGTEMP : allow hard touch in editor
//						ApplyForce (1.0f, touch.pos);
//
//						PushDetection(1f);
//					}
//					else
//					{
//						//Debug.Log ("touch ID = "+p.id+", "+p.maxforce+", "+p.force);
//						float pForce = touch.force/touch.maxforce;
//						float pCompressedForce = pForce / touchZoomPressureCutoff;
//						if (pCompressedForce > 1.0f)
//							pCompressedForce = 1.0f;
//						ApplyForce(pCompressedForce, touch.pos);
//						if (pCompressedForce > maxTouchDepth)
//							maxTouchDepth = pForce;
//
//						PushDetection(pForce);
//					}
//				}
//			}
//			else
//			{
//				PushDetection(0f);
//				//ApplyForce (0f, Vector2.zero);
//			}
//		}
//
//		
//		void PushDetection(float pForce)
//		{
//			// detects rapid applications of force by the player. Like a tap or a click, but done by pushing harder for a brief period.
//			if (pForce < gameplaySettings.pushMinLevel) 
//			{
////				if (lastpForce >= gameplaySettings.pushMinLevel)
////				{
////					// transitioning back to low level.
////					if ((highestPush >= gameplaySettings.pushMaxLevel) && (pushTimer < gameplaySettings.pushTimeLimit))
////					{
////						pushDone = true;
////					}
////				}
//				
//				pushTimer = 0f;
//				highestPush = 0f;
//			}
//			else
//			{
//				if (pForce > highestPush)
//				{
//					highestPush = pForce;
//					if ((highestPush >= gameplaySettings.pushMaxLevel) && (pushTimer < gameplaySettings.pushTimeLimit))
//					{
//						pushDone = true;
//						pushTimer = 10000f;
//					}
//				}
//				
//				pushTimer += Time.deltaTime;
//			}
//			
//			lastpForce = pForce;
//		}
//
///*
//		void PushDetection(float pForce)
//		{
//			// detects rapid applications of force by the player. Like a tap or a click, but done by pushing harder for a brief period.
//			if (pForce < gameplaySettings.pushMinLevel) 
//			{
//				if (lastpForce >= gameplaySettings.pushMinLevel)
//				{
//					// transitioning back to low level.
//					if ((highestPush >= gameplaySettings.pushMaxLevel) && (pushTimer < gameplaySettings.pushTimeLimit))
//					{
//						pushDone = true;
//					}
//				}
//
//				pushTimer = 0f;
//				highestPush = 0f;
//			}
//			else
//			{
//				if (pForce > highestPush)
//				{
//					highestPush = pForce;
//				}
//
//				pushTimer += Time.deltaTime;
//			}
//
//			lastpForce = pForce;
//		}
//*/
//
//		public bool PlayerHasPushed()
//		{
//			return pushDone;
//		}
//
//		public void ResetPushDetection()
//		{
//			pushDone = false;
//			lastpForce = 0f;
//		}
//
//		NativeTouch? GetDataOfCurrentTouch()
//		{
//			if (fingerID != 0)
//			{
//				var inputTouches = InputManager.instance.touches;
//
//				//if touch hasn't been added to display list, one should be added
//				foreach(var p in inputTouches) 
//				{
//					//Debug.Log ("touch ID = "+p.id+", "+p.maxforce+", "+p.force);
//					
//					if (p.id == fingerID)
//					{
//						return p;
//					}
//				}
//			}
//
//			return null;
//		}
//
//		public void ApplyForce(float force, Vector2 pos)
//		{
//			switch(mode)
//			{
//			case Modes3DTouch.PictureZoom:
//				if (touchZoomActive)
//					ApplyForcePicture(force, pos);
//				break;
//			case Modes3DTouch.ButtonAfterTouch:
//				ApplyForceButton(force, pos);
//				break;
//			case Modes3DTouch.quizType5:
//				if (scratchablePlane)
//					scratchablePlane.SetPressure(force);
//				break;
//			}
//		}
//
//
//		Button[] GetOtherButtons()
//		{
//			Button[] allButtons = transform.parent.GetComponentsInChildren<Button>();
//			int numButtons = allButtons.Length;
//
//			if (numButtons > 1)
//			{
//				Button[] otherButtons = new Button[numButtons-1];
//				Button thisButton = gameObject.GetComponent<Button>();
//				int i=0;
//				foreach(Button b in allButtons)
//				{
//					if (b != thisButton)
//					{
//						otherButtons[i++] = b;
//					}
//				}
//
//				return otherButtons;
//			}
//
//			return null;
//		}
//
//		void SetButtonsColourScale(Button[] buttons, Color col, float scale)
//		{
//			if (buttons != null)
//			{
//				foreach(Button b in buttons)
//				{
//					if (b.interactable)
//					{
//						ColorBlock cb = b.colors;
//						cb.pressedColor = col;
//						cb.normalColor = col;
//						cb.disabledColor = col;
//						b.colors = cb;
//
//						Vector3 scaleVec = new Vector3(scale, 1.0f, 1.0f);	//scale, scale);
//						b.transform.localScale = scaleVec;
//					}
//				}
//			}
//		}
//
//
//		void ScaleColorThisButton(float force)
//		{
//			Button b = gameObject.GetComponent<Button>();
//			// force is between 0 and 1
//			float scale = 1f - force*gameplaySettings.touchPressureFactor;	
//			Color col = (gameplaySettings.touchButtonFullColor - originalColor) * force + originalColor;
//			ColorBlock cb = b.colors;
//			Vector3 scaleVec = new Vector3(scale,scale,scale);
//			transform.localScale = scaleVec;
//			cb.pressedColor = col;
//			b.colors = cb;
//		}
//
//		void ScaleColorOtherButtons(float force)
//		{
//			// other buttons
//			Button[] otherButtons = GetOtherButtons();
//			float scaleOther = 1f + Mathf.Pow(force,2)*(gameplaySettings.touchOtherButtonsZoomFactor-1f);
//			Color col2 = (gameplaySettings.touchOtherButtonsFullColor - originalColor) * force + originalColor;
//			SetButtonsColourScale(otherButtons,col2,scaleOther);
//		}
//
//		void HideOtherButtons()
//		{
//			Button[] otherButtons = GetOtherButtons();
//
//			foreach(Button b in otherButtons)
//			{
//				//b.gameObject.SendMessage("Hide");
//				b.gameObject.GetComponent<Visibility>().Hide ();
//				b.interactable = false;
//			}
//		}
//
//		void ApplyForceButton(float force, Vector2 pos)
//		{
//			if (!gameplaySettings.doAfterTouchOnAnswerButtons)
//				return;
//
//			Button b = gameObject.GetComponent<Button>();
//			if (b.interactable)
//			{
//				// force is between 0 and 1
////				float scale = 1f - force*gameplaySettings.touchPressureFactor;
////
////				//Color col = originalColor * (1f - force*gameplaySettings.touchPressureFactor2);
////				Color col = (gameplaySettings.touchButtonFullColor - originalColor) * force + originalColor;
////				ColorBlock cb = b.colors;
//
//				if (force > gameplaySettings.lockLevel)
//				{
//					// Hard select
//					//encouragementManager.ShowEncouragement(EncouragementManager.EncouragementTypes.DoubleDown);
//
//					// lock the button
//					Handheld.Vibrate ();
//					Color col = Color.black;
//					ColorBlock cb = b.colors;
//					cb.disabledColor = col;
//					//b.interactable = false;
//					float scale = (1f - gameplaySettings.lockLevel*gameplaySettings.touchPressureFactor)*gameplaySettings.lockAdditionalScale;
//					Vector3 scaleVec = new Vector3(scale,scale,scale);
//					transform.localScale = scaleVec;
//					cb.pressedColor = col;
//					b.colors = cb;
//
//					HideOtherButtons();
//
//					// do some fireworks
//					Vector3[] buttonPanelCorners = new Vector3[4];
//					transform.parent.GetComponent<RectTransform>().GetWorldCorners(buttonPanelCorners);
//					Vector2 fpos = new Vector2(buttonPanelCorners[0].x,buttonPanelCorners[0].y);
//					Vector2 size = new Vector2(buttonPanelCorners[2].x-fpos.x, buttonPanelCorners[2].y-fpos.y);
//					Rect buttonPanelWorldRect = new Rect(fpos,size);
//					if (FireworksManager._instance)
//						FireworksManager._instance.ManyStarBursts(8,buttonPanelWorldRect,1500f,0.04f);
//
//					hardSelect = true;
//					b.onClick.Invoke();
//				}
//				else
//				{
//					ScaleColorThisButton(force);
//				}
//
////				Vector3 scaleVec = new Vector3(scale,scale,scale);
////				transform.localScale = scaleVec;
////				cb.pressedColor = col;
////				b.colors = cb;
//
//				if (force > 0f)
//				{	
//					// other buttons
//					ScaleColorOtherButtons(force);
////					Button[] otherButtons = GetOtherButtons();
////					float scaleOther = 1f + force*(gameplaySettings.touchOtherButtonsZoomFactor-1f);
////					Color col2 = (gameplaySettings.touchOtherButtonsFullColor - originalColor) * force + originalColor;
////					SetButtonsColourScale(otherButtons,col2,scaleOther);
//				}
//			}
//
////			Color color = gameObject.GetComponent<Button>().transition.PrePressedColor;
////			color *= (1f - force);
////			gameObject.GetComponent<Button>().PressedColor = color;
//		}
//
//		void ApplyForcePicture(float force, Vector2 pos)
//		{
//			// force is between 0 and 1
//			float scale = 1f + force*touchZoomFactor;
//			Vector3 scaleVec = new Vector3(scale,scale,scale);
//			transform.localScale = scaleVec;
//
//			pos += zoomOffset;
//
//
//			if (force > 0f)
//			{
//				gameObject.GetComponent<AspectRatioFitter>().enabled = false;
//				Vector3 offset = new Vector3(pos.x, pos.y) - originalPicPos;
//				Vector3 picpos = /*new Vector3(0f,100f)*(scale-1f) + */ originalPicPos - offset*(scale-1f);	// + new Vector3(force*100,force*100);
//				transform.position = picpos;
//			}
//			else
//			{
//				transform.position = originalPicPos;
//				gameObject.GetComponent<AspectRatioFitter>().enabled = true;
//			}
//		}
//
//		public float GetTouchDepth()
//		{
//			NativeTouch? ntn = GetDataOfCurrentTouch();
//			if (ntn != null)
//			{
//				NativeTouch nt = (NativeTouch)ntn;
//				return nt.force;
//			}
//			return 0f;
//		}
//
//		public Vector2 GetTouchPositionInImage()
//		{
//			NativeTouch? ntn = GetDataOfCurrentTouch();
//			if (ntn != null)
//			{
//				NativeTouch nt = (NativeTouch)ntn;
//				return nt.pos;
//			}
//
//			return Vector2.zero;
//		}
//		
//		public Vector2 GetTouchPositionInScreenSpace()
//		{
//			NativeTouch? ntn = GetDataOfCurrentTouch();
//			if (ntn != null)
//			{
//				NativeTouch nt = (NativeTouch)ntn;
//				return nt.pos;
//			}
//
//			return Vector2.zero;
//		}
//		
//
//
//		void MovePicture(Vector2 offset)
//		{
//
//		}
//
//		public float GetLengthOfTouch()
//		{
//			return Time.realtimeSinceStartup - touchStartTime;
//		}
//
//		public float GetDepthOfTouch()
//		{
//			return maxTouchDepth;
//		}
//
//		public bool GetHardSelect()
//		{
//			return hardSelect;
//		}
//	}
//}
