using System;
using AppKit;
using Foundation;

namespace LexisNexis.Red.Mac
{
	public class TocTableViewDataSources : NSTableViewDataSource
	{
		TocViewController viewController { get; set;}
		public TocTableViewDataSources (TocViewController controller)
		{
			viewController = controller;
		}

		public override nint GetRowCount (NSTableView tableView)
		{
			return (viewController==null)||
				viewController.TOCDataManager==null||
				(viewController.TOCDataManager.TOCNodeList==null) ? 
				0 : viewController.TOCDataManager.TOCNodeList.Count;
		}
	}
}

