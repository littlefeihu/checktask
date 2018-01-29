using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace LexisNexis.Red.Mac
{
	public partial class TagsOrgViewController : AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public TagsOrgViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public TagsOrgViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public TagsOrgViewController () : base ("TagsOrgView", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new TagsOrgView View {
			get {
				return (TagsOrgView)base.View;
			}
		}
	}
}
