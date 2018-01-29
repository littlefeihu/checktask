using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Foundation;
using AppKit;
using CoreGraphics;

using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Business;

using LexisNexis.Red.Common.Services;


namespace LexisNexis.Red.Mac
{
	public partial class ForgetPasswordWindowController : NSWindowController
	{
		public nint countryIndex { get; set; }
		public ForgetPasswordWindowController (IntPtr handle) : base (handle)
		{
		}

		[Export ("initWithCoder:")]
		public ForgetPasswordWindowController (NSCoder coder) : base (coder)
		{
		}

		public ForgetPasswordWindowController (CGPoint location) : base ("ForgetPasswordWindow")
		{
			//Window.SetFrameOrigin (location);
		}

		public override void AwakeFromNib ()
		{
			base.AwakeFromNib ();

			Window.TitlebarAppearsTransparent = true;
			Window.Title = "LexisNexis Red";
			Window.TitleVisibility = NSWindowTitleVisibility.Hidden;
			Window.StandardWindowButton (NSWindowButton.ZoomButton).Hidden = true;
			var titleBarView = Window.StandardWindowButton (NSWindowButton.CloseButton).Superview;
			titleBarView.WantsLayer = true;
			titleBarView.Layer.BackgroundColor = Utility.ColorWithRGB(237,28,36,1.0f).CGColor; //(203,15,34,1.0f)

			TopView.WantsLayer = true;
			TopView.Layer.BackgroundColor = Utility.ColorWithRGB(237,28,36,1.0f).CGColor;      //(203,15,34,1.0f)

			LogoImageView.Image = Utility.ImageWithFilePath ("/Images/Login/LexisNexis-Logo.png");

			BottomView.WantsLayer = true;
			BottomView.Layer.BackgroundColor = Utility.ColorWithRGB(240,240,240,1.0f).CGColor;

			countryIndex = countryIndex < 0 ? 0 : countryIndex;

			SendButton.Enabled = false;

			EmailTF.Cell.PlaceholderString = "name@domain.com";
			EmailTF.Changed += (sender, e) => handleTextDidChange ((NSNotification)sender);

			InfoLabel.StringValue = "If you don't have an internet connection, " +
				"please call Customer Support on:";

			ServiceTF.StringValue = 
				"Australia - 1800 999 906 \n" +
				"Hong Kong - +852 2179 7888 \n" +
				"Malaysia - 1800-88-8856 \n" +
				"New Zealand - 0800 800 986 \n" +
				"Singapore - +65-6349-0110";

			CancelButton.Title = "Cancel";
			SendButton.Title = "Submit";
		}

		public void SetCountryCombobox (nint index)
		{
			if (index == -1) {
				return;
			}
			nint curIndex = index < 0 ? 0 : index;
			CountryCombobox.SelectItem (curIndex);
		}

		public void SetCountryCombobox (string title)
		{
			if (title == null) {
				return;
			}

			CountryCombobox.Title = title;
		}

		public new ForgetPasswordWindow Window {
			get { return (ForgetPasswordWindow)base.Window; }
		}

		public bool IsRequestCancel {get; set;}
		public bool IsResetSuccess {get; set;}

		void handleTextDidChange(NSNotification obj)
		{
			SendButton.Enabled = EmailTF.StringValue.Length>0?true:false;
		}

		partial void PopupButtonSelectChange (NSObject sender)
		{
			var button = (NSPopUpButton)sender;

			button.Title = button.TitleOfSelectedItem;
		}

		partial void CancelButtonClick (NSObject sender)
		{
			IsResetSuccess = false;
			Window.Close();
		}

//		partial void CloseWindow (NSObject sender)
//		{
//			IsResetSuccess = false;
//			Window.Close();
//		}

		async partial void SendButtonClick (NSObject sender)
		{
			SetControlsState(false);
			LoginUser user = new LoginUser ();
			user.Email = EmailTF.StringValue;

			List<Country> regions = ConfigurationService.GetAllCountryMap ();

			foreach (Country region in regions) {
				if (region.CountryName.Equals (CountryCombobox.Title)) {
					user.CountryCode = region.CountryCode;
					break;
				}
			}

			PasswordResetEnum resetResponse = await ResetPassword(user);
			if (IsRequestCancel) {
				IsRequestCancel = false;
				return;
			}

			SetControlsState(true);

			AlertSheet.RunResetPasswordAlert(Window, resetResponse);

			if (resetResponse == PasswordResetEnum.ResetSuccess) {

				IsResetSuccess = true;
				Window.OrderOut(null);
				Window.Close();

			}else {
				IsResetSuccess = false;
			}
		}

		async Task<PasswordResetEnum> ResetPassword (LoginUser user)
		{
			string email = user.Email;
			string countryCode = user.CountryCode;

			var loginResponse = await LoginUtil.Instance.ResetPassword (email, countryCode);
			return loginResponse;
		}

		void SetControlsState (bool enanble) {
			CountryCombobox.Enabled = enanble;
			EmailTF.Enabled = enanble;
			SendButton.Enabled = enanble;
		}
	}
}
