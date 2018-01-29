
using System;
using System.Collections.Generic;
using System.Linq;
using MonoMac.Foundation;
using MonoMac.AppKit;

namespace LexisNexis.Red.Mac
{
	public partial class AnnocationsViewController : MonoMac.AppKit.NSViewController
	{
		#region Constructors

		// Called when created from unmanaged code
		public AnnocationsViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public AnnocationsViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public AnnocationsViewController () : base ("AnnocationsView", NSBundle.MainBundle)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new AnnocationsView View {
			get {
				return (AnnocationsView)base.View;
			}
		}
	}
}

