
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Foundation;
using AppKit;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Services;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.HelpClass;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public partial class LoginWindowController : NSWindowController
	{
		#region Constructors
		// Called when created from unmanaged code
		public LoginWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public LoginWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public LoginWindowController () : base ("LoginWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			Window.MakeFirstResponder (userMail);
		}
			
		#endregion

		#region properties

		//strongly typed window accessor
		public new LoginWindow Window {
			get {
				return (LoginWindow)base.Window;
			}
		}

		List<Country> regions;
		bool isRequestCancel { get; set;}
		ForgetPasswordWindowController forgetPasswordWCtrl{ get; set;}
		#endregion

		public override void AwakeFromNib ()
		{
			Window.Title = "LexisNexis Red";
			Window.TitleVisibility = NSWindowTitleVisibility.Hidden;
			Window.TitlebarAppearsTransparent = true;

			Window.StandardWindowButton (NSWindowButton.ZoomButton).Hidden = true;
			var titleBarView = Window.StandardWindowButton (NSWindowButton.CloseButton).Superview;
			titleBarView.WantsLayer = true;

			titleBarView.Layer.BackgroundColor = Utility.ColorWithRGB(237,28,36,1.0f).CGColor; //(237,28,36,1.0f) (203,15,34,1.0f)

			TopView.WantsLayer = true;
			TopView.Layer.BackgroundColor = Utility.ColorWithRGB(237,28,36,1.0f).CGColor;

			LogoImageView.Image = Utility.ImageWithFilePath ("/Images/Login/LexisNexis-Logo.png");


			imageView.Image = Utility.ImageWithFilePath ("/Images/Login/loading_gif.gif");
			imageView.Animates = true;
			imageView.Hidden = true;

			comboBox.SetKeyboardFocusRingNeedsDisplay (comboBox.FocusRingMaskBounds);

			userMail.Changed += delegate (object sender, EventArgs e) {
				handleTextDidChange ((NSNotification)sender);
			};

			userPassword.Changed += delegate (object sender, EventArgs e) {
				handleTextDidChange ((NSNotification)sender);
			};
				
			userMail.Cell.PlaceholderString = "name@domain.com";
			userPassword.Cell.PlaceholderString = "Required";

			LoginUserDetails userDetail = LoginUtil.Instance.GetLastUserLoginInfo ();
			if (userDetail!=null && userDetail.Country != null) {
				//int countryIndex = CountryNameByCountryCode (userDetail.Country.CountryCode);
				//comboBox.SelectItemWithTag (countryIndex+1);
				comboBox.Title = userDetail.Country.CountryName;
			}

			resetButton.Enabled = true;
			loginButton.Enabled = false;

			//userMail.StringValue = @"tracy@lexisred.com";
			//userPassword.StringValue = @"Password1";
		}

		int CountryNameByCountryCode(string countryCode)
		{
			List<Country> countryArray = ConfigurationService.GetAllCountryMap ();
			int index = 0;
			foreach (Country region in countryArray) {

				if (region.CountryCode.Equals (countryCode)) {
					break;
				}

				index++;
			}

			return index;
		}

		#region Button Action

		partial void PopupButtonSelectChange (NSObject sender)
		{
			var button = (NSPopUpButton)sender;

			button.Title = button.TitleOfSelectedItem;
		}

		partial void CloseLoginWindow (NSObject sender) 
		{
			mainWindow.Close(); 
		}

		partial void ForgetButtonClick (NSObject sender)
		{
			CGRect frameRect = Window.Frame;
			CGPoint orgPoint = frameRect.Location;
			orgPoint.Y = frameRect.Top-(435-260);

			if (forgetPasswordWCtrl == null) {
				forgetPasswordWCtrl = new ForgetPasswordWindowController(orgPoint);
			}

			var resetWindow = forgetPasswordWCtrl.Window;

		    Window.OrderOut(null);
			forgetPasswordWCtrl.SetCountryCombobox(comboBox.Title);
			forgetPasswordWCtrl.ShowWindow(this);

			resetWindow.WindowShouldClose += t => true;
			resetWindow.WillClose += delegate (object obj, EventArgs e){
				Window.OrderFront(null);
			};
		}

		async partial void RegisterButtonClick (NSObject sender)
		{
			SetControlsState(false);

			//invoke login business logic
			LoginUser user = new LoginUser ();
			user.Email = userMail.StringValue;
			user.Password = userPassword.StringValue;

			regions = ConfigurationService.GetAllCountryMap ();

			foreach (Country region in regions) {

				if (region.CountryName.Equals (comboBox.Title)) {
					user.CountryCode = region.CountryCode;
					break;
				}
			}

			LoginStatusEnum loginResponse = await SignIn (user);

			if (isRequestCancel) {
				isRequestCancel = false;
				return;
			}

			SetControlsState(true);
		
			if (loginResponse != LoginStatusEnum.LoginSuccess) {
				AlertSheet.RunLoginAlert(loginResponse);
			} else {
				
				mainWindow.OrderOut(null);
				mainWindow.Close(); 

				LoginUserDetails userDetail = GlobalAccess.Instance.CurrentUserInfo;

				var appDelegate = (AppDelegate)NSApplication.SharedApplication.Delegate;

				//for test
				//bool isEnter = true;
				//if (isEnter) {

				if (userDetail.NeedChangePassword) {
					CGRect frameRect = Window.Frame;
					CGPoint orgPoint = frameRect.Location;
					orgPoint.Y = frameRect.Top - (315-260);
					//using (var windowController = new ChangePasswordWindowController(orgPoint)) {
					var windowController = new ChangePasswordWindowController(orgPoint);	
					var changeWindow = windowController.Window;
						windowController.ShowWindow(this);

						Window.OrderOut(null);

						changeWindow.WindowShouldClose += t => true;
						changeWindow.WillClose += delegate (object obj, EventArgs e){
							if (windowController.IsChangeSuccess) {
								appDelegate.SwitchWindowByWindowName ("PublicationWindowController");
								appDelegate.SetLogOutMenuItemFullName();
							} else {
								Window.OrderFront(null);
							}
						};
					//}

				} else {
				    
				    appDelegate.SwitchWindowByWindowName ("PublicationWindowController");
					appDelegate.SetLogOutMenuItemFullName();
				}

			}
		}
		#endregion

		#region methods
		async Task<LoginStatusEnum> SignIn (LoginUser user)
		{
			string email = user.Email;
			string password = user.Password;
			string countryCode = user.CountryCode;

			var loginResponse = await LoginUtil.Instance.ValidateUserLogin (email, password, countryCode);
			return loginResponse;
		}

		void SetControlsState (bool enable) {
			imageView.Hidden = enable;
			comboBox.Enabled = enable;
			userMail.Enabled = enable;
			userPassword.Enabled = enable;
			loginButton.Enabled = enable;
			resetButton.Enabled = enable;
		}

		// -------------------------------------------------------------------------------
		//	handleTextDidChange:
		//
		//	The text in NSTextField has changed, try to attempt type completion.
		// -------------------------------------------------------------------------------
		public void handleTextDidChange(NSNotification obj)
		{
			if (userMail.StringValue.Length > 0 && userPassword.StringValue.Length > 0) {
				loginButton.Enabled = true;
			} else {
				loginButton.Enabled = false;
			}
		}

		#endregion

		#region api with class LoginWindow
		public void CancelLogin ()
		{
			SetControlsState(true);
			isRequestCancel = true;
		}

		public void Login ()
		{
			RegisterButtonClick (loginButton);
		}
		#endregion
	}
}

