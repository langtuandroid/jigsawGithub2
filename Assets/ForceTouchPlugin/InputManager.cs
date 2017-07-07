using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;
	
	private int mouseId = -1;
	private Vector2 lastMousePos;

	public float nativeTouchScale = 1;
	public List<NativeTouch> touches;
	public bool supportsTouchRadius;
	public ForceTouchState forceTouchState;

	public void Awake()
    {
		instance = this;
		Application.targetFrameRate = 60; //iOS by default runs 30fps


		//retrieve screen scale (iOS operates in lowDPI mode)
		nativeTouchScale = ForceTouchPlugin.GetScaleFactor();
		
		//checks whether current device supports TouchRadius (iOS 8 and above)
		supportsTouchRadius = ForceTouchPlugin.SupportsTouchRadius ();

		//prepare to store touches
		touches = new List<NativeTouch> ();	
    }

	public void Start()
	{
		//during awake of first scene force touch state is not yet available. If InputManager is initialized later it's ok to call this in Awake.
		//For platforms other than iOS ForceTouchState.IncompatibleOS will be returned.
		forceTouchState = ForceTouchPlugin.GetForceTouchState(); 

		//it would be best to only start tracking if forceTouchState is Available or if supportsTouchRadius is true.
		//If native touches are used on old ios devices or devices that have force touch disabled it's a performance penalty that could be avoided.
		//if (forceTouchState == ForceTouchState.Available)
		{
			ForceTouchPlugin.StartTracking (); //won't be executed on anything but iOS so no need to add compiler conditionals
			//AddCallback();
		}
	}

	/*
	 * Method that will be called by native end if ForceTouch state changes. You must
	 * subscribe to that event by calling AddCallback method. For this method to be called,
	 * InputManager component has to be in hierarchy. 
	 * See AddCallbackMethod in ForceTouchPlugin.cs for more info
	 */
	public void UpdateForceTouch(string message)
	{
		if (GetComponent<StatusUI>()) //this is only required for StatusUI.
			GetComponent<StatusUI>().callbackMessage = message;
			
		forceTouchState = ForceTouchPlugin.GetForceTouchState (Int32.Parse(message));
	}
	
	/*
	 * Method used to subscribe this component as event handler for ForceTouch state change (use car disable/enable ForceTouch/3D touch in accessibility settings).
	 */
	public void AddCallback()
	{
		ForceTouchPlugin.SetCallbackMethod(this.name, "UpdateForceTouch"); //won't be executed on anything but iOS so no need to add compiler conditionals
	}
	
	/* 
	 * Removes this component as event handler for ForceTouch state change event. 
	 */ 
	public void RemoveCallback()
	{
		ForceTouchPlugin.RemoveCallbackMethod();
	}


	/* 
	 * Refresh touch data.
	 */
	public void Update () 
	{	
		if (GetComponent<StatusUI>()) //this is only required for StatusUI.
			GetComponent<StatusUI>().currentInput = -1;
			
		if (ForceTouchPlugin.IsTracking()) //this will give false for all platforms except iOS. On iOS it will give true if StartTracking was called. 
		{
			/*
			 * If you don't need to handle touch state, you can simply replace the code bellow with the following code:
			 * touches.Clear();
			 *
			 * touches.AddRange(nativeTouches);
			 *
			 * if (GetComponent<StatusUI>()) //this is only required for StatusUI.
			 * 		GetComponent<StatusUI>().currentInput = 1;
			 */
			
			UpdateNativeTouches();

			var nativeTouches = ForceTouchPlugin.GetNativeTouches();
			for (var i = 0; i < nativeTouches.Count; i++)
			{
				HandleNativeInput(nativeTouches[i]);
			}
			
			if (GetComponent<StatusUI>()) //this is only required for StatusUI.
				GetComponent<StatusUI>().currentInput = 1;
		}
		else if (Input.touchCount > 0) //if native touch isn't available fallback to unity touch
		{
			touches.Clear();

			for (int i=0; i<Input.touchCount; i++) 
			{
				var t = Input.touches [i];
				HandleInput (t.fingerId, t.phase, t.position, t.deltaPosition);
			}

			if (GetComponent<StatusUI>()) //this is only required for StatusUI.
				GetComponent<StatusUI>().currentInput = 2;
		}
		else //if unity touch is unavailable fallback to mouse input
		{
			touches.Clear();

			var mousePos = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
			if(Input.GetMouseButtonDown(0))
				HandleInput(mouseId, TouchPhase.Began, mousePos, Vector2.zero);
			else if(Input.GetMouseButtonUp(0))
				HandleInput(mouseId, TouchPhase.Ended, mousePos, mousePos-lastMousePos);
			else if(Input.GetMouseButton(0))
			{
				var delta = mousePos-lastMousePos;
				if(delta!=Vector2.zero)
					HandleInput(mouseId, TouchPhase.Moved, mousePos, delta);
				else
					HandleInput(mouseId, TouchPhase.Stationary, mousePos, delta);
			}
			else
				return;

			lastMousePos = Input.mousePosition;

			if (GetComponent<StatusUI>()) //this is only required for StatusUI.
				GetComponent<StatusUI>().currentInput = 3;
		}
	}

	/* 
	 * You can remove this code if you don't rely on touch phase in your code.
	 *
	 * Code used to handle touch states for native touches. 
	 * 
 	 * Removes touches that were marked as ended in previous frame. All other touches 
	 * are marked as Ended. Touches that are available in this frame will be updated once 
	 * more to reflect their correct state. Those that won't be updated, will remain as ended
	 * and get removed when this method is executed next frame.
	 */
	private void UpdateNativeTouches()
	{
		for (int i = touches.Count - 1; i>=0; i--) 
		{
			if (touches[i].phase == TouchPhase.Ended)
				touches.RemoveAt(i);
			else
			{
				var temp = touches[i];
				temp.phase = TouchPhase.Ended;
				touches[i] = temp;
			}
		}
	}

	/*
	 * You can remove this code if you don't rely on touch phase in your code.
	 * 
	 * Add Native Touches to the list.
	 */
	private void HandleNativeInput(NativeTouch touch)
	{
		for (int i = 0; i < touches.Count; i++) 
		{
			if (touches[i].id == touch.id)
			{
				//if touch delta isn't zero - this touch moved since last frame, otherwise it remained stacionary
				touch.phase = (touch.delta != Vector2.zero) ? TouchPhase.Moved : TouchPhase.Stationary;
				touches[i] = touch;
				return;
			}
		}

		//if touch wasn't found on the list, it must have just began
		touch.phase = TouchPhase.Began;
		touches.Add (touch);
	}
	
	/*
	 * Add UnityTouch and Mouse events to the list
	 */
	private void HandleInput (int pointerId, TouchPhase phase, Vector2 position, Vector2 deltaPosition)
	{
        touches.Add (new NativeTouch ()
		{
			id = pointerId,
			pos = position,
			delta = deltaPosition,
			phase = phase,
			force = 1.0f,
			maxforce = -1f,
			radius = -1,
			radiusTolerance = 0
		});
	}



}