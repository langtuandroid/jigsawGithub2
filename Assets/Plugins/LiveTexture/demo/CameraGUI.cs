using UnityEngine;
using System.Collections.Generic;
using System;
using Prime31;



namespace Prime31
{
	public class CameraGUI : MonoBehaviourGUI
	{
		public GameObject target; // the object to apply the camera texture to

	    private Texture2D texture; // local reference of the texture
		private bool _isCameraCaptureStarted;
		// this texture is applied when no video is playing
		private Texture2D _yellowTexture;


#if UNITY_IOS
		void Awake()
		{
			_yellowTexture = new Texture2D( 1, 1 );
			_yellowTexture.SetPixel( 0, 0, Color.yellow );
			_yellowTexture.Apply();

			target.GetComponent<Renderer>().sharedMaterial.mainTexture = _yellowTexture;
		}


		void OnGUI()
		{
			beginColumn();


			if( GUILayout.Button( "Request Access to Camera" ) )
			{
				LiveTextureBinding.requestAccessToCamera();
			}


			if( GUILayout.Button( "Start Capture (low)" ) )
			{
				// start the camera capture and use the returned texture
		        texture = LiveTextureBinding.startCameraCapture( false, LTCapturePreset.Size192x144 );
		        target.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
		        LiveTextureBinding.updateMaterialUVScaleForTexture( target.GetComponent<Renderer>().sharedMaterial, texture );
				_isCameraCaptureStarted = true;
			}


			if( GUILayout.Button( "Start Capture (high)" ) )
			{
				texture = LiveTextureBinding.startCameraCapture( false, LTCapturePreset.Size1280x720 );
				target.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
				LiveTextureBinding.updateMaterialUVScaleForTexture( target.GetComponent<Renderer>().sharedMaterial, texture );
				_isCameraCaptureStarted = true;
			}


			if( GUILayout.Button( "Start Capture (ultra high)" ) )
			{
				texture = LiveTextureBinding.startCameraCapture( false, LTCapturePreset.Size1920x1080 );
				target.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
				LiveTextureBinding.updateMaterialUVScaleForTexture( target.GetComponent<Renderer>().sharedMaterial, texture );
				_isCameraCaptureStarted = true;
			}


			if( GUILayout.Button( "Start Capture (front, low)" ) )
			{
		        texture = LiveTextureBinding.startCameraCapture( true, LTCapturePreset.Size192x144 );
		        target.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
		        LiveTextureBinding.updateMaterialUVScaleForTexture( target.GetComponent<Renderer>().sharedMaterial, texture );
				_isCameraCaptureStarted = true;
			}


			if( GUILayout.Button( "Start Capture (front, high)" ) )
			{
		        texture = LiveTextureBinding.startCameraCapture( true, LTCapturePreset.Size640x480 );
		        target.GetComponent<Renderer>().sharedMaterial.mainTexture = texture;
		        LiveTextureBinding.updateMaterialUVScaleForTexture( target.GetComponent<Renderer>().sharedMaterial, texture );
				_isCameraCaptureStarted = true;
			}


			if( GUILayout.Button( "Stop Capture" ) )
			{
		        LiveTextureBinding.stopCameraCapture();
		        texture = null;
				_isCameraCaptureStarted = false;
				target.GetComponent<Renderer>().sharedMaterial.mainTexture = _yellowTexture;
			}


			endColumn( true );


			if( GUILayout.Button( "Start Recording Video" ) )
			{
				LiveTextureBinding.startRecordingCameraOutput( "livetexture.mp4" );
			}


			if( GUILayout.Button( "Stop Recording Video" ) )
			{
				LiveTextureBinding.stopRecordingCameraOutput();
			}


			if( _isCameraCaptureStarted && GUILayout.Button( "Set Exposure Mode" ) )
			{
				LiveTextureBinding.setExposureMode( LTExposureMode.ContinuousAutoExposure );
			}


			if( _isCameraCaptureStarted && GUILayout.Button( "Set Focus Mode" ) )
			{
				LiveTextureBinding.setFocusMode( LTFocusMode.ContinuousAutoFocus );
			}


			endColumn();



			if( bottomRightButton( "Next" ) )
			{
		        LiveTextureBinding.stopCameraCapture();
		        texture = null;

		        Application.LoadLevel( "VideoTestScene" );
			}
		}


		#region MonoBehaviour LiveTexture Cleanup

		void OnApplicationQuit()
		{
	        LiveTextureBinding.stopCameraCapture();
	        texture = null;
		}


		void OnApplicationPause( bool paused )
		{
			if( paused )
			{
		        LiveTextureBinding.stopCameraCapture();
		        texture = null;
			}
		}

		#endregion


		#region Video Recording Events

		void OnEnable()
		{
			VideoTextureManager.requestAccessToCameraCompleteEvent += requestAccessToCameraCompleteEvent;
			VideoTextureManager.videoRecordingDidSucceedEvent += videoRecordingDidSucceedEvent;
			VideoTextureManager.videoRecordingDidFailEvent += videoRecordingDidFailEvent;
		}


		void OnDisable()
		{
			VideoTextureManager.requestAccessToCameraCompleteEvent -= requestAccessToCameraCompleteEvent;
			VideoTextureManager.videoRecordingDidSucceedEvent -= videoRecordingDidSucceedEvent;
			VideoTextureManager.videoRecordingDidFailEvent -= videoRecordingDidFailEvent;
		}


		private void requestAccessToCameraCompleteEvent( bool didAllowAccess )
		{
			Debug.Log( "requestAccessToCameraCompleteEvent. didAllowAccess: " + didAllowAccess );
		}


		private void videoRecordingDidSucceedEvent( string path )
		{
			Debug.Log( "videoRecordingDidSucceedEvent: " + path );

			// save the video to the users photo album
			LiveTextureBinding.saveVideoToSavedPhotosAlbum( path );
		}


		private void videoRecordingDidFailEvent( string error )
		{
			Debug.Log( "videoRecordingDidFailEvent: " + error );
		}

		#endregion

#endif

	}

}
