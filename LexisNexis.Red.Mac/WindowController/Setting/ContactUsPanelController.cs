using System;

using Foundation;
using AppKit;
using CoreGraphics;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.HelpClass;
using System.Text;

namespace LexisNexis.Red.Mac
{
	public partial class ContactUsPanelController : NSWindowController
	{
		public ContactUsPanelController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public ContactUsPanelController (NSCoder coder) : base (coder)
		{
		}

		public ContactUsPanelController (CGPoint location) : base ("ContactUsPanel")
		{
			Window.SetFrameOrigin (location);
			Window.BackgroundColor = NSColor.White;
			Window.MakeFirstResponder (null);
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();


			LoginUserDetails userDetail = GlobalAccess.Instance.CurrentUserInfo;
			if (userDetail == null) {
				return;
			}
			string telephone = userDetail.Country.ContactUs.Phone;
			string worktime = userDetail.Country.ContactUs.WorkingHours;
			string internalCall = userDetail.Country.ContactUs.InternationalCallers;
			string email = userDetail.Country.ContactUs.Email;
			string faxPhone = userDetail.Country.ContactUs.Fax;
			string postUs = userDetail.Country.ContactUs.PostToUs.Content;
			postUs=postUs.Replace ("      ", "");
			string sendDx = userDetail.Country.ContactUs.SendByDX;
			sendDx=sendDx.Replace ("      ", "");

			var attributeString = new NSMutableAttributedString ("");
			attributeString.Append (Utility.AttributedTitle (telephone,
				Utility.ColorWithHexColorValue ("#000000", 1.0f),"System",14,NSTextAlignment.Left));
			attributeString.Append (Utility.AttributedTitle ("\n"+worktime,
				Utility.ColorWithHexColorValue ("#000000", 0.85f),"System",13,NSTextAlignment.Left));
			attributeString.Append (Utility.AttributedTitle ("\nPhone us",
				Utility.ColorWithHexColorValue ("#666666", 0.85f),"System",13,NSTextAlignment.Left));
			PhoneNumberTF.AttributedStringValue = attributeString;
			TeleImage.Image = Utility.ImageWithFilePath ("/Images/Setting/Phone_Icon@1x.png");

			attributeString = new NSMutableAttributedString ("");
			attributeString.Append (Utility.AttributedTitle (internalCall,
				Utility.ColorWithHexColorValue ("#000000", 1.0f),"System",14,NSTextAlignment.Left));
			attributeString.Append (Utility.AttributedTitle ("\nInternational callers",
				Utility.ColorWithHexColorValue ("#666666", 0.85f),"System",13,NSTextAlignment.Left));
			InternationalCallerTF.AttributedStringValue = attributeString;

			//string emailString = email+"\nE-mail us" ;
//			EmailTF.AllowsEditingTextAttributes = true;
//			attributeString = new NSMutableAttributedString ("");
//			attributeString.Append (Utility.AttributeLinkTitle (email, email, "Contact Us", "Support", 
//				Utility.ColorWithHexColorValue ("#000000",1.0f), 14));
//			attributeString.Append (Utility.AttributedTitle ("\nE-mail us",
//				Utility.ColorWithHexColorValue ("#666666", 0.85f),"System",13,NSTextAlignment.Left));
//			EmailTF.AttributedStringValue = attributeString;
			EmailImage.Image = Utility.ImageWithFilePath ("/Images/Setting/Email_Icon@1x.png");
			EmailButton.Cell.Bordered = false;
			EmailButton.Cell.SetButtonType (NSButtonType.MomentaryChange);
			EmailButton.AttributedTitle = Utility.AttributedTitle (email,
				Utility.ColorWithHexColorValue ("#0080fc", 0.85f),"System",13,NSTextAlignment.Left);
			EmailButton.AttributedAlternateTitle = Utility.AttributedTitle (email,
				Utility.ColorWithHexColorValue ("#0080fc", 1.0f),"System",13,NSTextAlignment.Left);

			attributeString = new NSMutableAttributedString ("");
			attributeString.Append (Utility.AttributedTitle (faxPhone,
				Utility.ColorWithHexColorValue ("#000000", 1.0f),"System",14,NSTextAlignment.Left));
			attributeString.Append (Utility.AttributedTitle ("\nFax us",
				Utility.ColorWithHexColorValue ("#666666", 0.85f),"System",13,NSTextAlignment.Left));
			FaxusTF.AttributedStringValue = attributeString;

			LocationImage.Image = Utility.ImageWithFilePath ("/Images/Setting/Location_Icon@1x.png");
			if (userDetail.Country.CountryCode == "AU" || userDetail.Country.CountryCode == "NZ") {
				attributeString = new NSMutableAttributedString ("");
				attributeString.Append (Utility.AttributedTitle ("LexisNexis Customer Relations\n",
					Utility.ColorWithHexColorValue ("#000000", 1.0f),"System",14,NSTextAlignment.Left));
				attributeString.Append (Utility.AttributedTitle (postUs,
					Utility.ColorWithHexColorValue ("#000000", 1.0f),"System",14,NSTextAlignment.Left));
				attributeString.Append (Utility.AttributedTitle ("\nPost to us",
					Utility.ColorWithHexColorValue ("#666666", 0.85f),"System",13,NSTextAlignment.Left));
				PostusTF.AttributedStringValue = attributeString;
			} else {
				attributeString = new NSMutableAttributedString ("");
				attributeString.Append (Utility.AttributedTitle (postUs,
					Utility.ColorWithHexColorValue ("#000000", 1.0f),"System",14,NSTextAlignment.Left));
				attributeString.Append (Utility.AttributedTitle ("\nPost to us",
					Utility.ColorWithHexColorValue ("#666666", 0.85f),"System",13,NSTextAlignment.Left));
				PostusTF.AttributedStringValue = attributeString;
			}

			if (!string.IsNullOrEmpty (sendDx)&& sendDx.Length >0) {
				attributeString = new NSMutableAttributedString ("");
				attributeString.Append (Utility.AttributedTitle (sendDx,
					Utility.ColorWithHexColorValue ("#000000", 1.0f),"System",14,NSTextAlignment.Left));
				attributeString.Append (Utility.AttributedTitle ("\nSend by DX",
					Utility.ColorWithHexColorValue ("#666666", 0.85f),"System",13,NSTextAlignment.Left));
				SendDX.AttributedStringValue = attributeString;
			}
		}

		public new ContactUsPanel Window {
			get { return (ContactUsPanel)base.Window; }
		}

		partial void EmailButtonClick (NSObject sender)
		{
			string subtitle = EmailButton.Title;

			string mailtoAddress = string.Format("mailto:{0}?Subject={1}&body={2}", subtitle,"Contact Us","Support");

			var uri = new Uri(mailtoAddress);
			var nsurl = new NSUrl(uri.GetComponents(UriComponents.HttpRequestUrl,UriFormat.UriEscaped));

			NSWorkspace.SharedWorkspace.OpenUrl(nsurl);
		}
			
	}
}
