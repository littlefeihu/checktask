using UIKit;

using Xamarin;

namespace LexisNexis.Red.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			#if RELEASE
			Insights.Initialize ("5cf7a6c1f8fd825732daeef0419ef76585d28c2a");
			#elif TESTING
			Insights.Initialize ("fae515b407b6914ab402a221bf097131122b2c5d");
			#elif PREVIEW
			Insights.Initialize ("497c9b7504189b054ec73732670ea54bfa3d5a66");
			#else
			Insights.Initialize ("f7ae58689ddde8c8084e012fee4378a9ae16302f");
			#endif

			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}

