
using System;
using Foundation;
using AppKit;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.HelpClass;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public partial class LNRedAboutPanelController : AppKit.NSWindowController
	{
		#region Constructors

		// Called when created from unmanaged code
		public LNRedAboutPanelController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public LNRedAboutPanelController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public LNRedAboutPanelController (CGPoint location) : base ("LNRedAboutPanel")
		{
			//Window.SetFrameOrigin(location);
			Window.AnimationBehavior = NSWindowAnimationBehavior.AlertPanel;
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new LNRedAboutPanel Window {
			get {
				return (LNRedAboutPanel)base.Window;
			}
		}

		public override void WindowDidLoad ()
		{
			base.WindowDidLoad ();


			CGRect frame = Window.Frame;
			Window.BackgroundColor = NSColor.White;
			ContentTextView.EnclosingScrollView.BorderType = NSBorderType.NoBorder;

			Window.Title = "";
			AboutTitle.StringValue = "About LexisNexis Red";

			string filePath = NSBundle.MainBundle.PathForResource("/Images/Setting/LexisNexisRed","doc");
			NSData fileData = NSData.FromFile (filePath);

			NSDictionary docAttributeo = new NSDictionary();
			var content = new NSAttributedString (fileData, out docAttributeo);

			ContentTextView.TextStorage.SetString(content);
		}

		public void InitializePanel (CGPoint location)
		{
			Window.SetFrameOrigin(location);
			if (GlobalAccess.Instance.CurrentUserInfo != null) {
				LastSyncTime.StringValue = "Latest server sync " 
				+ SettingsUtil.Instance.GetLastSyncedTime ().ToString ("h:mmtt, ").ToLower()
				+SettingsUtil.Instance.GetLastSyncedTime ().ToString ("d MMM yyyy");
			} else {
				LastSyncTime.StringValue = "";
			}
		}
			
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
		}
	}
}

