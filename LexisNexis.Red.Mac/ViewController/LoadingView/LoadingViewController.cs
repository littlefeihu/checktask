using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public partial class LoadingViewController : AppKit.NSViewController
	{
		public string LoadInfo { get; set; }

		#region Constructors

		// Called when created from unmanaged code
		public LoadingViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public LoadingViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public LoadingViewController (CGPoint location, string loadInfo) : base ("LoadingView", NSBundle.MainBundle)
		{
			LoadInfo = loadInfo;
			View.SetFrameOrigin (location);
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		//strongly typed view accessor
		public new LoadingView View {
			get {
				return (LoadingView)base.View;
			}
		}



		public override void ViewDidLoad ()
		{
//			View.WantsLayer = true;
//			View.Layer.BackgroundColor = Utility.ColorWithRGB(240,241,242,1.0f).CGColor;
//			View.Layer.CornerRadius = 10.0f;
			LoadInfoTF.StringValue = LoadInfo;

		}
	}
}
