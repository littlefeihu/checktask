using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Text.Method;
using Android.Text;
using Android.Text.Style;
using Android.Graphics;
using Android.Util;
using LexisNexis.Red.Droid.TextStyle;
using Android.Text.Util;

namespace LexisNexis.Red.Droid.Widget.ExpandableTextView
{
	public class ExpandableTextViewUtilty: Java.Lang.Object, ViewTreeObserver.IOnGlobalLayoutListener, View.IOnClickListener
	{
		private const float ViewMoreLessProportion = 1.0f;
		private const string Apostrophe = "...";
		private readonly TextView tv;

		public ExpandableTextViewUtilty(TextView tv)
		{
			this.tv = tv;
		}

		public static void MakeTextViewResizable(
			TextView tv, int maxLines, ISimpleExpandableTextViewListerner listerner,
			AutoLinkOptions autoLinkOptions, ViewMoreLessOptions viewMoreLessOptions, bool clickSwitch, bool isPartial = true)
		{
			tv.Tag = new ExpandableTextViewTag()
			{
				Text = tv.Text,
				MaxLines = maxLines,
				ClickSwitch = clickSwitch,
				IsPartial = isPartial,
				Listerner = listerner,
				AutoLinkOptions = autoLinkOptions,
				ViewMoreLessOptions = viewMoreLessOptions,
			};

			init(tv);
		}

		public static void MakeTextViewResizable(
			TextView tv, int maxLines, ISimpleExpandableTextViewListerner listerner,
			AutoLinkOptions autoLinkOptions, bool isPartial = true)
		{
			tv.Tag = new ExpandableTextViewTag()
			{
				Text = tv.Text,
				MaxLines = maxLines,
				ClickSwitch = false,
				IsPartial = isPartial,
				Listerner = listerner,
				AutoLinkOptions = autoLinkOptions,
				ViewMoreLessOptions = null,
			};

			init(tv);
		}

		private static void init(TextView tv)
		{
			var tag = tv.Tag as ExpandableTextViewTag;
			tag.LineCount = -1;
			tag.FormattedText = null;

			tv.ViewTreeObserver.AddOnGlobalLayoutListener(new ExpandableTextViewUtilty(tv));
			tv.MovementMethod = LinkMovementMethod.Instance;
		}

		public static bool IsPartial(TextView tv)
		{
			var tag = tv.Tag as ExpandableTextViewTag;
			if(tag == null)
			{
				return true;
			}

			return tag.IsPartial;
		}

		public void OnClick(View v)
		{
			Switch((TextView)v);
		}

		public void OnGlobalLayout()
		{
			var tag = tv.Tag as ExpandableTextViewTag;
			if(tag == null)
			{
				throw new InvalidOperationException("Unable to handle a TextView without Tag.");
			}

			ViewTreeObserver obs = tv.ViewTreeObserver;
			obs.RemoveOnGlobalLayoutListener(this);

			if(tag.LineCount < 0)
			{
				tag.LineCount = tv.LineCount;
				if(tag.FormattedText == null)
				{
					var spanned = tv.TextFormatted as ISpannable;
					if(spanned != null)
					{
						tag.FormattedText = spanned;
					}

					if(tag.AutoLinkOptions != null)
					{
						SpannableStringBuilder ssb =
							tag.FormattedText == null
								? new SpannableStringBuilder(tv.Text)
								: new SpannableStringBuilder(tag.FormattedText);

						Linkify.AddLinks(ssb, tag.AutoLinkOptions.AutoLinkMask);

						if(!tag.AutoLinkOptions.IsLinkUnderline)
						{
							FixedTextUtils.RemoveLinkUnderline(ssb);
						}

						tag.FormattedText = ssb;
					}
				}
			}

			if(tag.IsPartial)
			{
				if(tv.LineCount > tag.MaxLines)
				{
					int lineStartIndex = tv.Layout.GetLineStart(tag.MaxLines - 1);
					int lineEndIndex = tv.Layout.GetLineEnd(tag.MaxLines - 1);

					int remainLineWidth;
					if(tag.ViewMoreLessOptions == null)
					{
						Rect apostropheRect = new Rect();
						tv.Paint.GetTextBounds(Apostrophe, 0, Apostrophe.Length, apostropheRect);
						remainLineWidth =
							tv.Width
						- (apostropheRect.Width() * 5 / 3)
						- tv.TotalPaddingLeft - tv.TotalPaddingRight;
					}
					else
					{
						Rect viewMoreRect = new Rect();
						tv.Paint.GetTextBounds(tag.ViewMoreLessOptions.ViewMore, 0, tag.ViewMoreLessOptions.ViewMore.Length, viewMoreRect);
						Rect apostropheRect = new Rect();
						tv.Paint.GetTextBounds(Apostrophe, 0, Apostrophe.Length, apostropheRect);
						remainLineWidth =
							tv.Width
						- (int)Math.Round(viewMoreRect.Width() * (1.0f + 3.0f / (float)(tag.ViewMoreLessOptions.ViewMore.Length)) * ViewMoreLessProportion)
						- apostropheRect.Width()
						- tv.TotalPaddingLeft - tv.TotalPaddingRight;
					}

					String test = tv.Text.Substring(lineStartIndex, lineEndIndex - lineStartIndex);
					int returnIndex = test.IndexOf('\r');
					if(returnIndex < 0)
					{
						returnIndex = test.IndexOf('\n');
					}

					if(returnIndex >= 0)
					{
						test = test.Substring(0, returnIndex);
					}

					Rect testRect = new Rect();
					while(test.Length > 0)
					{
						tv.Paint.GetTextBounds(test, 0, test.Length, testRect);
						if(testRect.Width() < remainLineWidth)
						{
							break;
						}

						test = test.Substring(0, test.Length - 1);
					}

					if(tag.ViewMoreLessOptions == null)
					{
						if(tag.FormattedText == null)
						{
							tv.Text = tv.Text.Substring(0, lineStartIndex + test.Length) + Apostrophe;
						}
						else
						{
							var ssbPartial = new SpannableStringBuilder(
								                 tag.FormattedText.SubSequenceFormatted(0, lineStartIndex + test.Length));
							ssbPartial.Append(Apostrophe);
							tv.TextFormatted = ssbPartial;
						}
					}
					else
					{
						if(tag.FormattedText == null)
						{
							String text = tv.Text.Substring(0, lineStartIndex + test.Length) + Apostrophe + " ";
							var ssb = new SpannableStringBuilder(text);
							ssb.Append(StyleViewMoreLess(tag));
							tv.TextFormatted = ssb;
						}
						else
						{
							var ssbPartial = new SpannableStringBuilder(
								                 tag.FormattedText.SubSequenceFormatted(0, lineStartIndex + test.Length));
							ssbPartial.Append(Apostrophe + " ");
							ssbPartial.Append(StyleViewMoreLess(tag));
							tv.TextFormatted = ssbPartial;
						}
					}
					tv.ScrollTo(0, 0);

					if(tag.ClickSwitch)
					{
						tv.SetOnClickListener(this);
					}
				}
				else if(tag.AutoLinkOptions != null)
				{
					tv.TextFormatted = tag.FormattedText;
				}
			}
			else
			{
				if(tag.ViewMoreLessOptions == null)
				{
					if(tag.FormattedText == null)
					{
						tv.Text = tag.Text;
					}
					else
					{
						tv.TextFormatted = tag.FormattedText;
					}
				}
				else
				{
					if(tag.FormattedText == null)
					{
						String text = tag.Text + " ";
						var ssb = new SpannableStringBuilder(text);
						ssb.Append(StyleViewMoreLess(tag));
						tv.TextFormatted = ssb;
					}
					else
					{
						var ssb = new SpannableStringBuilder(tag.FormattedText);
						ssb.Append(" ");
						ssb.Append(StyleViewMoreLess(tag));
						tv.TextFormatted = ssb;
					}
				}

				tv.ScrollTo(0, 0);
			}

			if(tag.Listerner != null)
			{
				tag.Listerner.LineCountDetected(tv);
			}
		}



		private void ProcessAutoLink(ExpandableTextViewTag tag, SpannableStringBuilder ssbPartial)
		{
			if(tag.AutoLinkOptions != null && (!tag.AutoLinkOptions.IsLinkUnderline))
			{
				var allURLSpans = tag.FormattedText.GetSpans(
					0, tag.FormattedText.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));

				var partialURLSpans = ssbPartial.GetSpans(0, ssbPartial.Length(), Java.Lang.Class.FromType(typeof(URLSpan)));
				foreach(var span in partialURLSpans)
				{
					int startIndex = ssbPartial.GetSpanStart(span);
					int endIndex = ssbPartial.GetSpanEnd(span);
					var foundSpan = FixedTextUtils.FindSpan(tag.FormattedText, allURLSpans, startIndex).JavaCast<URLSpan>();
					ssbPartial.RemoveSpan(span);

					if(foundSpan != null)
					{
						ssbPartial.SetSpan(new NoUnderlineURLSpan(foundSpan.URL), startIndex, endIndex, 0);
					}
				}
			}
		}

		private static ISpannable StyleViewMoreLess(ExpandableTextViewTag tag)
		{
			string expandText = tag.IsPartial ? tag.ViewMoreLessOptions.ViewMore : tag.ViewMoreLessOptions.ViewLess;
			SpannableStringBuilder ssb = new SpannableStringBuilder(expandText);

			/*
			ssb.SetSpan(new ViewMoreLessSpan(delegate
			{
				tag.IsPartial = ! tag.IsPartial;
				tv.RequestLayout();
				tv.ViewTreeObserver.AddOnGlobalLayoutListener(new ExpandableTextViewUtilty(tv));
			}), text.Length - expandText.Length, text.Length, 0);
			*/

			ssb.SetSpan(new ForegroundColorSpan(Color.Red), 0, expandText.Length, 0);
			ssb.SetSpan(new RelativeSizeSpan(ViewMoreLessProportion), 0, expandText.Length, 0);

			return ssb;
		}

		public static bool ExceedMaxLines(TextView tv)
		{
			ExpandableTextViewTag tag = tv.Tag as ExpandableTextViewTag;
			if(tag == null)
			{
				throw new InvalidOperationException("Unable to handle a TextView without Tag.");
			}

			return tag.LineCount > tag.MaxLines;
		}

		public static void Switch(TextView tv)
		{
			ExpandableTextViewTag tag = tv.Tag as ExpandableTextViewTag;
			if(tag == null)
			{
				throw new InvalidOperationException("Unable to handle a TextView without Tag.");
			}

			tag.IsPartial = !tag.IsPartial;
			tv.RequestLayout();
			tv.ViewTreeObserver.AddOnGlobalLayoutListener(new ExpandableTextViewUtilty(tv));
		}

		public static bool IsExpandable(TextView tv)
		{
			ExpandableTextViewTag tag = tv.Tag as ExpandableTextViewTag;
			return tag != null;
		}
	}
}

