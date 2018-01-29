using System;
using System.Collections.Generic;

using AppKit;
using Foundation;
using WebKit;
using ObjCRuntime;
using Newtonsoft.Json;
using System.Linq;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	class ContentPageWebFrameLoadDelegate : WebFrameLoadDelegate
	{
		PageContentViewController viewController;

		public ContentPageWebFrameLoadDelegate (PageContentViewController aController)
		{
			this.viewController = aController;
		}


		public override void ClearedWindowObject (WebView webView, WebScriptObject windowObject, WebFrame forFrame)
		{
			//Console.WriteLine ("ClearedWindowObject");
			windowObject.SetValueForKey(this, new NSString("native"));
		}

		#if true
		public override void StartedProvisionalLoad (WebView sender, WebFrame forFrame)
		{
			//Console.WriteLine ("StartedProvisionalLoad");
		}

		public override void FinishedLoad (WebView sender, WebFrame forFrame)
		{
			viewController.LoadFinished ("FinishedLoad");
		}

		[Export("webView:windowScriptObjectAvailable:")]
		public override void WindowScriptObjectAvailable(WebView webView, WebScriptObject windowScriptObject)
		{
		    //Console.WriteLine ("WindowScriptObjectAvailable");
		}
		#endif

		// This shows you how to expose an Objective-C function that is not present.
		// The function isSelectorExcludedFromWebScript: is part of the WebScripting Protocol
		[Export ("isSelectorExcludedFromWebScript:")]
		static bool isSelectorExcludedFromWebScript(Selector sel)
		{
			//Console.WriteLine ("isSelectorExcludedFromWebScript{0}",sel.Name);

			// For security, you must explicitly allow a selector to be called from JavaScript.
			// i.e. CustomMouseUp: is NOT _excluded_ from scripting, so it can be called.

			switch (sel.Name)
			{
			case "CustomMouseUp:":
			case "CustomScrollDrag:":
			case "ClickAHref:":
			case "updateMarker:":
				return false;
			}

			return true;  // disallow everything else
		}

		[Export("CustomMouseUp:")]
		public void CustomMouseUp(string message)
		{
			//Console.WriteLine ("CustomMouseUp{0}",message);
			viewController.CustomMouseUp (message);
		}

		[Export("CustomScrollDrag:")]
		public void CustomScrollDrag(string message)
		{
			//Console.WriteLine ("CustomScrollDrag{0}",message);
			viewController.CustomScrollDrag (message);
		}

		[Export("ClickAHref:")]
		public void ClickHref(string message)
		{
			//Console.WriteLine ("ClickHref{0}",message);
			viewController.ClickAHref (message);
		}

		[Export ("webScriptNameForSelector:")]
		static string webScriptNameForSelector(Selector sel)
		{
			//Console.WriteLine ("webScriptNameForSelector{0}",sel.Name);
			switch (sel.Name)
			{
			case "CustomMouseUp:":
				return "CustomMouseUp";

			case "CustomScrollDrag:":
				return "CustomScrollDrag:";
					
			case "ClickAHref:":
				return "ClickAHref";

			case "updateMarker:":
				return "updateMarker";
			}

			return "";
		}
			
	}

	class ContentPageWebUIDelegate : WebUIDelegate
	{
		public override NSMenuItem[] UIGetContextMenuItems (WebView sender,
			NSDictionary forElement, NSMenuItem[] defaultMenuItems)
		{
			return null;
		}

		public override float UIGetHeaderHeight (WebView sender)
		{
			return 44;
		}

		public override float UIGetFooterHeight (WebView sender)
		{
			return 44;
		}

		public override void UIDrawHeaderInRect (WebView sender, CGRect rect)
		{
			//throw new System.NotImplementedException ();

			CGRect destRect = sender.MainFrame.FrameView.Frame;
			destRect.Y = destRect.Height-44.0f;
			destRect.Height = 34.0f;
			destRect.X = 30.0f;
			destRect.Width -= 60.0f;

			// Draw label.
			NSStringAttributes attributes = new NSStringAttributes ();
			attributes.Font = NSFont.SystemFontOfSize(11);
			"header".DrawInRect (rect, attributes);
		}

		public override void UIDrawFooterInRect (WebView sender, CGRect rect)
		{
			CGRect destRect = sender.MainFrame.FrameView.Frame;
			destRect.Height = 44.0f;
			destRect.X = 30.0f;
			destRect.Width -= 60.0f;

			// Draw label.
			NSStringAttributes attributes = new NSStringAttributes ();
			attributes.Font = NSFont.SystemFontOfSize(11);
			"footer".DrawInRect (rect, attributes);
		}

		public override CGRect UIGetContentRect (WebView sender)
		{
			return new CGRect(0,0,100,100);
		}

//		- (BOOL)webView:(WebView *)webView shouldChangeSelectedDOMRange:
//		(DOMRange *)currentRange
//		toDOMRange:(DOMRange *)proposedRange
//		affinity:(NSSelectionAffinity)selectionAffinity
//		stillSelecting:(BOOL)flag
//		{
//			return [[proposedRange startContainer] isContentEditable];
//		}
	}

	class ContentPagePolicyDelegate : WebPolicyDelegate
	{
		public override void DecidePolicyForNavigation (WebView webView, NSDictionary actionInformation, NSUrlRequest request, WebFrame frame, NSObject decisionToken)
		{
			switch ((WebNavigationType)((NSNumber)actionInformation [WebPolicyDelegate.WebActionNavigationTypeKey]).Int32Value) {
			case WebNavigationType.BackForward:
			case WebNavigationType.FormResubmitted:
			case WebNavigationType.FormSubmitted:
				WebPolicyDelegate.DecideIgnore (decisionToken);
				break;
			case WebNavigationType.LinkClicked:
				NSWorkspace.SharedWorkspace.OpenUrl (actionInformation[WebPolicyDelegate.WebActionOriginalUrlKey] as NSUrl);
				WebPolicyDelegate.DecideIgnore (decisionToken);
				break;
			case WebNavigationType.Other:
			case WebNavigationType.Reload:
				WebPolicyDelegate.DecideUse (decisionToken);
				break;
			}
		}
	}
}

