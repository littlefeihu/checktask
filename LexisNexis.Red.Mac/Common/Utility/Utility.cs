using System;
using Foundation;
using AppKit;
using CoreGraphics;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.HelpClass;
using CoreText;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LexisNexis.Red.Mac
{
	public static class Utility
	{
		public static NSImage ImageWithFilePath(string filePath)
		{
			return NSImage.FromStream (System.IO.File.OpenRead (NSBundle.MainBundle.ResourcePath+filePath));
		}
			
		public static NSColor ColorWithHexColorValue (string hexColorValue, nfloat alpha)
		{
			string value = hexColorValue.Replace ("#","0x");
			if (value.Length == 8){
				string red = value.Substring (2, 2);
				string green = value.Substring (4, 2);
				string blue = value.Substring (6, 2);

				return NSColor.FromCalibratedRgba (Convert.ToInt16 (red, 16)/255.0f, 
												Convert.ToInt16 (green, 16)/255.0f, 
												Convert.ToInt16 (blue, 16)/255.0f, 
					                            alpha);
			}

			return NSColor.Clear;
		}

		public static NSColor ColorWithRGB (float red, float green, float blue, nfloat alpha)
		{
			return NSColor.FromCalibratedRgba (red/255.0f, green/255.0f, blue/255.0f, alpha);
		}

		public static NSColor BlendNSColor(int level, int totalLevel)
		{
			float FirstLevelAlpha = 0.35f;
			float MaxLevelAlpha = 1.0f;
			float ratio = FirstLevelAlpha + (MaxLevelAlpha - FirstLevelAlpha) * ((float)level) / ((float)totalLevel);

			float inverseRatio = 1.0f - ratio;
			float r = 255, g = 0, b = 0;
			float br = 255, bg = 255, bb = 255;


			float red = (r * ratio) + (br * inverseRatio);
			float green = (g * ratio) + (bg * inverseRatio);
			float blue = (b * ratio) + (bb * inverseRatio);

			return Utility.ColorWithRGB (red, green, blue, 1.0f);
		}

		public static CGColor BlendColor(int level, int totalLevel)
		{
			float FirstLevelAlpha = 0.35f;
			float MaxLevelAlpha = 1.0f;
			float ratio = FirstLevelAlpha + (MaxLevelAlpha - FirstLevelAlpha) * ((float)level) / ((float)totalLevel);

			float inverseRatio = 1.0f - ratio;

			float r = 255, g = 2, b = 4;
			float br = 255, bg = 255, bb = 255;

			float red = (r * ratio) + (br * inverseRatio);
			float green = (g * ratio) + (bg * inverseRatio);
			float blue = (b * ratio) + (bb * inverseRatio);

			return  Utility.ColorWithRGB (red,green,blue,1.0f).CGColor;
		}
			
		public static CGColor CGColorByLevel(int level)
		{
			int red,green,blue;

			switch(level) {
			case 0:
				red = 255;
				green = 205;
				blue = 205;
				break;
			case 1:
				red = 254;
				green = 179;
				blue = 180;
				break;
			case 2:
				red = 254;
				green = 154;
				blue = 155;
				break;
			case 3:
				red = 254;
				green = 128;
				blue = 130;
				break;
			case 4:
				red = 253;
				green = 104;
				blue = 106;
				break;
			case 5:
				red = 253;
				green = 78;
				blue = 82;
				break;
			case 6:
				red = 253;
				green = 54;
				blue = 59;
				break;
			case 7:
				red = 253;
				green = 31;
				blue = 39;
				break;
			case 8:
				red = 253;
				green = 14;
				blue = 27;
				break;

			default:
				red = 252;
				green = 0;
				blue = 13;
				break;
			}

			return Utility.ColorWithRGB (red,green,blue,1.0f).CGColor;
		}
			
		public static NSAttributedString AttributeTitle (string title, NSColor color, float fontsize)
		{
			NSMutableParagraphStyle ps = new NSMutableParagraphStyle();
			ps.Alignment = NSTextAlignment.Center;

			NSColor fontColor = color;
			NSFont font = NSFont.SystemFontOfSize(fontsize);

			NSString titleObj = new NSString (title);

			NSStringAttributes attributes = new NSStringAttributes ();
			attributes.Font = font;
			attributes.ForegroundColor = fontColor;
			attributes.ParagraphStyle = ps;
			attributes.BackgroundColor = NSColor.Clear;
			attributes.ToolTip = titleObj;
			NSAttributedString buttonString = new NSAttributedString (title,attributes);

			return buttonString;
		}

		public static NSAttributedString AttributedPartialTitle(string title, List<string>keyWordsList, NSColor fontColor, 
			string fontName, float fontSize, NSTextAlignment textAlignment, NSLineBreakMode breakMode)
		{
			string patten = "\\b(";
			int i = 0;

			foreach (var item in keyWordsList) {
				if (i!=0) {
					patten = patten + "|" + item;
				}else {
					patten = patten + item;
				}
				i++;
			}
			patten = patten+")\\b";
			string [] results = Regex.Split (title, patten);

			NSMutableParagraphStyle ps = new NSMutableParagraphStyle();
			ps.Alignment = textAlignment;
			ps.LineBreakMode = breakMode;

			NSMutableAttributedString attrTitle = new NSMutableAttributedString ();

			foreach (var item in results) {
				//Console.WriteLine ("result:{0}", item);
				if (item.Length == 0) {
					continue;
				}
				if (keyWordsList.Contains (item)) {
					NSStringAttributes attributes = new NSStringAttributes ();

					attributes.Font = NSFont.FromFontName(fontName+" Bold Italic",fontSize);
					attributes.ForegroundColor = Utility.ColorWithRGB(0,0,0,0.45f);
					attributes.ParagraphStyle = ps;

					NSAttributedString attrString = new NSAttributedString (item, attributes);
					attrTitle.Append (attrString);
				} else {
					NSStringAttributes attributes = new NSStringAttributes ();
					attributes.Font = NSFont.FromFontName(fontName,fontSize);
					attributes.ForegroundColor = fontColor;
					attributes.ParagraphStyle = ps;
					NSAttributedString attrString = new NSAttributedString (item, attributes);
					attrTitle.Append (attrString);
				}

			}

			return attrTitle;
		}

		public static NSAttributedString AttributedTitle (string title, NSColor color, String fontName, float fontSize, NSTextAlignment textAlignment)
		{
			NSMutableParagraphStyle ps = new NSMutableParagraphStyle();
			ps.Alignment = textAlignment;
			ps.LineSpacing = 0.0f;
			NSColor fontColor = color;
			NSFont font = NSFont.FromFontName(fontName,fontSize);
			if (font == null) {
				font = NSFont.SystemFontOfSize (fontSize);
			}
			NSString titleObj = new NSString (title);

			NSStringAttributes attributes = new NSStringAttributes ();
			attributes.Font = font;
			attributes.ForegroundColor = fontColor;
			attributes.ParagraphStyle = ps;
			attributes.ToolTip = titleObj;
			NSAttributedString buttonString = new NSAttributedString (title,attributes);

			return buttonString;
		}

		public static NSAttributedString AttributeLinkTitle (string title, string subtitle, string subject, string body, NSColor color, float fontsize)
		{
			NSColor fontColor = color;
			NSFont font = NSFont.SystemFontOfSize (fontsize);

			int index = title.IndexOf (subtitle);
			int linkIndex = index + subtitle.Length;
			string lastString = title.Substring (linkIndex);

			String mailtoAddress = String.Format ("mailto:{0}?Subject={1}&body={2}", subtitle, subject, body);
			NSString hyperlink = new NSString (mailtoAddress);
			NSColor linkTextColor = Utility.ColorWithHexColorValue ("#0000ff", 0.85f);//("#0080fc", 0.85f);
			NSObject[] aobjects = new NSObject[] {
				font,
				linkTextColor,
				hyperlink,
				linkTextColor,
				NSObject.FromObject (NSUnderlineStyle.Single)
			};
			NSObject[] akeys = new NSObject[] { NSStringAttributeKey.Font, 
				NSStringAttributeKey.ForegroundColor,
				NSStringAttributeKey.Link,
				NSStringAttributeKey.StrikethroughColor,
				NSStringAttributeKey.UnderlineStyle
			};
			NSDictionary linkAttributes = NSDictionary.FromObjectsAndKeys (aobjects, akeys);

			NSObject[] objects = new NSObject[] { font, fontColor };
			NSObject[] keys = new NSObject[] { NSStringAttributeKey.Font, 
				NSStringAttributeKey.ForegroundColor
			};
			NSDictionary attributes = NSDictionary.FromObjectsAndKeys (objects, keys);

			NSMutableAttributedString linKAttributedString = new NSMutableAttributedString (title);
			linKAttributedString.BeginEditing ();
			linKAttributedString.AddAttributes (attributes, new NSRange (0, index));
			linKAttributedString.AddAttributes (linkAttributes, new NSRange (index, subtitle.Length));
			if (!string.IsNullOrWhiteSpace (lastString)) {
				linKAttributedString.AddAttributes (attributes, new NSRange (linkIndex, lastString.Length));
			}
			linKAttributedString.EndEditing ();

			return linKAttributedString;
		}
			
		public static nfloat HeightWrappedToWidth (string title, nfloat fontSize, nfloat width)
		{
			NSAttributedString textAttrStr = new NSAttributedString (title, NSFont.SystemFontOfSize (fontSize));
			var maxSize = new CGSize (width, 1000);
			CGRect boundRect = textAttrStr.BoundingRectWithSize (maxSize,
				                   NSStringDrawingOptions.TruncatesLastVisibleLine |
				                   NSStringDrawingOptions.UsesLineFragmentOrigin |
				                   NSStringDrawingOptions.UsesFontLeading);

			//multiple of 17
			nfloat stringHeight = boundRect.Height;
			return stringHeight;
		}

		public static CGPoint GetModalPanelLocation(nfloat nModalWidth, nfloat nModalHeight) 
		{
			//nfloat nModalWidth = 690.0f;
			//nfloat nModalHeight = 680.0f;
			var mainWindow = NSApplication.SharedApplication.MainWindow;
			if (mainWindow == null) {
				CGRect frame = NSScreen.MainScreen.VisibleFrame;
				nfloat orgy = frame.Height - nModalHeight;
				nfloat orgx = (frame.Width - nModalWidth) / 2;
				return new CGPoint (orgx,orgy);
			}
			CGRect frameRect = mainWindow.Frame;
			nfloat barHeight = frameRect.Height-mainWindow.ContentView.Frame.Height;
			CGPoint orgPoint = frameRect.Location;
			orgPoint.Y = frameRect.Bottom-nModalHeight-barHeight;
			orgPoint.X = frameRect.X + (frameRect.Width - nModalWidth) / 2;
			return orgPoint;
		}

		public static nfloat TitleBarHeight ()
		{
			var mainWindow = NSApplication.SharedApplication.MainWindow;
			CGRect frameRect = mainWindow.Frame;
			nfloat barHeight = frameRect.Height-mainWindow.ContentView.Frame.Height;
			return barHeight;
		}

		public static bool IsKeyWindowLogin ()
		{
			if (NSApplication.SharedApplication.KeyWindow == null) {
				return false;
			}
			string title = NSApplication.SharedApplication.KeyWindow.Class.Name;
			return title == "LoginWindow" || title == "ChangePasswordWindow" || title == "ResetPasswordWindow" ? true : false;
		}

		public static bool IsKeyWindowByTitle (string title)  //PublicationInfoPanel "PublicationsWindow"
		{
			string classTitle = NSApplication.SharedApplication.KeyWindow.Class.Name;
			//Console.WriteLine ("{0}",title);
			return classTitle == title? true : false;
		}

		public  static string CurrentUserInfo ()
		{
			LoginUserDetails userDetail = GlobalAccess.Instance.CurrentUserInfo;
			string userInfo = userDetail.Country.CountryCode + "." + userDetail.Email;

			return userInfo;
		}

		public static string GetAppCacheAbsolutePath()
		{
			var cachepath = NSSearchPath.GetDirectories(NSSearchPathDirectory.CachesDirectory,NSSearchPathDomain.User);
			var hideDirPath = NSBundle.MainBundle.BundleIdentifier;
			string appRootPath = cachepath[0]+"/"+hideDirPath+"/";
			return appRootPath;
		}

		public static string GetAppUserAbsolutePath()
		{
			var cachepath = NSSearchPath.GetDirectories(NSSearchPathDirectory.CachesDirectory,NSSearchPathDomain.User);
			var hideDirPath = NSBundle.MainBundle.BundleIdentifier;
			string appRootPath = cachepath[0]+"/"+hideDirPath+"/";

			LoginUserDetails userDetail = GlobalAccess.Instance.CurrentUserInfo;
			appRootPath = appRootPath + userDetail.UserFolder;
			return appRootPath;
		}

		public static string FormateLastReadDate(DateTime dateTime)
		{
			return dateTime.ToString("h:mm")
				+ dateTime.ToString("tt").ToLower()
				+ dateTime.ToString(", d MMM yyyy");
		}

		#region mark legal define
		public static bool IsValidDictionary() {
			LoginUserDetails userDetail = GlobalAccess.Instance.CurrentUserInfo;
			if (userDetail.Country.CountryCode == "AU" ||
				userDetail.Country.CountryCode == "NZ") {
				return true;
			} else {
				return false;
			}
		}

		public static string EscapeSpace(string urlString)
		{
			var escapeString = urlString.Replace ("%20", " ");
			return escapeString;
		}

		public static string UrlUTF8Encode (string urlString)
		{
			var escapeString = urlString.Replace (",", @"%2C");
			escapeString = escapeString.Replace (";", @"%3B");

			return escapeString;
		}

		public static string VerifyLegalTerm (string term)
		{
			var verifyTerm = term.Trim ();
			verifyTerm = RemoveApostrophes (verifyTerm);
			verifyTerm = ToSingular (verifyTerm);
			verifyTerm = verifyTerm.Trim ();
			return verifyTerm;
		}

		public static string ToSingular(string word)
		{
			Regex plural1 = new Regex("(?<keep>[^aeiou])ies$");
			Regex plural2 = new Regex("(?<keep>[aeiou]y)s$");
			Regex plural3 = new Regex("(?<keep>[sxzh])es$");
			Regex plural4 = new Regex("(?<keep>[^sxzhyu])s$");

			if (plural1.IsMatch(word))
				return plural1.Replace(word, "${keep}y");
			else if (plural2.IsMatch(word))
				return plural2.Replace(word, "${keep}");
			else if (plural3.IsMatch(word))
				return plural3.Replace(word, "${keep}");
			else if (plural4.IsMatch(word))
				return plural4.Replace(word, "${keep}");

			return word;
		}

		public static string RemoveApostrophes(string term)
		{
			string word = term.Replace ("’s", "");  //person’s
			word = term.Replace ("'s", "");
			word = term.Replace ("’s", "");

			//remove special characters
			string regEx="[`~!@#$%^&*()+=|{}':;',\\[\\].<>/?~！@#￥%……&*（）——+|{}【】‘；：”“’。，、？]"; 
			Regex reg = new Regex(regEx, RegexOptions.IgnoreCase);
			word = reg.Replace (word, "");
			return word;
		}
		#endregion

		public static PublicationsWindowController GetMainWindowConroller() {
			var NSApp = NSApplication.SharedApplication;
			//var winController = (PublicationsWindowController)NSApp.MainWindow.WindowController;
			var windowController = (PublicationsWindowController)((AppDelegate)NSApp.Delegate).publicationsWindowController;
			return windowController;
		}

		public static NSImage CreateImageWithColor(string colorValue)
		{
			NSGraphicsContext.GlobalSaveGraphicsState ();
			CGSize size = new CGSize(12, 12);
			NSImage tintImage = new NSImage (size);
			tintImage.LockFocus ();

			float cornerRadius = 5f;
			CGRect rect = new CGRect (0,0,10,10);
			NSBezierPath path = NSBezierPath.FromRoundedRect (rect,cornerRadius,cornerRadius); 
			if (string.IsNullOrEmpty (colorValue)) {
				NSColor.Grid.Set ();
				path.Stroke ();
			} else {
				Utility.ColorWithHexColorValue (colorValue, 1.0f).SetFill ();
				path.Fill ();
			}

			tintImage.UnlockFocus ();
			CGContext context = NSGraphicsContext.CurrentContext.CGContext;

			return tintImage;
		}
	}
}

