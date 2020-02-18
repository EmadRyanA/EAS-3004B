using UnityEngine;
>>>>>>> 1340a0937943f54a00200a9e0e4f49a9acd5aec5

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
}
