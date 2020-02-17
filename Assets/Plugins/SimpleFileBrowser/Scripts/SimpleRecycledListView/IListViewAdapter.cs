namespace SimpleFileBrowser
{
	public delegate void OnItemClickedHandler( ListItem item );

	public interface IListViewAdapter
	{
		OnItemClickedHandler OnItemClicked { get; set; }

		int Count { get; }
		float ItemHeight { get; }

		ListItem CreateItem();

		void SetItemContent( ListItem item );
	}
=======
﻿namespace SimpleFileBrowser
{
	public delegate void OnItemClickedHandler( ListItem item );

	public interface IListViewAdapter
	{
		OnItemClickedHandler OnItemClicked { get; set; }

		int Count { get; }
		float ItemHeight { get; }

		ListItem CreateItem();

		void SetItemContent( ListItem item );
	}
>>>>>>> 431160aa739fa61569cf147d0576d59f0d0da843
}
