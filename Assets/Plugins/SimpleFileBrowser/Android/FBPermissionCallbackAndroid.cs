#if !UNITY_EDITOR && UNITY_ANDROID
>>>>>>> 1340a0937943f54a00200a9e0e4f49a9acd5aec5
using System.Threading;
using UnityEngine;

namespace SimpleFileBrowser
{
	public class FBPermissionCallbackAndroid : AndroidJavaProxy
	{
		private object threadLock;
		public int Result { get; private set; }

		public FBPermissionCallbackAndroid( object threadLock ) : base( "com.yasirkula.unity.FileBrowserPermissionReceiver" )
		{
			Result = -1;
			this.threadLock = threadLock;
		}

		public void OnPermissionResult( int result )
		{
			Result = result;

			lock( threadLock )
			{
				Monitor.Pulse( threadLock );
			}
		}
	}
}
#endif
