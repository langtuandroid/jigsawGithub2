using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Globalization;
using System;
using System.IO;

/*
* 1. IncompatibleOS for all platforms except iOS and everything bellow iOS 9.0
* 2. Unknown - is returned if you request TouchForceState too early (Awake of first scene is bad idea)
* 3. Unavailable - for all devices that do not support it (at the moment of writting everything except iPhone 6s/6s+ and iPad Pro) and for devices that have Force Touch disabled in accessibility settings.
* 4. Available - for devices that have hardware to support Force Touch and have it enabled in Accessibility Settin
*/
public enum ForceTouchState
{
	Available,
	Unavailable,
	Unknown,
	IncompatibleOS
}

/*
 * Struct used to handle native touch data
 */
public struct NativeTouch
{
	public int id;
	public Vector2 pos;
	public Vector2 delta;
	public TouchPhase phase;
	public float force;
	public float maxforce;
	public float radius;
	public float radiusTolerance;

	public override string ToString()
	{
		return string.Format ("id: {0}, pos: {1}, force: {2}, maxForce: {3}, radius: {4}, radiusTolerance: {5}", id, pos, force, maxforce, radius, radiusTolerance);
	}
}

public class ForceTouchPlugin : MonoBehaviour
{
	#if UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void startTracking(float unityScreenSize);

	[DllImport ("__Internal")]
	private static extern void stopTracking();

	[DllImport ("__Internal")]
	private static extern int getForceTouchState();

	[DllImport ("__Internal")]
	private static extern float getScaleFactor(float unityScreenSize);

	[DllImport ("__Internal")]
	private static extern int getNativeTouches(out IntPtr dataPtr);

	[DllImport ("__Internal")]
	private static extern void setCallbackMethod (string gameObject, string methodName);

	[DllImport ("__Internal")]
	private static extern void removeCallbackMethod ();

	[DllImport ("__Internal")]
	private static extern bool supportsTouchRadius ();
	#endif

	private static bool isTracking = false;
	private static List<NativeTouch> touches;
	
	private const int TOUCH_BINARY_SIZE = 36;
 
	/*
	 * Returns wheter native side of the plugin is currently tracking touches.
	 */
	public static bool IsTracking()
	{
		#if UNITY_EDITOR
			return false;
		#elif UNITY_IOS
			return isTracking;
		#else
			return false;
		#endif
	}

	/*
	 * Retrieves touch data from native code.
	 */ 
	public static List<NativeTouch> GetNativeTouches()
	{
		if (touches == null)
			touches = new List<NativeTouch> ();
		else
			touches.Clear ();

		#if UNITY_EDITOR
			//do nothing
		#elif UNITY_IOS
			//Create a memory pointer
			IntPtr unmanagedPtr;

			//Receive binary data from native code. Native code also allocates memory for this data.
			int size = getNativeTouches(out unmanagedPtr);

			//if touches are available
			if (size > 0)
			{
				//copy binary data from unamanged memory to managed memory
				byte[] managedData = new byte[size];
				Marshal.Copy(unmanagedPtr, managedData, 0, size);

				int touchCount = (int)size/TOUCH_BINARY_SIZE; //each touch is serialized into 36 bytes of binary data
				for (var i = 0; i < touchCount; i++)
				{

					var temp = new NativeTouch()
					{
						id = BitConverter.ToInt32(managedData, i * TOUCH_BINARY_SIZE),
						pos = new Vector2(BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 4), BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 8)),
						delta = new Vector2(BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 12), BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 16)),
						force = BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 20),
						maxforce = BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 24),
						radius = BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 28),
						radiusTolerance = BitConverter.ToSingle(managedData, i * TOUCH_BINARY_SIZE + 32)
					};

					touches.Add(temp);
				}

				//release unamanaged memory
				Marshal.FreeHGlobal(unmanagedPtr);
			}
		#else
			//do nothing
		#endif
		return touches;
	}

	/*
	 * Force native code to capture input data. It's ok to call this method multiple times.
	 */
	public static void StartTracking()
	{
		#if UNITY_EDITOR
			//
		#elif UNITY_IOS
			isTracking = true;		
			startTracking(Mathf.Max(Screen.width, Screen.height));
		#else
			//
		#endif
	}

	/*
	 * Force native code to stop capturing data. It's ok to call this method multiple times.
	 */
	public static void StopTracking()
	{
		#if UNITY_EDITOR
			//
		#elif UNITY_IOS
			isTracking = false;
			stopTracking();
		#else
			//
		#endif
	}

	/*
	 * Retrieve iOS device scale factor. Though plugin returns
	 * touch data in full screen resolution, iOS operates using low DPI space
	 * and using scale factor you can transform touch data back to 
	 * iOS space. For non retina devices this will be 1; for retina devices
	 * this will be 2; There's an exception for iPhone 6 Plus and iPhone 6s Plus,
	 * where native code sees screen size as 2208x1242, input area resolution
	 * is 736x413, but Unity sees it as 1920x1080 (this is hardware resolution
	 * of screen panel used in these devices). Therefore, even though the getScaleFactor
	 * for iPhone 6+/6s+ should be 3 (and it is 3 for iOS apps), for applications
	 * using Metal and Direct3d it is 2,608.
	 */
	public static float GetScaleFactor()
	{
		#if UNITY_EDITOR
			return 1;
		#elif UNITY_IOS
			return getScaleFactor(Mathf.Max(Screen.width, Screen.height));
		#else
			return 1;
		#endif
	}


	/*
	 * Tests device capabilites for ForceTouch. See overload method for more info.
	 * https://developer.apple.com/library/prerelease/ios/documentation/UIKit/Reference/UITouch_Class/#//apple_ref/c/tdef/UIForceTouchCapability
	 */
	public static ForceTouchState GetForceTouchState()
	{
		#if UNITY_EDITOR
			return ForceTouchState.IncompatibleOS;
		#elif UNITY_IOS
			return GetForceTouchState(getForceTouchState());
		#else
			return ForceTouchState.IncompatibleOS;
		#endif
	}

	/*
	 * Returns current ForceTouchState. Possible values:
	 * 1. IncompatibleOS for all platforms except iOS and everything bellow iOS 9.0
     * 2. Unknown - is returned if you request TouchForceState too early (Awake of first scene is bad idea)
	 * 3. Unavailable - for all devices that do not support it (at the moment of writting everything except iPhone 6s/6s+ and iPad Pro) and for devices that have Force Touch disabled in accessibility settings.
	 * 4. Available - for devices that have hardware to support Force Touch and have it enabled in Accessibility Settin
	 */
	public static ForceTouchState GetForceTouchState(int state)
	{
		#if UNITY_EDITOR
		return ForceTouchState.IncompatibleOS;
		#elif UNITY_IOS
		switch(state)
		{
		case 1:
			return ForceTouchState.Available;
		case 2:
			return ForceTouchState.Unavailable;
		case 3:
			return ForceTouchState.Unknown;
		case 4:
			return ForceTouchState.IncompatibleOS;
		default:
			return ForceTouchState.IncompatibleOS;
		}
		#else
		return ForceTouchState.IncompatibleOS;
		#endif
	}

	/*
	 * Register callback method to be triggered when user changes ForceTouch 
	 * settings in Accessibility settings. GameObject has to be in hierarchy and method has to have signature:
	 * public void MethodName(string message). See InputManager.cs for example
	 * Only last method/GO subscribed will be called.
	 */
	public static void SetCallbackMethod(string GameObjectName, string MethodName)
	{
		#if UNITY_EDITOR
		// 
		#elif UNITY_IOS
			setCallbackMethod(GameObjectName, MethodName);
		#else
		//
		#endif
	}

	/*
	 * Remove callback for ForceTouch state change event
	 */
	public static void RemoveCallbackMethod()
	{
		#if UNITY_EDITOR
		// 
		#elif UNITY_IOS
			removeCallbackMethod();
		#else
		//
		#endif
	}

	/*
	 * Returns whether current device supports touch radius.
	 * True for iOS 8 and above
	 * False for all others
	 */
	public static bool SupportsTouchRadius()
	{
		#if UNITY_EDITOR
		return false;
		#elif UNITY_IOS
			return supportsTouchRadius();
		#else
		return false;
		#endif
	}

}