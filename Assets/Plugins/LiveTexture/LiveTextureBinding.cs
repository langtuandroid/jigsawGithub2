using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;



#if UNITY_IOS
public enum LTCapturePreset
{
	Size192x144, // same as low
	Size640x480,
	Size1280x720,
	Size1920x1080
}

public enum LTExposureMode
{
	Locked = 0,
	AutoExpose = 1,
	ContinuousAutoExposure = 2,
}

public enum LTFocusMode
{
	Locked = 0,
	AutoFocus = 1,
	ContinuousAutoFocus = 2,
}



namespace Prime31
{
	public static class LiveTextureBinding
	{
		private delegate void nativeTextureIdChangedDelegate( int newTextureId );
		private static Texture2D _cameraTexture;
		public static bool isUsingMetalAPI;


		[DllImport("__Internal")]
		private static extern void _liveTextureSetNativeTextureIdChangedDelegate( nativeTextureIdChangedDelegate callback );

		[DllImport("__Internal")]
		private static extern void _liveTextureSetProNotAvailable();

		[DllImport("__Internal")]
		private static extern bool _liveTextureIsUsingMetal();


		static LiveTextureBinding()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				isUsingMetalAPI = _liveTextureIsUsingMetal();
#if !UNITY_PRO_LICENSE
				_liveTextureSetProNotAvailable();
#endif
				_liveTextureSetNativeTextureIdChangedDelegate( nativeTextureIdChanged );
			}
		}


		[MonoPInvokeCallback( typeof( nativeTextureIdChangedDelegate ) )]
		private static void nativeTextureIdChanged( int newTextureId )
		{
#if UNITY_PRO_LICENSE
			if( _cameraTexture )
				_cameraTexture.UpdateExternalTexture( new IntPtr( newTextureId ) );
#endif
		}


		[DllImport("__Internal")]
		private static extern bool _liveTextureRequestAccessToCamera();

		// iOS 7+ only. Results in the requestAccessToCameraCompleteEvent firing with the status.
		public static void requestAccessToCamera()
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_liveTextureRequestAccessToCamera();
		}


		//void _liveTextureSaveVideoToSavedPhotosAlbum( const char* path )
		[DllImport("__Internal")]
		private static extern void _liveTextureSaveVideoToSavedPhotosAlbum( string pathToFile );

		// Saves the video file to the users saved photos album. Note that pathToFile must be a valid path to a video file!
		public static void saveVideoToSavedPhotosAlbum( string pathToFile )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_liveTextureSaveVideoToSavedPhotosAlbum( pathToFile );
		}


	    [DllImport("__Internal")]
	    private static extern bool _liveTextureIsCaptureAvailable();

		// Checks to see if the device has a supported camera and the proper iOS version
	    public static bool isCaptureAvailable()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _liveTextureIsCaptureAvailable();
			return false;
	    }


	    [DllImport("__Internal")]
	    private static extern void _liveTextureStartCameraCapture( bool useFrontCameraIfAvailable, int capturePreset, int textureId );

		[DllImport("__Internal")]
		private static extern void _liveTextureStartMetalCameraCapture( bool useFrontCameraIfAvailable, int capturePreset, IntPtr texture );

		// Starts the camera capture and returns a Texture2D that will have the camera output as it's content
	    public static Texture2D startCameraCapture( bool useFrontCameraIfAvailable, LTCapturePreset capturePreset )
	    {
	    	int width = 0, height = 0;
			switch( capturePreset )
			{
				case LTCapturePreset.Size192x144:
					width = 192;
					height = 144;
					break;
				case LTCapturePreset.Size640x480:
					width = 640;
					height = 480;
					break;
				case LTCapturePreset.Size1280x720:
					width = 1280;
					height = 720;
					break;
				case LTCapturePreset.Size1920x1080:
					width = 1920;
					height = 1080;
					break;
			}

	        // Create texture that will be updated in the plugin code
			var texFormat = LiveTextureBinding.isUsingMetalAPI ? TextureFormat.BGRA32 : TextureFormat.ARGB32;
			_cameraTexture = new Texture2D( width, height, texFormat, false );

			if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				if( isUsingMetalAPI )
					_liveTextureStartMetalCameraCapture( useFrontCameraIfAvailable, (int)capturePreset, _cameraTexture.GetNativeTexturePtr() );
				else
					_liveTextureStartCameraCapture( useFrontCameraIfAvailable, (int)capturePreset, _cameraTexture.GetNativeTextureID() );
			}

			return _cameraTexture;
	    }


	    [DllImport("__Internal")]
	    private static extern void _liveTextureStopCameraCapture();

		// Stops the camera capture and destroys the Texture2D
	    public static void stopCameraCapture()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_liveTextureStopCameraCapture();

			if( _cameraTexture )
			{
				GameObject.Destroy( _cameraTexture );
				_cameraTexture = null;
			}
	    }


		// Updates the materials UV offset to accommodate the texture being placed into the next biggest power of 2 container
		public static void updateMaterialUVScaleForTexture( Material material, Texture2D texture )
		{
			var textureOffset = new Vector2( (float)texture.width / (float)nearestPowerOfTwo( texture.width ), (float)texture.height / (float)nearestPowerOfTwo( texture.height ) );
			material.mainTextureScale = textureOffset;
		}


		private static int nearestPowerOfTwo( float number )
		{
			int n = 1;

			while( n < number )
				n <<= 1;

			return n;
		}


	    [DllImport("__Internal")]
	    private static extern void _liveTextureSetExposureMode( int exposureMode );

		// Sets the exposure mode. Capture must be started for this to work!
	    public static void setExposureMode( LTExposureMode mode )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_liveTextureSetExposureMode( (int)mode );
	    }


	    [DllImport("__Internal")]
	    private static extern void _liveTextureSetFocusMode( int focusMode );

		// Sets the focus mode. Capture must be started for this to work!
	    public static void setFocusMode( LTFocusMode mode )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_liveTextureSetFocusMode( (int)mode );
	    }


	    [DllImport("__Internal")]
	    private static extern bool _liveTextureStartRecordingCameraOutput( string filename );

		// Starts recording the camera output to a file. filename should be just the filename and not a full path. Note that the file will
		// be deleted if it exists. Returns true if the recording was started successfully. Videos are recorded at 640 x 480 using an H.264
		// encoder so a .mov file extension is recommended.
		public static bool startRecordingCameraOutput( string filename )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				return _liveTextureStartRecordingCameraOutput( filename );
			return false;
	    }


		[DllImport("__Internal")]
	    private static extern void _liveTextureStopRecordingCameraOutput();

		// Stops video recording. The VideoTextureManager.videoRecordingDidSucceed/FailEvent will subsequently fire after the file is processed.
		public static void stopRecordingCameraOutput()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_liveTextureStopRecordingCameraOutput();
	    }
	}

}
#endif
