using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31;
using System.Runtime.InteropServices;
using AOT;


#if UNITY_IOS

namespace Prime31
{
	public class VideoTextureManager : AbstractManager
	{
		// Fired when a call to requestAccessToCamera completes. The bool indicates if access was granted.
		public static event Action<bool> requestAccessToCameraCompleteEvent;

		// Fired when a video recording completes successfully. Includes the full path to the video file.
		public static event Action<string> videoRecordingDidSucceedEvent;

		// Fired when a video recording fails. Includes the error returned by the AVAssetWriter on the native side.
		public static event Action<string> videoRecordingDidFailEvent;


		private static Dictionary<int, VideoTexture> videoTextureInstances = new Dictionary<int, VideoTexture>();
		private delegate void nativeTextureIdChangedForVideoDelegate( int videoTextureInstanceId, int newTextureId );



		[DllImport("__Internal")]
		private static extern void _liveTextureSetNativeTextureIdChangedForVideoDelegate( nativeTextureIdChangedForVideoDelegate callback );

		static VideoTextureManager()
		{
			AbstractManager.initialize( typeof( VideoTextureManager ) );

			if( Application.platform == RuntimePlatform.IPhonePlayer )
				_liveTextureSetNativeTextureIdChangedForVideoDelegate( nativeTextureIdChangedForVideo );
		}


		[MonoPInvokeCallback( typeof( nativeTextureIdChangedForVideoDelegate ) )]
		private static void nativeTextureIdChangedForVideo( int videoTextureInstanceId, int newTextureId )
		{
#if UNITY_PRO_LICENSE
			if( videoTextureInstances.ContainsKey( videoTextureInstanceId ) )
				videoTextureInstances[videoTextureInstanceId].texture.UpdateExternalTexture( new IntPtr( newTextureId ) );
#endif
		}


		void requestAccessToCameraComplete( string result )
		{
			if( requestAccessToCameraCompleteEvent != null )
				requestAccessToCameraCompleteEvent( result == "1" );
		}


		private void videoRecordingDidSucceed( string path )
		{
			if( videoRecordingDidSucceedEvent != null )
				videoRecordingDidSucceedEvent( path );
		}


		private void videoRecordingDidFail( string error )
		{
			if( videoRecordingDidFailEvent != null )
				videoRecordingDidFailEvent( error );
		}


		public static void registerInstance( int instanceId, VideoTexture instance )
		{
			videoTextureInstances[instanceId] = instance;
		}


		public static void deRegisterInstance( int instanceId )
		{
			if( videoTextureInstances.ContainsKey( instanceId ) )
				videoTextureInstances.Remove( instanceId );
		}


		private void videoDidStart( string instanceIdString )
		{
			var instanceId = int.Parse( instanceIdString );
			if( videoTextureInstances.ContainsKey( instanceId ) )
				videoTextureInstances[instanceId].onStarted();
		}


		private void videoDidFinish( string instanceIdString )
		{
			var instanceId = int.Parse( instanceIdString );
			if( videoTextureInstances.ContainsKey( instanceId ) )
				videoTextureInstances[instanceId].onComplete();
		}


		private void videoCurrentPresentationTime( string instanceAndPresTime )
		{
			var parts = instanceAndPresTime.Split( new char[] { '-' } );
			var instanceId = int.Parse( parts[0] );
			var presTime = float.Parse( parts[1] );

			if( videoTextureInstances.ContainsKey( instanceId ) )
				videoTextureInstances[instanceId].videoPresentationTimeUpdated( presTime );
		}

	}

}
#endif
