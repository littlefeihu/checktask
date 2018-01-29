using System;

namespace LexisNexis.Red.iOS
{
	public static class ContentFormatUtil
	{

		public static string Format(string content)
		{
			if (!string.IsNullOrEmpty (content)) {
				content = FormatImageUrl (content);
			}

			return content;
		}

		private static string FormatImageUrl (string content)
		{
			if (!string.IsNullOrEmpty (content)) {
				content = content.Replace ("\"cbcc.gif\"", "\"../Images/Content/cbcc.gif\"");
			}

			return content;
		}
	}
}

