
using System;
using Foundation;
using AppKit;

namespace LexisNexis.Red.Mac
{
	public partial class ExpandGridTableView : NSTableView
	{
		#region Constructors

		// Called when created from unmanaged code
		public ExpandGridTableView (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ExpandGridTableView (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion



	}
}

