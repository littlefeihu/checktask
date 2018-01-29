
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;

namespace LexisNexis.Red.Mac
{
	public partial class LNRedAboutPanel : AppKit.NSWindow
	{
		#region Constructors

		// Called when created from unmanaged code
		public LNRedAboutPanel (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public LNRedAboutPanel (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

//		public override NSWindowAnimationBehavior AnimationBehavior ()
//		{
//			return NSWindowAnimationBehavior.AlertPanel;
//		}
		#endregion
	}
}

