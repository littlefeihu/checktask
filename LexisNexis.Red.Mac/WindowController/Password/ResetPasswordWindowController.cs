
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using MonoMac.Foundation;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.Services;


namespace LexisNexis.Red.Mac
{
	public partial class ResetPasswordWindowController : MonoMac.AppKit.NSWindowController
	{
		#region Constructors

		// Called when created from unmanaged code
		public ResetPasswordWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ResetPasswordWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public ResetPasswordWindowController () : base ("ResetPasswordWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		#region
		//strongly typed window accessor
		public new ResetPasswordWindow Window {
			get {
				return (ResetPasswordWindow)base.Window;
			}
		}

		public bool IsRequestCancel {get; set;}
		public bool IsResetSuccess {get; set;}
		#endregion

		public override void AwakeFromNib ()
		{
			//var customView = Window.ContentView;
			//customView.WantsLayer = true;
			//customView.Layer.CornerRadius = 5.0f;

			TopImageView.Image = Utility.ImageWithFilePath("/Images/Login/login_bk_red.png");
			BottonImageView.Image = Utility.ImageWithFilePath ("/Images/Login/login_bk_gray.png");
			LogoImageView.Image = Utility.ImageWithFilePath ("/Images/Login/LexisNexis-Logo.png");
			CloseButton.Image = Utility.ImageWithFilePath ("/Images/Login/loginico_close.png");
			CloseButton.AlternateImage = Utility.ImageWithFilePath ("/Images/Login/loginico_close_press.png");

			CountryCombobox.UsesDataSource = true;
			CountryCombobox.Completes = true;
			CountryCombobox.DataSource = new CountryDataSource ();
			CountryCombobox.SelectItem (0);

			SendButton.Enabled = false;

			EmailTF.Cell.PlaceholderString = "name@domain.com";
			EmailTF.Changed += (sender, e) => handleTextDidChange ((NSNotification)sender);

			InfoLabel.StringValue = "If you don't have an internet connection, " +
				"please contact Customer Support on:";

			ServiceTF.StringValue = "Australia 1800 999 906\n" +
				"Hong Kong 1800 999 906\n" +
				"Malaysia 1800 999 906\n" +
				"New Zealand 0800 800 986\n" +
				"Singapore 1800 9999 906";

			CancelButton.Title = "Cancel";
			SendButton.Title = "Send";
		}

		void handleTextDidChange(NSNotification obj)
		{
			SendButton.Enabled = EmailTF.StringValue.Length>0?true:false;
		}

		partial void CancelButtonClick (NSObject sender)
		{
			IsResetSuccess = false;
			Window.Close();
		}

		partial void CloseWindow (NSObject sender)
		{
			IsResetSuccess = false;
			Window.Close();
		}

		async partial void SendButtonClick (NSObject sender)
		{
			SetControlsState(false);
			LoginUser user = new LoginUser ();
			user.Email = EmailTF.StringValue;

			List<Country> regions = ConfigurationService.GetAllCountryMap ();

			foreach (Country region in regions) {
				if (region.CountryName.Equals (CountryCombobox.StringValue)) {
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

