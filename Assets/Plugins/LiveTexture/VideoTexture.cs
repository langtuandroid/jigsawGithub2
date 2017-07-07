using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;


#if UNITY_IOS

namespace Prime31
{
	public class VideoTexture
	{
		// If true, the Texture2D will be automatically destroyed for you and all resources freed. If false, it is your responsibility to call
		// stop to destory the Texture2D.
		public bool autoDestructOnCompletion = true;

		// The video texture. This is valid immediately after creating a VideoTexture instance. Use it like you would any other Texture2D.
		public Texture2D texture { get; private set; }

		// Fired when a video texture begins playing
		public Action videoDidStartEvent;

		// Fired when a video texture is done playing
		public Action videoDidFinishEvent;

		// Fired when the presentation time of the video changes. Useful for syncing with audio playback on the Unity side
		public Action<float> videoPresentationTimeUpdatedEvent;


		private static int _instanceIdCounter = 1;
		private int _instanceId;
		private AudioSource _syncedAudioSource;
		private float _audioSourceAllowedVariance = 0.2f;



		[DllImport("__Internal")]
		private static extern void _liveTextureStartVideoTexturePlayback( int instanceId, string filename, int textureId, bool shouldLoop, float startTime );

		[DllImport("__Internal")]
		private static extern void _liveTextureStartMetalVideoTexturePlayback( int instanceId, string filename, IntPtr texture, bool shouldLoop, float startTime );

		[DllImport("__Internal")]
		private static extern void _liveTextureFirePresentationTimeUpdatedEvents( int instanceId, bool shouldFireEvents );

		[DllImport("__Internal")]
		private static extern void _liveTexturePauseVideoTexturePlayback( int instanceId );

		[DllImport("__Internal")]
		private static extern void _liveTextureUnpauseVideoTexturePlayback( int instanceId );

		[DllImport("__Internal")]
		private static extern void _liveTextureStopVideoTexturePlayback( int instanceId );

		[DllImport("__Internal")]
		private static extern void _liveTextureSetGainForTextureInstance( int instanceId, float gain );



		// Constructor. Used to create a video texture and register it with the VideoTextureManager.
		public VideoTexture( string filename, int width, int height, bool shouldLoop = false, float startTime = 0 )
		{
			_instanceId = _instanceIdCounter++;

	        // Create texture that will be updated in the plugin code
			var texFormat = LiveTextureBinding.isUsingMetalAPI ? TextureFormat.BGRA32 : TextureFormat.ARGB32;
			texture = new Texture2D( width, height, texFormat, false );

	        if( Application.platform == RuntimePlatform.IPhonePlayer )
	        {
				if( LiveTextureBinding.isUsingMetalAPI )
					_liveTextureStartMetalVideoTexturePlayback( _instanceId, filename, texture.GetNativeTexturePtr(), shouldLoop, startTime );
				else
					_liveTextureStartVideoTexturePlayback( _instanceId, filename, texture.GetNativeTextureID(), shouldLoop, startTime );
			}

			VideoTextureManager.registerInstance( _instanceId, this );
		}


		// Toggles whether the VideoTexture should fire the videoPresentationTimeUpdatedEvent
	    public void firePresentationTimeUpdatedEvents( bool shouldFireEvents )
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_liveTextureFirePresentationTimeUpdatedEvents( _instanceId, shouldFireEvents );
	    }


		// Pass in an AudioSource that has a clip with the same duration of the video file and it will play along with the video
		public void syncAudioSource( AudioSource audioSource, float allowedTimeVariance = 0.2f )
		{
			firePresentationTimeUpdatedEvents( true );
			_syncedAudioSource = audioSource;
			_audioSourceAllowedVariance = allowedTimeVariance;
		}


		// Pauses the video texture
	    public void pause()
	    {
			if( _syncedAudioSource != null )
				_syncedAudioSource.Pause();

	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_liveTexturePauseVideoTexturePlayback( _instanceId );
	    }


		// Unpauses the video texture
	    public void unpause()
	    {
			if( _syncedAudioSource != null )
				_syncedAudioSource.Play();

	        if( Application.platform == RuntimePlatform.IPhonePlayer )
				_liveTextureUnpauseVideoTexturePlayback( _instanceId );
	    }


		// Stops and releases the video texture player. The Texture2D will be destroyed automatically for you
	    public void stop()
	    {
	        if( Application.platform == RuntimePlatform.IPhonePlayer )
			{
				_liveTextureStopVideoTexturePlayback( _instanceId );
				cleanup();
			}
	    }


		// Sets the gain for the instance clamped from 0 to 1
		public void setGain( float gain )
		{
			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_liveTextureSetGainForTextureInstance( _instanceId, Mathf.Clamp01( gain ) );
		}


		private void cleanup()
		{
			if( _syncedAudioSource != null )
				_syncedAudioSource.Stop();
			_syncedAudioSource = null;

			UnityEngine.Object.Destroy( texture );
			texture = null;
			VideoTextureManager.deRegisterInstance( _instanceId );
		}


		// called internally
		public void onStarted()
		{
			if( _syncedAudioSource != null )
			{
				_syncedAudioSource.time = 0f;
				_syncedAudioSource.Play();
			}

			if( videoDidStartEvent != null )
				videoDidStartEvent();
		}


		// called internally
		public void onComplete()
		{
			if( _syncedAudioSource != null )
				_syncedAudioSource.Stop();

			if( videoDidFinishEvent != null )
				videoDidFinishEvent();

			// if we are set to autodestruct do so
			if( autoDestructOnCompletion )
				stop();
		}


		// called internally
		public void videoPresentationTimeUpdated( float presentationTime )
		{
			// if we have a synced AudioSource, keep it in sync if it strays from the allowed variance
			if( _syncedAudioSource != null )
			{
				if( Mathf.Abs( _syncedAudioSource.time - presentationTime ) > _audioSourceAllowedVariance )
					_syncedAudioSource.time = presentationTime;
			}

			if( videoPresentationTimeUpdatedEvent != null )
				videoPresentationTimeUpdatedEvent( presentationTime );
		}

	}

}
#endif
