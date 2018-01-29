
using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.HelpClass;
using CoreGraphics;
using WebKit;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Mac
{
	public partial class LNLegalPanelController : AppKit.NSWindowController
	{
		#region Constructors

		// Called when created from unmanaged code
		public LNLegalPanelController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public LNLegalPanelController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public LNLegalPanelController (CGPoint location) : base ("LNLegalPanel")
		{
			Window.SetFrameOrigin (location);
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed window accessor
		public new LNLegalPanel Window {
			get {
				return (LNLegalPanel)base.Window;
			}
		}
			
		public override void WindowDidLoad ()
		{
			base.WindowDidLoad ();

			Window.BackgroundColor = NSColor.White;
			ContentTextView.EnclosingScrollView.BorderType = NSBorderType.NoBorder;

			Window.Title = "";
			AboutTitle.StringValue = "About Lexisnexis Legal & Professional";
			if (GlobalAccess.Instance.CurrentUserInfo!=null) {
				LastSyncTime.StringValue = "Latest server sync " + SettingsUtil.Instance.GetLastSyncedTime ().ToString ("HH:mmtt dd MMM yyyy");
			} else {
				LastSyncTime.StringValue = "";
			}


			LoginUserDetails userDetail = GlobalAccess.Instance.CurrentUserInfo;
			if (userDetail == null) {
				string filePath = NSBundle.MainBundle.PathForResource("/Images/Setting/LexisNexisLegal","htm");
				NSData fileHtmData = NSData.FromFile (filePath);

				NSDictionary docAttribute = new NSDictionary();
				var htmContent = NSAttributedString.CreateWithHTML (fileHtmData, out docAttribute);
					//new NSAttributedString (fileHtmData, out docAttribute);

				ContentTextView.TextStorage.SetString (htmContent);
				return;
			}
				
			var htmlString = SettingsUtil.Instance.GetLexisNexisInfo ();
			htmlString = htmlString.Replace ("font-size:12px;", "font-size:12px;\n font-family: \"HelveticaNeue-Medium\";");
			//htmlString = htmlString.Replace ("line-height:16px;", "line-height:14pt;");
			htmlString = htmlString.Replace ("<h3>\nLexisNexis Legal & Professional \n</h3>", "");
			htmlString = htmlString.Replace ("color:#000000;", "color:#808085;");  //# 808085 6D6D72
			var nsString = new NSString (htmlString);
			var fileData = nsString.Encode (NSStringEncoding.UTF8);
			NSDictionary docAttributeo = new NSDictionary();
			var url = new NSUrl ("");
			var content = new NSAttributedString (fileData, url, out docAttributeo);
			ContentTextView.TextStorage.SetString(content);

		}
	}
}

