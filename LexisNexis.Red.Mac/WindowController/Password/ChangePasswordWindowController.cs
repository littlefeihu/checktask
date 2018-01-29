
using System;
using System.Threading.Tasks;

using Foundation;
using AppKit;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.BusinessModel;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public partial class ChangePasswordWindowController : AppKit.NSWindowController
	{
		#region Constructors

		// Called when created from unmanaged code
		public ChangePasswordWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public ChangePasswordWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public ChangePasswordWindowController (CGPoint location) : base ("ChangePasswordWindow")
		{
			//Window.SetFrameOrigin (location);
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		#region properties
		//strongly typed window accessor
		public new ChangePasswordWindow Window {
			get {
				return (ChangePasswordWindow)base.Window;
			}
		}

		public bool IsRequestCancel {get; set;}
		public bool IsChangeSuccess {get; set;}
		#endregion

		#region Action
		partial void CloseWindow (NSObject sender)
		{
			IsChangeSuccess = false;
			Window.Close();
		}

		partial void CancelButtonClick (NSObject sender)
		{
			IsChangeSuccess = false;
			Window.Close();
		}

		async partial void SendButtonClick (NSObject sender)
		{
			PasswordChangeEnum changeResponse = await ChangePassword(newerPwdTF.StringValue, 
				retypeNewPwdTF.StringValue);
			
			if (IsRequestCancel) {
				IsRequestCancel = false;
				return;
			}

			SetControlsState(true);

			AlertSheet.RunChangePasswordAlert(Window, changeResponse);

			if (changeResponse == PasswordChangeEnum.ChangeSuccess) {

				IsChangeSuccess = true;
				Window.OrderOut(null);
				Window.Close();

			}else {
				IsChangeSuccess = false;
			}

		}

		async Task<PasswordChangeEnum> ChangePassword (string passwordOnde, string passwordTwo)
		{
			var changeResponse = await LoginUtil.Instance.ChangePassword (passwordOnde, passwordTwo);
			return changeResponse;
		}

		#endregion

		#region methods
		public override void AwakeFromNib()
		{
			Window.TitlebarAppearsTransparent = true;
			Window.Title = "LexisNexis Red";
			Window.TitleVisibility = NSWindowTitleVisibility.Hidden;

			Window.StandardWindowButton (NSWindowButton.ZoomButton).Hidden = true;
			var titleBarView = Window.StandardWindowButton (NSWindowButton.CloseButton).Superview;
			titleBarView.WantsLayer = true;
			titleBarView.Layer.BackgroundColor = Utility.ColorWithRGB(237,28,36,1.0f).CGColor;  //(203,15,34,1.0f)

			TopView.WantsLayer = true;
			TopView.Layer.BackgroundColor = Utility.ColorWithRGB(237,28,36,1.0f).CGColor;  //(203,15,34,1.0f)

			LogoImageView.Image = Utility.ImageWithFilePath ("/Images/Login/LexisNexis-Logo.png");

			BottomView.WantsLayer = true;
			BottomView.Layer.BackgroundColor = Utility.ColorWithRGB(240,240,240,1.0f).CGColor;

			sendButton.Enabled = false;

			newerpwdLabel.Cell.StringValue = "New password:";
			newerPwdTF.Cell.PlaceholderString = "Please enter new password";
			retypepwdLabel.Cell.PlaceholderString = "Retype New password";
			retypeNewPwdTF.Cell.PlaceholderString = "Please re-type password";
			cancelButton.Title = "Cancel";
			sendButton.Title = "Submit";

			newerPwdTF.Changed += (sender, e) => handleTextDidChange ((NSNotification)sender);
			retypeNewPwdTF.Changed += (sender, e) => handleTextDidChange ((NSNotification)sender);
		}

		void handleTextDidChange(NSNotification obj)
		{
			sendButton.Enabled = (newerPwdTF.StringValue.Length>0&&retypeNewPwdTF.StringValue.Length>0)?true:false;
		}

		void SetControlsState (bool enanble) {
			retypeNewPwdTF.Enabled = enanble;
			newerPwdTF.Enabled = enanble;
			sendButton.Enabled = enanble;
		}

		#endregion


	}
}

