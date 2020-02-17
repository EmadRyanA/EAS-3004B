<<<<<<< HEAD
﻿#if !UNITY_EDITOR && UNITY_ANDROID
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
=======
﻿#if !UNITY_EDITOR && UNITY_ANDROID
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
>>>>>>> 431160aa739fa61569cf147d0576d59f0d0da843
#endif