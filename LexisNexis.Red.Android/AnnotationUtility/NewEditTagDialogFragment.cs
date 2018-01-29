using Android.OS;
using Android.Views;
using DialogFragment=Android.Support.V4.App.DialogFragment;
using System.Collections.Generic;
using System;
using Android.Widget;
using Android.Graphics.Drawables;
using LexisNexis.Red.Droid.Utility;
using Android.Graphics;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Droid.App;

namespace LexisNexis.Red.Droid.AnnotationUtility
{
	public class NewEditTagDialogFragment : DialogFragment
	{
		public const string NewEditTagDialogFragmentTag = "newEditTagDialogFragment";

		private const string SelectedTagColorKey = "SelectedTagColor";
		private const string TagIdKey = "TagIdKey";
		private const string TagNameKey = "TagNameKey";
		private const string TagColorKey = "TagColorKey";

		private readonly ImageView[] ivTagColors = new ImageView[12];
		private readonly static Color[] TagColors;

		private EditText etTagName;

		static NewEditTagDialogFragment()
		{
			var colors = AnnCategoryTagUtil.Instance.GetTagColors();
			TagColors = new Color[colors.Count];
			for(int i = 0; i < colors.Count; ++i)
			{
				TagColors[i] = Color.ParseColor(colors[i].ColorValue);
			}
		}

		public static NewEditTagDialogFragment NewInstance(string tagId = null, string tagName = null, string tagColor = null)
		{
			var b = new Bundle();
			var fragment = new NewEditTagDialogFragment();
			fragment.Arguments = b;
			if(!string.IsNullOrEmpty(tagId)
				&& !string.IsNullOrEmpty(tagName)
				&& !string.IsNullOrEmpty(tagColor))
			{
				b.PutString(TagIdKey, tagId);
				b.PutString(TagNameKey, tagName);
				b.PutString(TagColorKey, tagColor);
			}

			return fragment;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			Dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
			Dialog.SetCanceledOnTouchOutside(true);

			var vwDialog = inflater.Inflate(Resource.Layout.annotation_newtag_popup, container);

			etTagName = vwDialog.FindViewById<EditText>(Resource.Id.etTagName);

			ivTagColors[0] = vwDialog.FindViewById<ImageView>(Resource.Id.ivTagColor0);
			ivTagColors[1] = vwDialog.FindViewById<ImageView>(Resource.Id.ivTagColor1);
			ivTagColors[2] = vwDialog.FindViewById<ImageView>(Resource.Id.ivTagColor2);
			ivTagColors[3] = vwDialog.FindViewById<ImageView>(Resource.Id.ivTagColor3);
			ivTagColors[4] = vwDialog.FindViewById<ImageView>(Resource.Id.ivTagColor4);
			ivTagColors[5] = vwDialog.FindViewById<ImageView>(Resource.Id.ivTagColor5);

			ivTagColors[6] = vwDialog.FindViewById<ImageView>(Resource.Id.ivTagColor6);
			ivTagColors[7] = vwDialog.FindViewById<ImageView>(Resource.Id.ivTagColor7);
			ivTagColors[8] = vwDialog.FindViewById<ImageView>(Resource.Id.ivTagColor8);
			ivTagColors[9] = vwDialog.FindViewById<ImageView>(Resource.Id.ivTagColor9);
			ivTagColors[10] = vwDialog.FindViewById<ImageView>(Resource.Id.ivTagColor10);
			ivTagColors[11] = vwDialog.FindViewById<ImageView>(Resource.Id.ivTagColor11);

			vwDialog.FindViewById<Button>(Resource.Id.btnCancel).Click += delegate
			{
				Dismiss();

				EditTagsDialogFragment.NewInstance().Show(
					Activity.SupportFragmentManager.BeginTransaction(),
					EditTagsDialogFragment.EditTagsDialogFragmentTag);
			};

			vwDialog.FindViewById<Button>(Resource.Id.btnOk).Click += delegate
			{
				if(string.IsNullOrEmpty(etTagName.Text))
				{
					Toast.MakeText(
						Activity,
						Resource.String.AnnotationNewTagPage_PleaseInputTagName,
						ToastLength.Short).Show();
					return;
				}

				var tagId = Arguments.GetString(TagIdKey);
				if(string.IsNullOrEmpty(tagId))
				{
					AnnCategoryTagUtil.Instance.AddTag(
						etTagName.Text,
						AnnCategoryTagUtil.Instance.GetTagColors()[GetSelectedTagColorIndex()].ColorValue);
				}
				else
				{
					AnnCategoryTagUtil.Instance.UpdateTag(
						Guid.Parse(tagId),
						etTagName.Text,
						AnnCategoryTagUtil.Instance.GetTagColors()[GetSelectedTagColorIndex()].ColorValue);
				}

				Dismiss();

				// Notify Activity to update tag list
				var updateListener = Activity as ITagListUpdateListener;
				if(updateListener != null)
				{
					updateListener.OnTagListUpdated();
				}

				EditTagsDialogFragment.NewInstance().Show(
					Activity.SupportFragmentManager.BeginTransaction(),
					EditTagsDialogFragment.EditTagsDialogFragmentTag);
			};

			for(int i = 0; i < ivTagColors.Length; ++i)
			{
				ivTagColors[i].Click += delegate(object sender, EventArgs e)
				{
					var id = ((View)sender).Id;
					var index = 0;
					for(int j = 0; j < ivTagColors.Length; ++j)
					{
						if(id == ivTagColors[j].Id)
						{
							index = j;
						}
					}

					SetSelectedTagColorIndex(index);
					UpdateTagColors();
				};
			}

			var tagName = Arguments.GetString(TagNameKey);
			if(!string.IsNullOrEmpty(tagName))
			{
				vwDialog.FindViewById<TextView>(Resource.Id.tvTitle).Text =
					MainApp.ThisApp.Resources.GetString(Resource.String.AnnotationNewTagPage_EditTag);
				etTagName.Text = tagName;
				var color = Color.ParseColor(Arguments.GetString(TagColorKey));
				for(int i = 0; i < TagColors.Length; ++i)
				{
					if(TagColors[i] == color)
					{
						SetSelectedTagColorIndex(i);
						break;
					}
				}
			}

			UpdateTagColors();

			return vwDialog;
		}

		private void UpdateTagColors()
		{
			var selectedIndex = GetSelectedTagColorIndex();
			for(int i = 0; i < ivTagColors.Length; ++i)
			{
				var drawable = (GradientDrawable)Resources.GetDrawable(Resource.Drawable.tag_selectcolor_round_icon);
				drawable.SetStroke(Conversion.Dp2Px(5), TagColors[i]);
				drawable.SetColor(selectedIndex == i ? TagColors[i] : Color.White);
				ivTagColors[i].SetImageDrawable(drawable);
			}
		}

		private int GetSelectedTagColorIndex()
		{
			return Arguments.GetInt(SelectedTagColorKey, 0);
		}

		private void SetSelectedTagColorIndex(int index)
		{
			Arguments.PutInt(SelectedTagColorKey, index);
		}
	}
}

