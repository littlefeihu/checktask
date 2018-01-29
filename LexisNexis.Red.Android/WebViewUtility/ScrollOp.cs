using System;
using Newtonsoft.Json;

namespace LexisNexis.Red.Droid.WebViewUtility
{
	public class ScrollOp
	{
		public const string TypeTop = "top";
		public const string TypeHighLight = "highlight";
		public const string TypePboPage = "pbopage";
		public const string TypeRefpt = "refpt";
		public const string TypeIndex = "index";

		[JsonProperty("type")]
		public string Type;

		[JsonProperty("tocid")]
		public string TocId;

		[JsonProperty("headtype")]
		public string HeadType;

		[JsonProperty("headsequence")]
		public int HeadSequence;

		[JsonProperty("pagenum")]
		public int PageNum;

		[JsonProperty("refptid")]
		public string RefptId;

		[JsonProperty("title")]
		public string Title;

		public string Serialize()
		{
			return JsonConvert.SerializeObject(this);
		}
	}
}

