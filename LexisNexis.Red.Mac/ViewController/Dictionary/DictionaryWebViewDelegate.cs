using System;
using System.Collections.Generic;
using System.Linq;

using AppKit;
using Foundation;
using WebKit;
using ObjCRuntime;
using Newtonsoft.Json;
using CoreGraphics;

namespace LexisNexis.Red.Mac
{
	class DictionaryeWebFrameLoadDelegate : WebFrameLoadDelegate
	{
		LegalDefinePopViewController viewController;

		public DictionaryeWebFrameLoadDelegate (LegalDefinePopViewController aController)
		{
			this.viewController = aController;
		}


		public override void ClearedWindowObject (WebView webView, WebScriptObject windowObject, WebFrame forFrame)
		{
			//Console.WriteLine ("ClearedWindowObject");
			windowObject.SetValueForKey(this, new NSString("dictWebView"));
		}
			
		public override void StartedProvisionalLoad (WebView sender, WebFrame forFrame)
		{
			//Console.WriteLine ("StartedProvisionalLoad");
		}

		public override void FinishedLoad (WebView sender, WebFrame forFrame)
		{
			//Console.WriteLine ("FinishedLoad");
			viewController.LoadFinished ("FinishedLoad");
		}

		[Export("webView:windowScriptObjectAvailable:")]
		public override void WindowScriptObjectAvailable(WebView webView, WebScriptObject windowScriptObject)
		{
		    //Console.WriteLine ("WindowScriptObjectAvailable");
		}

		// This shows you how to expose an Objective-C function that is not present.
		// The function isSelectorExcludedFromWebScript: is part of the WebScripting Protocol
		[Export ("isSelectorExcludedFromWebScript:")]
		static bool isSelectorExcludedFromWebScript(Selector sel)
		{
			//Console.WriteLine ("isSelectorExcludedFromWebScript{0}", sel.Name);
			// For security, you must explicitly allow a selector to be called from JavaScript.
			// i.e. CustomMouseUp: is NOT _excluded_ from scripting, so it can be called.

			switch (sel.Name)
			{
			case "CustomMouseUp:":
			case "ClickAHref:":
				return false;
			}

			return true;  // disallow everything else
		}

		[Export("CustomMouseUp:")]
		public void DictionaryMouseUp(string message)
		{
			//Console.WriteLine ("CustomMouseUp{0}", message);
			viewController.CustomMouseUp (message);
		}

		[Export("ClickAHref:")]
		public void ClickHref(string message)
		{
			//Console.WriteLine ("ClickHref{0}", message);
			viewController.ClickAHref (message);
		}

		[Export ("webScriptNameForSelector:")]
		static string webScriptNameForSelector(Selector sel)
		{
			//Console.WriteLine ("webScriptNameForSelector{0}", sel.Name);
			switch (sel.Name)
			{
			case "CustomMouseUp:":
				return "CustomMouseUp";

			case "ClickAHref:":
				return "ClickAHref";
			}

			return "";
		}
	}

	class DictionaryWebUIDelegate : WebUIDelegate
	{
		public override NSMenuItem[] UIGetContextMenuItems (WebView sender,
			NSDictionary forElement, NSMenuItem[] defaultMenuItems)
		{
			return null;
		}
	}

	class DictionaryPolicyDelegate : WebPolicyDelegate
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
