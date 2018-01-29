using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using LexisNexis.Red.Common.Entity;
using Android.Graphics;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Common.HelpClass;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.ContentPage
{
	public class TOCListAdaptorViewHolder : RecyclerView.ViewHolder
	{
		private readonly Color DefaultBackgroundColor = Color.White;

		private static readonly Color MaxLevelColor = Color.ParseColor("#FF0000");
		private static readonly Color BackgroundColor = Color.ParseColor("#ffffff");
		private static readonly Color CurrentTOCTextColor = Color.ParseColor("#ffffff");
		private static readonly Color DefaultTOCTextColor = Color.ParseColor("#de000000");
		private const float FirstLevelAlpha = 0.3f;
		private const float MaxLevelAlpha = 1.0f;

		private static Color BlendColor(int level, int totalLevel)
		{
			float ratio = FirstLevelAlpha + (MaxLevelAlpha - FirstLevelAlpha) * ((float)level) / ((float)totalLevel);
			float inverseRatio = 1.0f - ratio;

			float r = (MaxLevelColor.R * ratio) + (BackgroundColor.R * inverseRatio);
			float g = (MaxLevelColor.G * ratio) + (BackgroundColor.G * inverseRatio);
			float b = (MaxLevelColor.B * ratio) + (BackgroundColor.B * inverseRatio);

			return Color.Rgb((int)r, (int)g, (int)b);
		}

		private readonly TOCListAdaptor adaptor;

		private LinearLayout llLevelMark;
		private LinearLayout llBackground;
		private LinearLayout llDivider;
		private TextView tvTitle;
		private ImageView ivIcon;

		public TOCNode Node
		{
			set;
			get;
		}

		public TOCListAdaptorViewHolder(View v, TOCListAdaptor adaptor): base(v)
		{
			this.adaptor = adaptor;

			llLevelMark = v.FindViewById<LinearLayout>(Resource.Id.llLevelMark);
			llBackground = v.FindViewById<LinearLayout>(Resource.Id.llBackground);
			llDivider = v.FindViewById<LinearLayout>(Resource.Id.llDivider);
			tvTitle = v.FindViewById<TextView>(Resource.Id.tvTitle);
			ivIcon = v.FindViewById<ImageView>(Resource.Id.ivIcon);

			llBackground.SetOnClickListener(new ItemClickListener(this));
		}

		private class ItemClickListener: Java.Lang.Object, View.IOnClickListener
		{
			private readonly TOCListAdaptorViewHolder vh;

			public ItemClickListener(TOCListAdaptorViewHolder vh)
			{
				this.vh = vh;
			}

			public void OnClick(View v)
			{
				if(DataCache.INSTATNCE.Toc == null)
				{
					return;
				}

				if(vh.Node == null)
				{
					// The node is "Table of Contents"
					if(DataCache.INSTATNCE.Toc.CurrentTOCNode != null)
					{
						vh.adaptor.SetCurrentNode(
							GetTopLevelNode(DataCache.INSTATNCE.Toc.CurrentTOCNode),
							false);
					}

					return;
				}

				if(vh.Node.IsParent())
				{
					var isExpanded = DataCache.INSTATNCE.Toc.IsExpanded(vh.Node, vh.adaptor.NodeList);
					// switch the expanded status of current node
					vh.adaptor.SetCurrentNode(vh.Node, !isExpanded);
				}
				else
				{
					if(DataCache.INSTATNCE.Toc.IsCurrentNode(vh.Node))
					{
						return;
					}

					var record = DataCache.INSTATNCE.Toc.GetNavigationItem();
					if(record == null
						|| !NavigationManagerHelper.CompareActualTocId(NavigationManagerHelper.ContentsTabGetTocId(record), vh.Node.ID))
					{
						NavigationManager.Instance.AddRecord(
							new ContentBrowserRecord(
								DataCache.INSTATNCE.Toc.Publication.BookId,
								vh.Node.ID,
								0));
					}

					DataCache.INSTATNCE.Toc.CurrentTOCNode = vh.Node;
					DataCache.INSTATNCE.Toc.BindNavigationItem();

					vh.adaptor.Activity.GetMainFragment().OpenContentPage();
					vh.adaptor.NotifyDataSetChanged();
				}
			}
		}

		public void Update()
		{
			if(Node == null)
			{
				// The node is "Table of Contents"
				llDivider.Visibility = ViewStates.Invisible;
				ivIcon.Visibility = ViewStates.Invisible;

				llLevelMark.SetBackgroundColor(GetColor());

				tvTitle.SetTextSize(Android.Util.ComplexUnitType.Sp, 14);
				tvTitle.Typeface = Typeface.Create(
					MainApp.ThisApp.Resources.GetString(Resource.String.FontFamily_Roboto_Medium),
					TypefaceStyle.Bold);
				tvTitle.Text = MainApp.ThisApp.Resources.GetString(Resource.String.TOCPanel_TableOfContents);
				tvTitle.SetTextColor(DefaultTOCTextColor);

				llBackground.SetBackgroundColor(DefaultBackgroundColor);
			}
			else
			{
				llDivider.Visibility = ViewStates.Visible;
				ivIcon.Visibility = ViewStates.Visible;

				llLevelMark.SetBackgroundColor(GetColor());

				tvTitle.SetTextSize(Android.Util.ComplexUnitType.Sp, 16);
				tvTitle.Typeface = Typeface.Create(
					MainApp.ThisApp.Resources.GetString(Resource.String.FontFamily_Roboto_Regular),
					TypefaceStyle.Normal);
				tvTitle.Text = Node.Title;

				if(Node.IsParent())
				{
					ivIcon.SetImageResource(
						DataCache.INSTATNCE.Toc.IsExpanded(Node, adaptor.NodeList)
						? Resource.Drawable.toc_expanded
						: Resource.Drawable.toc_collaped);
				}
				else
				{
					ivIcon.SetImageResource(
						DataCache.INSTATNCE.Toc.IsCurrentNode(Node)
						? Resource.Drawable.toc_page_white : Resource.Drawable.toc_page_grey);
				}

				if(DataCache.INSTATNCE.Toc.IsCurrentNode(Node))
				{
					tvTitle.SetTextColor(CurrentTOCTextColor);
					llBackground.SetBackgroundColor(GetColor());
				}
				else
				{
					tvTitle.SetTextColor(DefaultTOCTextColor);
					llBackground.SetBackgroundColor(DefaultBackgroundColor);
				}
			}
		}

		private Color GetColor()
		{
			// If the Node == null, it is "Table of Contents" node
			if(DataCache.INSTATNCE.Toc == null)
			{
				Android.Util.Log.Debug("dbg", "Toc is null!!!");
			}

			return BlendColor(
				Node == null ? 0 : Node.NodeLevel, 
				DataCache.INSTATNCE.Toc.GetMaxLevel(adaptor.NodeList));
		}

		private static TOCNode GetTopLevelNode(TOCNode node)
		{
			while(node.NodeLevel != 1)
			{
				node = node.ParentNode;
			}

			return node;
		}
	}
}

