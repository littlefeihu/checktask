
using System;
using System.IO;
using Foundation;
using AppKit;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public partial class TermConditionsPanelController : NSWindowController
	{
		#region Constructors

		// Called when created from unmanaged code
		public TermConditionsPanelController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public TermConditionsPanelController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public TermConditionsPanelController (CGPoint location) : base ("TermConditionsPanel")
		{
			Window.SetFrameOrigin(location);
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new TermConditionsPanel Window {
			get {
				return (TermConditionsPanel)base.Window;
			}
		}

		public override void WindowDidLoad ()
		{
			Window.BackgroundColor = NSColor.White;
			ContentTextView.EnclosingScrollView.BorderType = NSBorderType.NoBorder;
			base.WindowDidLoad ();
			Window.Title = "Teams and Conditions";
			string filePath = NSBundle.MainBundle.PathForResource("/Images/Setting/Termsandconditions","doc");
			NSData fileData = NSData.FromFile (filePath);

			NSDictionary docAttributeo = new NSDictionary();
			var content = new NSAttributedString (fileData, out docAttributeo);
			//var newContent = new NSAttributedString (SettingsUtil.Instance.GetTermsAndConditions ());
			ContentTextView.TextStorage.SetString(content);
		}
	}
}

