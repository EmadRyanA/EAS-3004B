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
<<<<<<< HEAD
}
=======
}
>>>>>>> 1340a0937943f54a00200a9e0e4f49a9acd5aec5
