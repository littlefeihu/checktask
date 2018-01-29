using System;
using System.IO;
using System.Collections.Generic;

using Foundation;
using AppKit;
using CoreGraphics;
using SQLite;
using ObjCRuntime;

using LexisNexis.Red.Common;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Mac.Common.Implementation;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Mac.Data;


//using Xamarin;
using System.Net;

namespace LexisNexis.Red.Mac
{
	public class BookEntity
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		[MaxLength(255)]
		public string Content { get; set; }
	}

	public partial class AppDelegate : NSApplicationDelegate
	{
		
		public LoginWindowController loginWindowController;
		public PublicationsWindowController publicationsWindowController;
		//PreferenceWindowController preferenceWindowController;
		//LNRedAboutPanelController aboutRedController;
		NSApplication NSApp = NSApplication.SharedApplication; 
		public bool isMiniFontSize {get; set;}
		public bool isMaxFontSize { get; set;}
		bool isValidateMenuItem { get; set;}
		const string version = "1.11.1";
		const string appName = "LexisNexis Red";

		public AppDelegate ()
		{
		}

		public override bool ApplicationShouldTerminateAfterLastWindowClosed (NSApplication sender)
		{
			return true;
		}

		 public override void DidFinishLaunching (NSNotification notification)
		{
			/* Initialize call should happen as soon as possible, ideally at app start-up. */
//			try {
//				Insights.Initialize ("97cc7007743e0016fc256ebeab93b828e69b85c5", version, appName, Utility.GetAppCacheAbsolutePath(), false);
//			} 
//			catch (Exception exception) { 
//				Insights.Report(exception);
//			}
		
			ServicePointManager.DefaultConnectionLimit = 10;
			IoCContainer.Instance.RegisterInstance<IDevice> (new MacDevices());
			IoCContainer.Instance.RegisterInstance<IDirectory> (new FileDirectory());
			IoCContainer.Instance.RegisterInstance<INetwork> (new MacNetWork ());
			IoCContainer.Instance.RegisterInstance<IPackageFile> (new UnZipFiles ());
			IoCContainer.Instance.RegisterInstance<ICryptogram> (new MacDecodeFile ());
			GlobalAccess.Instance.Init().Wait();

			NSPasteboard.GeneralPasteboard.ClearContents ();
		    

			LoginUserDetails userDetail = GlobalAccess.Instance.CurrentUserInfo;
			if (userDetail != null) {
				SwitchWindowByWindowName ("PublicationWindowController");
				SetLogOutMenuItemFullName ();

			} else {
				SwitchWindowByWindowName ("LoginWindowController");
			}

			//string content = "<h3>this is a test</h3>";
			//await PdfUtil.SaveAsPdf (content, "test");

		}

		public void SwitchWindowByWindowName (string windowName)
		{
			if (windowName.Equals ("LoginWindowController")) {

				var mainwindow = NSApplication.SharedApplication.KeyWindow;
				if (mainwindow!= null && mainwindow.Class.Name == "PublicationContentPanel") {
					mainwindow.Close ();
				}

				if (publicationsWindowController != null) {
					publicationsWindowController.Window.Close ();
					publicationsWindowController = null;
				}

				if (loginWindowController == null) {
					loginWindowController = new LoginWindowController ();
				}

				loginWindowController.Window.MakeKeyAndOrderFront (this);

			} else if (windowName.Equals ("PublicationWindowController")) {

				if (loginWindowController != null) {
					loginWindowController = null;
				}

				if (publicationsWindowController == null) {
					publicationsWindowController = new PublicationsWindowController ();
				}

				//var screenRect = NSScreen.Screens[0].Frame;
				//NSScreen.MainScreen.VisibleFrame;

				//publicationsWindowController.Window.SetFrame (screenRect, true);
				publicationsWindowController.Window.MakeKeyAndOrderFront (this);

				if (SettingsUtil.Instance!= null) {
					var bodyFontSize = SettingsUtil.Instance.GetFontSize ();
					if (bodyFontSize == 0) {
						bodyFontSize = 13;
					}

					if (bodyFontSize <= LNRConstants.ContentFont_MIN) {
						isMiniFontSize = true;
					}else if (bodyFontSize >= LNRConstants.ContentFont_MAX) {
						isMaxFontSize = true;
					}
				}

			}

			validateMenuItem (null);
		}

		#region action
		// Action method binded in MainMenu.xib to "Preferences..." menu item.
		//App menu
		partial void ShowPreferencesWindow (NSObject sender)
		{
			var preferenceWindowController = new PreferenceWindowController ();

			NSWindow window = preferenceWindowController.Window;

			NSApp.RunModalForWindow (window);
			window.OrderOut(null);
		}
			
		partial void ShowAboutLNLegal (NSObject sender)
		{
			CGPoint orgPoint = Utility.GetModalPanelLocation(690.0f, LNRConstants.WindowHeight_MIN); //658+22
			using (NSAutoreleasePool pool = new NSAutoreleasePool ()) {
				
				using (var panelController = new LNLegalPanelController(orgPoint)) {
					var legalWindow = panelController.Window;
					legalWindow.ReleasedWhenClosed = true;
					legalWindow.SetOneShot(true);

					var NSApp = NSApplication.SharedApplication;

					legalWindow.WindowShouldClose += t => true;
					legalWindow.WillClose += delegate (object obj, EventArgs e){
						NSApp.StopModal();
					};

					NSApp.RunModalForWindow(panelController.Window);
				}
			}
		}

		partial void ShowAboutLNRed (NSObject sender)
		{
			CGPoint orgPoint = Utility.GetModalPanelLocation(690.0f, LNRConstants.WindowHeight_MIN);  //658+22
			using (NSAutoreleasePool pool = new NSAutoreleasePool ()) {

				using (var aboutRedController = new LNRedAboutPanelController(orgPoint)) {
					aboutRedController.InitializePanel(orgPoint);
					var aboutWindow = aboutRedController.Window;

					var NSApp = NSApplication.SharedApplication;

					aboutWindow.WindowShouldClose += t => true;
					aboutWindow.WillClose += delegate (object obj, EventArgs e){
						NSApp.StopModal();
					};

					NSApp.RunModalForWindow(aboutWindow);
				};
			}
		}

		partial void TermAndConditions (NSObject sender)
		{
			CGPoint orgPoint = Utility.GetModalPanelLocation(690.0f, LNRConstants.WindowHeight_MIN);  //658+22
			var panelController = new TermConditionsPanelController(orgPoint);
			var termWindow = panelController.Window;

			var NSApp = NSApplication.SharedApplication;

			termWindow.WindowShouldClose += t => true;
			termWindow.WillClose += delegate (object obj, EventArgs e){
				NSApp.StopModal();
			};

			NSApp.RunModalForWindow(panelController.Window);
		}

		partial void LogOutLNRed (NSObject sender)
		{
			string title = "Log Out";
			string info = "Thank you for using LexisNexis Red application. Are you sure you want to exit ?";
			nint result = AlertSheet.RunConfirmAlert(title, info);
			if (result == (int)NSAlertType.DefaultReturn) {
				ConfirmLogout ();
			} else {
				WindowStopModal ();
			}
		}

		void ConfirmLogout ()
		{
			LoginUtil.Instance.Logout();

			WindowStopModal ();

			//Go back to login view

			var appDelegate = (AppDelegate)NSApplication.SharedApplication.Delegate;
			appDelegate.SwitchWindowByWindowName ("LoginWindowController");
		}

		void WindowStopModal ()
		{
			//preferenceWindow.OrderOut(null);
			NSApplication.SharedApplication.StopModal();
		}

		//File menu
		[Export("saveDocumentAs:")]
		async public void saveDocumentAs(NSObject sender)
		{
			if (publicationsWindowController == null ||
				publicationsWindowController.ContentVC == null) {
				return;
			}

			await publicationsWindowController.ContentVC.PageController.SavehtmlToPDF ();
		}

		[Export("print:")]
		public void print(NSObject sender)
		{
			
		}


		partial void PrintPdfDocument (Foundation.NSObject sender)
		{
			publicationsWindowController.ContentVC.PageController.PrintPDF(null);
		}
			
		//view menu
		partial void ActualSize (NSObject sender)
		{
			var mainWindow = NSApplication.SharedApplication.KeyWindow;
			var controller = (PublicationsWindowController)mainWindow.WindowController;
			if (controller.Class.Name == "PublicationsWindowController") {
				controller.ContentVC.FitFontSize();
				isMiniFontSize = false;
				isMaxFontSize = false;
				validateMenuItem(null);
			}
		}

		partial void TextZoomIn (NSObject sender)
		{
			if (isMaxFontSize) {
				validateMenuItem(null);
				return;
			}

			var mainWindow = NSApplication.SharedApplication.KeyWindow;
			var controller = (PublicationsWindowController)mainWindow.WindowController;
			if (controller.Class.Name == "PublicationsWindowController") {
				int fontSize = controller.ContentVC.ZoomInFontSize();
				isMaxFontSize = fontSize>=17?true:false;
				isMiniFontSize = false;
			}
				
			validateMenuItem(null);

			//Console.Write("isMaxFontSize:{0}; isMiniFontSize:{1}\n",isMaxFontSize,isMiniFontSize);
		}

		partial void TextZoomOut (NSObject sender)
		{
			if (isMiniFontSize){
				validateMenuItem(null);
				return;
			}

			var mainWindow = NSApplication.SharedApplication.KeyWindow;
			var controller = (PublicationsWindowController)mainWindow.WindowController;
			if (controller.Class.Name == "PublicationsWindowController") {
				int fontsize = controller.ContentVC.ZoomOutFontSize();
				isMiniFontSize = fontsize<=11?true:false;
				isMaxFontSize = false;
			}
				
			validateMenuItem(null);
			//Console.Write("isMaxFontSize:{0}; isMiniFontSize:{1}\n",isMaxFontSize,isMiniFontSize);
		}

		partial void OrganizePublications (NSObject sender)
		{
			var NSApp = NSApplication.SharedApplication;
			string bookTitle = NSApp.KeyWindow.Title;
			if (bookTitle == "Content") {
				bookTitle = PublicationsDataManager.SharedInstance.CurrentPublication.Name;
			}
			var orgPubsPanelCtl = new OrganizePublicationsPanelController(bookTitle);
			var organizeWindow = orgPubsPanelCtl.Window;
			organizeWindow.WindowShouldClose +=(t)=>true;
			organizeWindow.WillClose += delegate(object obj, EventArgs e){
				int bookID = orgPubsPanelCtl.BookIDDeleted;

				NSApp.EndSheet(organizeWindow);

				if (orgPubsPanelCtl.IsDeletePublication) {
					publicationsWindowController.PublicationsVC.HandleDeletePublictaionByBookID(bookID);
				} else if (orgPubsPanelCtl.IsDraggedPublicaiton) {
					publicationsWindowController.PublicationsVC.HandleSortPublictaions(orgPubsPanelCtl.BookIDArray);
				}
			};

			NSApp.BeginSheet(organizeWindow,NSApp.MainWindow);
		}

		//Help menu
		partial void FAQs (NSObject sender)
		{
		}

		partial void ContactUs (NSObject sender)
		{
			CGPoint orgPoint = Utility.GetModalPanelLocation(560.0f, 502.0f); //480+22
			using (NSAutoreleasePool pool = new NSAutoreleasePool ()) {

				using (var panelController = new ContactUsPanelController(orgPoint)) {
					var contactusWindow = panelController.Window;
					contactusWindow.ReleasedWhenClosed = true;

					var NSApp = NSApplication.SharedApplication;

					contactusWindow.WindowShouldClose += t => true;
					contactusWindow.WillClose += delegate (object obj, EventArgs e){
						NSApp.StopModal();
					};

					NSApp.RunModalForWindow(contactusWindow);
				}
			}
		}
		#endregion

		#region main menu
		public void SetLogOutMenuItemFullName()
		{
			string userFirstName = string.Empty;
			LoginUserDetails userDetail = GlobalAccess.Instance.CurrentUserInfo;
			if (userDetail == null) {
				return;
			}

			if (!string.IsNullOrEmpty(userDetail.FirstName) && 
				!userDetail.FirstName.Equals(" ")) {
				userFirstName = userDetail.FirstName;
			} else {
				userFirstName = userDetail.Email;
			}

			NSMenu sysmenu = NSApplication.SharedApplication.MainMenu;
			NSMenuItem subMenuItem = sysmenu.ItemAt (0);
			NSMenu subMenu = subMenuItem.Submenu;

			NSMenuItem subItem = subMenu.ItemAt (4);
			if (userFirstName != null) {
				string title = "Log Out " + userFirstName ;
				subItem.Title = title;
			}
		}

		[Export("validateMenuItem:")]
		public bool validateMenuItem(NSMenuItem menuItem)
		{
			SetEditMenuStatus ();
			//Console.WriteLine ("validateMenuItem");
			if (isValidateMenuItem) {
				isValidateMenuItem = false;
				return true;
			}

			isValidateMenuItem = true;

			var mainWindow = NSApplication.SharedApplication.MainWindow;
			if (mainWindow == null) {
				mainWindow = NSApplication.SharedApplication.KeyWindow;
				if (mainWindow == null) {
					isValidateMenuItem = false;
					return true;
				}
			}

			// logic to enable / disable goes here...
			string name = mainWindow.Class.Name;
			//Console.WriteLine ("class name:{0}", name);
			if (name == "LoginWindow" || name == "ChangePasswordWindow" || name == "ResetPasswordWindow") {
				SetLogOutMenuHide (true);
				SetFileMenuEnable (false);
				SetViewFontMenuEnable (false, true);
				SetHelpMenuEnable (false);
			}else if (name == "PublicationsWindow") {
				SetLogOutMenuHide (false);
				var controller = (PublicationsWindowController)mainWindow.WindowController;
				if (controller.CurrentViewMode == 3) {
					SetFileMenuEnable (true);
					SetViewFontMenuEnable(true, false);
				} else {
					SetFileMenuEnable (false);
					SetViewFontMenuEnable (false, false);
				}
				SetHelpMenuEnable (true);
			}else{
			}

			isValidateMenuItem = false;

			return true;
		}

		void SetLogOutMenuHide (bool isHidden)
		{
			NSMenu sysmenu = NSApplication.SharedApplication.MainMenu;
			sysmenu.AutoEnablesItems = false;
			//
			NSMenuItem subMenuItem = sysmenu.ItemAt (0);
			NSMenu subMenu = subMenuItem.Submenu;

			//Log Out LNRed
			NSMenuItem subItem = subMenu.ItemAt (4);
			subItem.Hidden = isHidden;
			subItem.Enabled = !isHidden;
		}

		void SetEditMenuStatus()
		{
			NSMenu sysmenu = NSApplication.SharedApplication.MainMenu;

			//Edit menu
			NSMenuItem subMenuItem = sysmenu.ItemAt (2);
			NSMenu subMenu = subMenuItem.Submenu;

			NSMenuItem [] itemArray = subMenu.ItemArray ();
			for (int i = 0; i < itemArray.Length; i++) {
				//Console.WriteLine ("{0}",itemArray [i].Title);
				if (itemArray [i].Title == "Start Dictation…") {
					itemArray [i].Hidden = true;
				}

				if (itemArray [i].Title == "Emoji & Symbols") {
					itemArray [i].Hidden = true;
				}
			}
		}

		/*
		private void SetEditMenuStatus ()
		{
			NSUserDefaults.StandardUserDefaults.SetBool (true, "NSDisabledStartDictationMenuItem");
			NSUserDefaults.StandardUserDefaults.SetBool (true, "NSDisabledEmojiSymbolsMenuItem");
		}
*/

		void SetFileMenuEnable(bool isEnable)
		{
			NSMenu sysmenu = NSApplication.SharedApplication.MainMenu;

			//file menu
			NSMenuItem subMenuItem = sysmenu.ItemAt (1);
			NSMenu subMenu = subMenuItem.Submenu;
			subMenu.AutoEnablesItems = false;

			//New
			NSMenuItem subItem = subMenu.ItemAt (0);
			subItem.Enabled = isEnable;

			//Open
			subItem = subMenu.ItemAt (1);
			subItem.Enabled = isEnable;

			//Open Recent
			subItem = subMenu.ItemAt (2);
			subItem.Enabled = isEnable;

			//Close
			subItem = subMenu.ItemAt (4);
			subItem.Enabled = isEnable;

			subItem = subMenu.ItemAt (5);
			subItem.Enabled = isEnable;

			//Save as PDF - Document
			subItem = subMenu.ItemAt (6);
			//Console.WriteLine ("title:{0}",subItem.Title);
			//subItem.Hidden = !isEnable;
			subItem.Enabled = isEnable;

			//Page Setup...
			subItem = subMenu.ItemAt (9);
			subItem.Enabled = isEnable;

			//Page Print...
			subItem = subMenu.ItemAt (10);
			subItem.Enabled = isEnable;
		}
			
		void SetViewFontMenuEnable (bool isEnable, bool isHide)
		{
			NSMenu sysmenu = NSApplication.SharedApplication.MainMenu;

			//
			NSMenuItem subMenuItem = sysmenu.ItemAt (3);
			NSMenu subMenu = subMenuItem.Submenu;
			subMenu.AutoEnablesItems = false;

			//actual size
			NSMenuItem subItem = subMenu.ItemAt (0);
			subItem.Enabled = isEnable;
			//Console.WriteLine ("title:{0}:{1}",subItem.Title, subItem.Enabled);

			//zoom text in
			subItem = subMenu.ItemAt (1);
			subItem.Enabled = isMaxFontSize==true?false:isEnable;

			//zoom text out
			subItem = subMenu.ItemAt (2);
			subItem.Enabled = isMiniFontSize==true?false:isEnable;

			//orgnize publications
			subItem = subMenu.ItemAt (4);
			bool isItemEnable = false;
			if (isHide) {
				isItemEnable = false;
			}else {
				isItemEnable = !isEnable;
			}
			subItem.Enabled = isItemEnable;
		}

		void SetHelpMenuEnable (bool isEnable)
		{
			NSMenu sysmenu = NSApplication.SharedApplication.MainMenu;

			NSMenuItem subMenuItem = sysmenu.ItemAt (5);
			NSMenu subMenu = subMenuItem.Submenu;
			subMenu.AutoEnablesItems = false;

			//FAQ
			NSMenuItem subItem = subMenu.ItemAt (0);
			subItem.Enabled = isEnable;

			//ContactUs
			subItem = subMenu.ItemAt (1);
			subItem.Enabled = isEnable;

			//other
			subItem = subMenu.ItemAt (2);
			subItem.Hidden = true;
		}

		#endregion
	}
}

