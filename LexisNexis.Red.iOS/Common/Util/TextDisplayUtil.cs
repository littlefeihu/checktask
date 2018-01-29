using System;
using System.Text;
using System.Collections.Generic;

using Foundation;
using UIKit;
using CoreGraphics;

namespace LexisNexis.Red.iOS
{
	public static class TextDisplayUtil
	{
		/// <summary>
		/// Gets the string bound rect.
		/// </summary>
		/// <returns>The string bound rect.</returns>
		/// <param name="text">Text.</param>
		/// <param name="font">Font.</param>
		/// <param name="maxSize">Max size.</param>
		public static CGSize GetStringBoundRect(string text, UIFont font, CGSize maxSize)
		{
			if (text != null && font != null) {
				NSAttributedString textAttrStr = new NSAttributedString (text,font);
				CGRect boundRect = textAttrStr.GetBoundingRect (maxSize,
					NSStringDrawingOptions.UsesLineFragmentOrigin | NSStringDrawingOptions.UsesFontLeading , null);
				return new CGSize (Math.Ceiling (boundRect.Size.Width), Math.Ceiling (boundRect.Size.Height));
			}


			return new CGSize (0, 0);
		}

		public static string Base64Encode(string str)
		{
			string result = null;
			if (str != null && str.Length > 0) {
				Encoding encode = Encoding.UTF8;
				byte[] byteData = encode.GetBytes (str);
				result =  Convert.ToBase64String (byteData, 0, byteData.Length);
			}
			return result;

		}


		/// <summary>
		/// Gets the highlighted attributed string.
		/// </summary>
		/// <returns>The highlighted attributes string.</returns>
		/// <param name="originStr">Origin string which contains keyword need to be highlighted</param>
		/// <param name="keywords">Keywords to be highlighted in origin str.</param>
		public static NSMutableAttributedString GetHighlightedString (string originStr, List<string> keywords)
		{
			var attrStr = new NSMutableAttributedString (originStr);
			var highlightStrAttribute = new UIStringAttributes {
				Font = UIFont.BoldSystemFontOfSize(14)
			};

			foreach (var keyword in keywords) {
				var ranges = FindRangesOfStringInText (keyword, originStr);
				foreach (var range in ranges) {
					attrStr.SetAttributes (highlightStrAttribute, range);

				}
			}

			return attrStr;
		}

		/// <summary>
		/// Finds all the ranges of string in text.
		/// if either of str or text(params) is null, then return empty List<NSRange>
 		/// </summary>
		/// <returns>The ranges of string in text.</returns>
		/// <param name="str">String.</param>
		/// <param name="text">Text.</param>
		private static List<NSRange> FindRangesOfStringInText(string str, string text)
		{
			List<NSRange> ranges = new List<NSRange> ();

			if (str != null && text != null) {
				int startIndex = 0;
				int index = text.IndexOf (str, startIndex);
				while(index != -1){
					ranges.Add(new NSRange(index, str.Length));
					startIndex = index + str.Length;
					index = text.IndexOf (str, startIndex );
				}
			}

			return ranges;
		}
	}
}

