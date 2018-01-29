using Foundation;
using System;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using UIKit;

using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.iOS
{
	public partial class LoginInterceptorViewController : UIViewController
	{
		public LoginInterceptorViewController (IntPtr handle) : base (handle)
		{
		}

		#region override UIViewController
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

 			GlobalAccess.Instance.Init ().ContinueWith((task)=>{
				this.InvokeOnMainThread(()=>{
					AppDisplayUtil.Instance.AppDelegateInstance.LoginInterceptorVC = this;
					View.BackgroundColor = UIColor.FromRGB (237, 28, 36);
					if (HasLogin ()) { //If user has login
						GoToMyPublicationViewController ();
					} else {
						GoToLoginViewController ();
					}
				});
			});
		
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			NavigationController.NavigationBarHidden = true;

		}
		#endregion

		/// <summary>
		/// Determines whether user has login.
		/// </summary>
		/// <returns><c>true</c> if user has login; otherwise, <c>false</c>.</returns>
		private bool HasLogin ()
		{
			var curUser= GlobalAccess.Instance.CurrentUserInfo;
			return curUser != null;
		}

		public void GoToMyPublicationViewController()
		{
			PerformSegue ("LoginInterceptorToHome", this);
		}

		public void GoToLoginViewController ()
		{
			PerformSegue ("LoginInterceptorViewControllerToLoginViewController", this);
		}

		public override UIStatusBarStyle PreferredStatusBarStyle ()
		{
			return UIStatusBarStyle.LightContent;
		}

	}
}
