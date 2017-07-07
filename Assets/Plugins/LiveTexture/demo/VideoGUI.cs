using UnityEngine;
using System.Collections.Generic;
using System;
using Prime31;



namespace Prime31
{
	public class VideoGUI : MonoBehaviourGUI
	{
		// the object to apply the video texture to
		public GameObject targetOne;


#if UNITY_IOS

		private VideoTexture _videoTexture;
		// this texture is applied when no video is playing
		private Texture2D _yellowTexture;


		void Awake()
		{
			_yellowTexture = new Texture2D( 1, 1 );
			_yellowTexture.SetPixel( 0, 0, Color.yellow );
			_yellowTexture.Apply();

			targetOne.GetComponent<Renderer>().material.mainTexture = _yellowTexture;
		}


		void OnGUI()
		{
			beginColumn();

			if( GUILayout.Button( "Start Video Texture" ) )
			{
				// create the video texture. this video texture also illustrates how you can sync an AudioSource with the video
				_videoTexture = new VideoTexture( "crazy-time.mp4", 320, 240 );
				_videoTexture.syncAudioSource( GetComponent<AudioSource>() );

				// apply the texture to a material and set the UVs
				targetOne.GetComponent<Renderer>().material.mainTexture = _videoTexture.texture;
				LiveTextureBinding.updateMaterialUVScaleForTexture( targetOne.GetComponent<Renderer>().sharedMaterial, _videoTexture.texture );

				// add some event handlers
				_videoTexture.videoDidStartEvent = () =>
				{
					Debug.Log( "Video one started" );
				};
				_videoTexture.videoDidFinishEvent = () =>
				{
					// when the video finishes if we are not set to loop this instance is no longer valid
					Debug.Log( "Video one finished" );
					targetOne.GetComponent<Renderer>().sharedMaterial.mainTexture = _yellowTexture;
				};
			}


			if( GUILayout.Button( "Pause" ) )
			{
				// null check in case Stop was pressed which will kill the VideoTexture
				if( _videoTexture != null )
					_videoTexture.pause();
			}


			if( GUILayout.Button( "Unpause" ) )
			{
				// null check in case Stop was pressed which will kill the VideoTexture
				if( _videoTexture != null )
					_videoTexture.unpause();
			}


			if( GUILayout.Button( "Stop" ) )
			{
				// null check in case Stop was pressed which will kill the VideoTexture
				if( _videoTexture != null )
				{
					_videoTexture.stop();
					_videoTexture = null;
				}
			}

			endColumn();


			if( bottomLeftButton( "Back" ) )
			{
				// if the video texture is still playing kill it
				OnApplicationQuit();

		        Application.LoadLevel( "CameraTestScene" );
			}
		}


		void OnApplicationQuit()
		{
	        // if the video texture is still playing kill it
			if( _videoTexture != null )
			{
				_videoTexture.stop();
				_videoTexture = null;
			}
		}


		void OnApplicationPause( bool paused )
		{
			if( paused )
			{
		        // if the video texture is still playing kill it
				if( _videoTexture != null )
				{
					_videoTexture.stop();
					_videoTexture = null;
				}
			}
		}

#endif

	}

}
