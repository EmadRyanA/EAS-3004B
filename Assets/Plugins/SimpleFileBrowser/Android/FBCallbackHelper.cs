#if !UNITY_EDITOR && UNITY_ANDROID
>>>>>>> 1340a0937943f54a00200a9e0e4f49a9acd5aec5
using UnityEngine;

namespace SimpleFileBrowser
{
	public class FBCallbackHelper : MonoBehaviour
	{
		private System.Action mainThreadAction = null;

		private void Awake()
		{
			DontDestroyOnLoad( gameObject );
		}

		private void Update()
		{
			if( mainThreadAction != null )
			{
				System.Action temp = mainThreadAction;
				mainThreadAction = null;
				temp();
			}
		}

		public void CallOnMainThread( System.Action function )
		{
			mainThreadAction = function;
		}
	}
}
#endif
