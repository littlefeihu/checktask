using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Newtonsoft.Json;

using Foundation;
using AppKit;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.BusinessModel;


namespace LexisNexis.Red.Mac
{
	public partial class LegalDefinePopViewController : AppKit.NSViewController
	{
		private string SearchTerm { get; set; }
		private bool IsHrefLoading { get; set; }
		private NavigationDictionaryManager NavigatorManager { set; get; }
		private string TitleCountryCode { get; set; }
		#region Constructors
		// Called when created from unmanaged code
		public LegalDefinePopViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public LegalDefinePopViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public LegalDefinePopViewController (string term, string countryCode) : base ("LegalDefinePopView", NSBundle.MainBundle)
		{
			TitleCountryCode = countryCode;
			SearchTerm = Utility.VerifyLegalTerm(term);
			Initialize ();

			//Console.WriteLine ("0:SearchTerm:{0}",SearchTerm);
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion

		public override void ViewDidLoad ()
		{
			DictionaryWebView.FrameLoadDelegate = new DictionaryeWebFrameLoadDelegate(this);
			DictionaryWebView.UIDelegate = new DictionaryWebUIDelegate ();
			NavigatorManager = new NavigationDictionaryManager();
			NavigatorManager.Record(SearchTerm.Trim());

			SearchDictionary (SearchTerm);
		}

		//strongly typed view accessor
		public new LegalDefinePopView View {
			get {
				return (LegalDefinePopView)base.View;
			}
		}

		public void LoadFinished(string message)
		{
			//Console.WriteLine ("{0}",message);
		}

		#region api with contentWebViewDelegate
		public void CustomMouseUp(string message)
		{
			//Console.WriteLine ("message:{0}", message);
		}

		public void ClickAHref (string message) 
		{
			if (message != null) {
				string words = message.Replace("looseleaf://legaldefine?term=", "");
				string escapeword = Utility.EscapeSpace (words);
				SeeAlso(escapeword);
			}
		}
		#endregion

		private void SearchDictionary(string term)
		{
			if (term != SearchTerm) {
				SearchTerm = term;
			}

			TermTF.StringValue = SearchTerm;

			PreButton.Enabled = NavigatorManager.IsCanBackWard;
			NextButton.Enabled = NavigatorManager.IsCanForWard;

			LegalDefinitionItem legalDefineItem = DictionaryUtil.SearchDictionary(term,TitleCountryCode);
			string htmlString = null;
			if (legalDefineItem == null ||
				legalDefineItem.ContextualDefinitions == null || 
				legalDefineItem.ContextualDefinitions.Count == 0) {
				htmlString = "<div id='content' style='width:140;height:20;margin-left:auto;margin-right:auto;margin-top:100px;margin-bottom:auto;'>No legal definition found.</div>"; 

			} else {
				htmlString = DictionaryHtmlStringFromLDItem (legalDefineItem, term);
				//Console.WriteLine ("{0}", htmlString);
			}

			DictionaryWebView.MainFrame.LoadHtmlString (htmlString, NSBundle.MainBundle.ResourceUrl);
		}

		private void SeeAlso(string words)
		{
			NavigatorManager.Record(words);
			SearchDictionary(words);
		}

		#region action
		partial void DictButtonClick (NSObject sender)
		{
			var pboard = NSPasteboard.GeneralPasteboard;

			string[] typeArray = {"NSStringPboardType"};
			pboard.DeclareTypes(typeArray,this);
			pboard.SetStringForType(SearchTerm,"NSStringPboardType");

			var cachepath = NSSearchPath.GetDirectories(NSSearchPathDirectory.ApplicationDirectory,NSSearchPathDomain.System);
			string appRootPath = cachepath[0]+"/"+"Dictionary.app";

			if (!Directory.Exists (appRootPath)) {
				return;
			}
				
			NSUrl url = new NSUrl(appRootPath);
			if (url != null) {
				NSWorkspace.SharedWorkspace.LaunchApplication("Dictionary.app");
			}
		}

		/*
		private void pressCMDV ()
		{
			CGEventSourceRef source = CGEventSourceCreate(kCGEventSourceStateCombinedSessionState);
			CGEventRef keyDown = CGEventCreateKeyboardEvent(source, (CGKeyCode) 9, true);
			CGEventSetFlags(keyDown, kCGEventFlagMaskCommand);
			CGEventRef keyUp = CGEventCreateKeyboardEvent(source, (CGKeyCode) 9, false);
			CGEventPost(kCGAnnotatedSessionEventTap, keyDown);
			CGEventPost(kCGAnnotatedSessionEventTap, keyUp);
			CFRelease(keyUp);
			CFRelease(keyDown);
			CFRelease(source);
		}*/

		partial void NextButtonClick (NSObject sender)
		{
			var words = NavigatorManager.GoForward();
			SearchDictionary(words);
		}

		partial void PreButtonClick (NSObject sender)
		{
			var words = NavigatorManager.GoBack();
			SearchDictionary(words);
		}

		partial void WikiButtonClick (NSObject sender)
		{
			var term = SearchTerm.Trim();
			var urlString = "https://www.google.com/search?q=" + term;
			NSString stringURL = new NSString(urlString);
			stringURL = stringURL.CreateStringByAddingPercentEscapes(NSStringEncoding.UTF8);

			NSUrl url = new NSUrl(stringURL.ToString());
			if (url != null) {
			    NSWorkspace.SharedWorkspace.OpenUrl(url);
			}
		}
		#endregion

		private String DictionaryHtmlStringFromLDItem(LegalDefinitionItem definitions, string term)
		{
			var version = definitions.DictionaryVersionText;
			var count = definitions.RelatedKeywordsCount;
			var itemList = definitions.ContextualDefinitions;

			String contextualDefinitions = String.Empty;
			string header = "<h3>" + term + "</h3>";
			contextualDefinitions += header;

			foreach (LegalContextualDefinitionItem definition in definitions.ContextualDefinitions)
			{
				contextualDefinitions += definitions.ContextualDefinitions.Count > 1 && definition.Context.Length > 0 ? 
					"<span class='Item'>" + definition.Context + "</span>" : String.Empty;
				contextualDefinitions += definition.DefinitionHtml;

				String relatedKeywords = string.Join("; ", definition.AllRelatedKeywords.Select(k => k.HyperlinkTermKeyword));
				String relatedKeywordsLabel = relatedKeywords.Length > 0 ? (contextualDefinitions.Length > 0 ? 
					"See also: " : "See: ") : String.Empty;
				String relatedKeywordClosing = relatedKeywords.Length > 0 ? "." : "";

				contextualDefinitions += "<br><br>" + relatedKeywordsLabel + relatedKeywords + relatedKeywordClosing;
				contextualDefinitions += "<br><br>";
			}

			string dicVersion = "<div class='version'>" + version + "</div>";
			contextualDefinitions += dicVersion;

			if (File.Exists ("Dictionary/Dictionary.html")) {
				var contentString = File.ReadAllText ("Dictionary/Dictionary.html");

				contentString = contentString.Replace ("#CONTENT#", contextualDefinitions);

				return contentString;
			} else {
				return "";
				//return BuildHtml (definitions, term, version);
			}
		}

		private String BuildHtml(LegalDefinitionItem definitions, string term, string version)
		{
			String contextualDefinitions = String.Empty;

			foreach (LegalContextualDefinitionItem definition in definitions.ContextualDefinitions)
			{
				contextualDefinitions += definitions.ContextualDefinitions.Count > 1 && definition.Context.Length > 0 ? "<span class='context'>" + definition.Context + "</span> " : String.Empty;
				contextualDefinitions += definition.DefinitionHtml;

				String relatedKeywords = string.Join("; ", definition.AllRelatedKeywords.Select(k => k.HyperlinkTermKeyword));
				String relatedKeywordsLabel = relatedKeywords.Length > 0 ? (contextualDefinitions.Length > 0 ? "See also: " : "See: ") : String.Empty;
				String relatedKeywordClosing = relatedKeywords.Length > 0 ? "." : "";

				contextualDefinitions += "<br><br>" + relatedKeywordsLabel + relatedKeywords + relatedKeywordClosing;
				contextualDefinitions += "<br><br>";
			}

			const String cssBody =
				@"  <style type='text/css'>
                        body {color: Black; font-size:12px;font-family:'Segoe UI';}
                        h3 {color: Black; font-size:12px;font-weight:bold;font-family:'Segoe UI';}
                        .version { color: Black; font-size:12px; font-family:'Segoe UI'; text-align:right; font-style:italic; }
                        .Content { margin-right:10px;}
                        a, a:link, a:visited {color:#ed1c24; text-decoration:none;}
                        a:hover {color:#ed1c24; text-decoration:underline;}</style>";

			const String js =
				@"   <meta name='viewport' content='width=device-width, initial-scale=1.0, minimum-scale=1.0, maximum-scale=1.0, user-scalable=no'/>
                     <meta http-equiv='Pragma' content='no-cache' />
                     <meta http-equiv='Expires' content='-1' />
                     <script src='ms-appx-web:///Html/Js/jquery.min.js'></script>
                     <script language='javascript' type='text/javascript'>
                     function ListenHref() {
                     $('a').click(function (e) {
                     if (this.href == null || this.href == '')
                     return false;  
                    var jsonObj = '{ type:\'href\',  value1:\'' + this.href + '\'}';
                    window.external.notify(jsonObj);
                    return false;
                    });};
                     </script>";

			string h = "<h3>" + term + "</h3>";
			string v = "<div class='version'>" + version + "</div>";

			String html =
				@"<html>
                    <head>" + js + cssBody + @"</head>
                    <body oncontextmenu='return false;'  onload='ListenHref();'>
                        <div id='content' class='Content'>"
                            + h + contextualDefinitions +v+
			"</div></body></html>";

			return html;
		}
	}


	public class NavigationDictionaryManager
	{
		public NavigationDictionaryManager()
		{
			HistoryRecords = new List<string>();
			CurrenRecordIndex = -1;
		}

		public List<string> HistoryRecords { get; set; }
		public int CurrenRecordIndex { get; set; }
		public bool IsCanBackWard { get; set; }
		public bool IsCanForWard { get; set; }

		public void Record(string words)
		{

			if (CurrenRecordIndex == HistoryRecords.Count - 1) {
				CurrenRecordIndex++;
				HistoryRecords.Add(words);
			} else if (CurrenRecordIndex < HistoryRecords.Count - 1){
				CurrenRecordIndex++;
				HistoryRecords.RemoveRange(CurrenRecordIndex, HistoryRecords.Count - CurrenRecordIndex);
				HistoryRecords.Add(words);
			}
			IsCanBackWard = CurrenRecordIndex > 0 ? true : false;
			IsCanForWard = false;
		}

		public string GoForward()
		{
			CurrenRecordIndex++;
			IsCanForWard = CurrenRecordIndex < HistoryRecords.Count - 1 ? true : false;
			IsCanBackWard = true;
			return HistoryRecords[CurrenRecordIndex];
		}

		public string GoBack()
		{
			CurrenRecordIndex--;
			IsCanForWard = true;
			IsCanBackWard = CurrenRecordIndex > 0 ? true : false;
			return HistoryRecords[CurrenRecordIndex];
		}
	}
}
