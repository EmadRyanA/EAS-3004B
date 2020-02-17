<<<<<<< HEAD
﻿using UnityEngine;

namespace SimpleFileBrowser
{
	[RequireComponent( typeof( RectTransform ) )]
	public class ListItem : MonoBehaviour
	{
		public object Tag { get; set; }
		public int Position { get; set; }

		private IListViewAdapter adapter;

		internal void SetAdapter( IListViewAdapter listView )
		{
			this.adapter = listView;
		}

		public void OnClick()
		{
			if( adapter.OnItemClicked != null )
				adapter.OnItemClicked( this );
		}
	}
=======
﻿using UnityEngine;

namespace SimpleFileBrowser
{
	[RequireComponent( typeof( RectTransform ) )]
	public class ListItem : MonoBehaviour
	{
		public object Tag { get; set; }
		public int Position { get; set; }

		private IListViewAdapter adapter;

		internal void SetAdapter( IListViewAdapter listView )
		{
			this.adapter = listView;
		}

		public void OnClick()
		{
			if( adapter.OnItemClicked != null )
				adapter.OnItemClicked( this );
		}
	}
>>>>>>> 431160aa739fa61569cf147d0576d59f0d0da843
}