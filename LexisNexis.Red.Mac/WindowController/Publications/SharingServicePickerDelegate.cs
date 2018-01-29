using System;
using AppKit;
using Foundation;
using ObjCRuntime;
using System.Collections.Generic;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	public class SharingServicePickerDelegate : NSSharingServicePickerDelegate
	{
		public SharingServicePickerDelegate () :base()
		{
		}

		public override NSSharingService[] SharingServicesForItems (NSSharingServicePicker sharingServicePicker, 
			NSObject[] items, 
			NSSharingService[] proposedServices)
		{
			List<NSSharingService> services = new List<NSSharingService>(0);

			//services.AddRange(proposedServices);

			NSString theFirstString = null;
			foreach (var item in items) {
				var ptr = Class.GetHandle ("NSString");
				if (item.IsKindOfClass(new Class(ptr))) {
					theFirstString = new NSString(item.ToString());
					break;
				}

				var attString = Class.GetHandle ("NSAttributedString");
				if (item.IsKindOfClass(new Class(attString))) {
					theFirstString = new NSString(((NSAttributedString)item).ToString());
					break;
				}
			}

			if (theFirstString != null) {
				var emailService = new NSSharingService ("Email document", 
					NSImage.ImageNamed(NSImageName.MobileMe), 
					NSImage.ImageNamed(NSImageName.MobileMe), () => {EmailDocument();
				});

				services.Add (emailService);

				var printService = new NSSharingService ("Print document", 
					NSImage.ImageNamed(NSImageName.StatusAvailable), 
					NSImage.ImageNamed(NSImageName.StatusAvailable), () => {PrintDocument();
					});

				services.Add (printService);
			}

			nint count = services.Count;

			return services.ToArray();
		}

		public override INSSharingServiceDelegate DelegateForSharingService (NSSharingServicePicker sharingServicePicker, 
			NSSharingService sharingService)
		{
			return new SharingServiceDelegate() ;
		}

		private void EmailDocument()
		{
		}

		private void PrintDocument()
		{
		}
	}



	public class SharingServiceDelegate: NSSharingServiceDelegate
	{
		public SharingServiceDelegate() : base()
		{
		}

		public override CGRect SourceFrameOnScreenForShareItem (NSSharingService sharingService, INSPasteboardWriting item)
		{
			return new CGRect(0,0,0,0);	
		}

		public override NSImage TransitionImageForShareItem (NSSharingService sharingService, INSPasteboardWriting item, CGRect contentRect)
		{
			return null;
		}

		public override NSWindow SourceWindowForShareItems (NSSharingService sharingService, NSObject[] items, NSSharingContentScope sharingContentScope)
		{
			return NSApplication.SharedApplication.MainWindow;
		}
	}
}

