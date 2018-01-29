
using System;

using Foundation;
using UIKit;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LexisNexis.Red.iOS
{
 
	public class LegalWebViewDelegate : UIWebViewDelegate
	{
		LegalDefineViewController legalVC;

		public LegalWebViewDelegate (LegalDefineViewController legalViewControl) : base()

		{
			legalVC = legalViewControl;	

 		}

		public override bool ShouldStartLoad (UIWebView webView, NSUrlRequest request, UIWebViewNavigationType navigationType)
		{
			string urlString = request.Url.AbsoluteString; //  looseleaf://legaldefine?term=identification%20evidence
			if(urlString != "file:///"){
 			    string empyString = urlString.Replace("looseleaf://legaldefine?term=","");   
				string wordString = empyString.Replace ("%20"," ");
   				legalVC.Record (wordString);
				legalVC.selectText = wordString;
 				legalVC.SearchDictionary (wordString,legalVC.currentBookId); 
 			}
			 
 			return true;
		}
}
}
