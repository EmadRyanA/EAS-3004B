#if !UNITY_EDITOR && UNITY_ANDROID
using UnityEngine;

namespace SimpleFileBrowser
{
	public class FBDirectoryReceiveCallbackAndroid : AndroidJavaProxy
	{
		private readonly FileBrowser.DirectoryPickCallback callback;
		private readonly FBCallbackHelper callbackHelper;

		public FBDirectoryReceiveCallbackAndroid( FileBrowser.DirectoryPickCallback callback ) : base( "com.yasirkula.unity.FileBrowserDirectoryReceiver" )
		{
			this.callback = callback;
			callbackHelper = new GameObject( "FBCallbackHelper" ).AddComponent<FBCallbackHelper>();
		}

		public void OnDirectoryPicked( string rawUri, string name )
		{
			callbackHelper.CallOnMainThread( () => DirectoryPickedCallback( rawUri, name ) );
		}

		private void DirectoryPickedCallback( string rawUri, string name )
		{
			try
			{
				if( callback != null )
					callback( rawUri, name );
			}
			finally
			{
				Object.Destroy( callbackHelper );
			}
		}
	}
<<<<<<< HEAD
}
#endif
=======
}
>>>>>>> 1340a0937943f54a00200a9e0e4f49a9acd5aec5
