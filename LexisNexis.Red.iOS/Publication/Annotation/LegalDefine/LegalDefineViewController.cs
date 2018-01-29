using System;
using System.Linq;
using System.Collections.Generic;


using Foundation;
using UIKit;
using CoreGraphics;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.HelpClass;
using HtmlAgilityPack;

namespace LexisNexis.Red.iOS
{
	
	public partial class LegalDefineViewController : UIViewController
	{
		public List<string> HistotyRecords{ get; set;}
		public int CurrentRecordIndex = -1;
   		public  List<LegalContextualDefinitionItem> LegalDefineList = new List<LegalContextualDefinitionItem> ();
 		public Dictionary<string, List<LegalContextualDefinitionItem>> LegalListDict{ get; set; }

		public string selectText{ get; set;}
 		public int currentBookId{ get; set;}
  
  		public LegalDefineViewController () : base ("LegalDefineViewController", null)
 		{
			HistotyRecords = new List<string>();
  		}
 
		public async override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			webView.Delegate = new LegalWebViewDelegate(this);
   
 			BackButton.SetImage (new UIImage ("Images/Navigation/RedBackArrow.png"), UIControlState.Normal);
			ForButton.SetImage (new UIImage ("Images/Navigation/GrayForwardArrow.png"), UIControlState.Normal);

			BackButton.Enabled = false;
			ForButton.Enabled = false;
		
			SelectTitleLabel.Text = selectText;
			Record (SelectTitleLabel.Text);

			currentBookId = AppDataUtil.Instance.GetCurrentPublication ().BookId;
 
 			SearchDictionary (SelectTitleLabel.Text,currentBookId);

    	} 

   
 		public void SearchDictionary(string words,int  bookId )
		{
			LegalDefinitionItem legalDefine = DictionaryUtil.SearchDictionary(words, AppDataUtil.Instance.GetCurrentPublication().BookId);
 			if (legalDefine == null || legalDefine.ContextualDefinitions == null || legalDefine.ContextualDefinitions.Count == 0)
			{
				string	htmlString = "<div id='content' style='width:240;height:40;margin-left:auto;margin-right:auto;font-size:22px;margin-top:150px;margin-bottom:auto;'>No legal definition found.</div>";//color:#A0A0A4; TextColor
  				webView.LoadHtmlString (htmlString,NSUrl.FromString(""));
 			}
			else
			{
 				if (legalDefine.ContextualDefinitions.Count != 0 ) {
					string html = BuildHtml (legalDefine,legalDefine.ContextualDefinitions[0].Term,legalDefine.DictionaryVersionText);
					webView.LoadHtmlString (html,NSUrl.FromString(""));
				}
			}
		}
		 

		public void Record(string words){
  			if (CurrentRecordIndex == HistotyRecords.Count - 1) {
				CurrentRecordIndex++;
				SelectTitleLabel.Text = words;
 				HistotyRecords.Add (words);
			} else if (CurrentRecordIndex < HistotyRecords.Count - 1) {
				CurrentRecordIndex++;
				SelectTitleLabel.Text = words;
 				HistotyRecords.RemoveRange (CurrentRecordIndex, HistotyRecords.Count - CurrentRecordIndex);
				HistotyRecords.Add (words);
			}
 			BackButton.Enabled = CurrentRecordIndex > 0 ? true : false;
			ForButton.Enabled = false;
		}
 
		partial void ForButtonClick (NSObject sender)
		{
			CurrentRecordIndex++;
  			string words = HistotyRecords[CurrentRecordIndex];
			SelectTitleLabel.Text = words;
 			SearchDictionary(SelectTitleLabel.Text,currentBookId);
  			BackButton.Enabled = true;
			ForButton.Enabled = CurrentRecordIndex < HistotyRecords.Count -1 ? true :false;
 		}

  
		partial void BackButtonClick (NSObject sender)
		{
			CurrentRecordIndex--;
 			string words = HistotyRecords[CurrentRecordIndex];
			SelectTitleLabel.Text = words;
 			SearchDictionary(SelectTitleLabel.Text,currentBookId);
  			BackButton.Enabled = CurrentRecordIndex > 0 ? true : false;
			ForButton.Enabled = true;
				 
		}
 
 
		public String BuildHtml(LegalDefinitionItem definitions, string term,string version)
		{
			String contextualDefinitions = String.Empty;

			foreach (LegalContextualDefinitionItem definition in definitions.ContextualDefinitions)
			{
				contextualDefinitions += definitions.ContextualDefinitions.Count > 1 && definition.Context.Length > 0 ? "<span class='context'>" + definition.Context + "</span> " : String.Empty;
				contextualDefinitions += definition.DefinitionHtml;

				String relatedKeywords = string.Join("; ", definition.AllRelatedKeywords.Select(k => k.HyperlinkTermKeyword));
				String relatedKeywordsLabel = relatedKeywords.Length > 0 ? (contextualDefinitions.Length > 0 ? "See also: " : "See: ") : String.Empty;
				String relatedKeywordClosing = relatedKeywords.Length > 0 ? "." : "";

				contextualDefinitions += "<br/><br/>" + relatedKeywordsLabel + relatedKeywords + relatedKeywordClosing;
				contextualDefinitions += "<br/><br/>";
			}
			//#ed1c24
			const String cssBody =
				@"  <style type='text/css'>
                        body {color: Black; font-size:18px;font-family:'Segoe UI';}
                        h3 {color: Black; font-size:28px;font-weight:bold;font-family:'Segoe UI';}
                        .version { color: Black; font-size:11px; font-family:'Segoe UI'; text-align:right; font-style:italic; }
                        .Content { margin-right:12px;}
                        a, a:link, a:visited {color:#0D94FC;font-size:16px;font-weight:bold; text-decoration:none;}
                        a:hover {color:#1680FC; text-decoration:underline;}</style>";

			const String js =//device-width
				@"   <meta name='viewport' content='width=320, initial-scale=1.0, minimum-scale=1.0, maximum-scale=1.0, user-scalable=no'/>
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
                        <div id='content' class='Content' style='overflow-x:hidden;'>"
                            + h + contextualDefinitions +v+
			"</div></body></html>";

			return html;
		}

		partial void searchWebButton (NSObject sender)
		{
			UIApplication.SharedApplication.OpenUrl(NSUrl.FromString("http://www.google.com/search?q="+Title));
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();

 		}
 		 

  	}


 
}

