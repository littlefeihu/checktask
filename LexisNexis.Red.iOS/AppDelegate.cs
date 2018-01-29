using System.Collections.Generic;

using Foundation;
using UIKit;

using LexisNexis.Red.Common;
using LexisNexis.Red.Common.Interface;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.iOS.Common.Implementation;


namespace LexisNexis.Red.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		
		public override UIWindow Window {
			get;
			set;
		}

		public LoginInterceptorViewController LoginInterceptorVC {
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the current popover controller.
		/// </summary>
		/// <value>The current popover controller.</value>
		public UIPopoverController CurPopoverController{ get; set;}

		public MyPublicationViewController MyPublicationController{ get; set;}

		public OpenedPublication CurOpendPublication{ get; set; }

		public List<PublicationView> PublicationViewList{ get; set; }


		// This method is invoked when the application is about to move from active to inactive state.
		// OpenGL applications should use this method to pause.
		public override void OnResignActivation (UIApplication application)
		{
		}
		
		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
		}
		
		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
		}
		
		// This method is called when the application is about to terminate. Save data, if needed.
		public override void WillTerminate (UIApplication application)
		{
		}

		public override void FinishedLaunching (UIApplication application)
		{

			IoCContainer.Instance.RegisterInstance<IDevice> (new Device());
			IoCContainer.Instance.RegisterInstance<IDirectory> (new FileDirectory());
			IoCContainer.Instance.RegisterInstance<INetwork> (new DeviceNetwork());
			IoCContainer.Instance.RegisterInstance<IPackageFile> (new PackageFile ());
			IoCContainer.Instance.RegisterInstance<ICryptogram> (new Cryptogram ());

			PublicationViewList = new List<PublicationView> ();
			CurOpendPublication = new OpenedPublication ();


		}
	}
}

