using System;
using Android.Support.V7.Widget;
using LexisNexis.Red.Droid.Widget.DraggableList;
using Android.Views;
using Android.Widget;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Droid.AlertDialogUtility;
using LexisNexis.Red.Common.Business;

namespace LexisNexis.Red.Droid.SettingsBoardPage
{
	public class DraggablePublicationListViewHolder: RecyclerView.ViewHolder, DraggableRecyclerView.DraggableViewHolder
	{
		private readonly RecyclerView.Adapter adapter;

		private readonly TextView tvTitle;
		private readonly ImageView ivLeftDelete;
		private readonly ImageView ivRightDelete;
		private readonly ImageView ivMoveHandler;

		public ObjHolder<Publication> Publication
		{
			get;
			set;
		}

		public DraggablePublicationListViewHolder(View v, RecyclerView.Adapter adapter): base(v)
		{
			this.adapter = adapter;

			tvTitle = v.FindViewById<TextView> (Resource.Id.tvTitle);
			ivLeftDelete = v.FindViewById<ImageView>(Resource.Id.ivLeftDelete);
			ivRightDelete = v.FindViewById<ImageView>(Resource.Id.ivRightDelete);
			ivMoveHandler = v.FindViewById<ImageView>(Resource.Id.ivMoveHandler);

			ivLeftDelete.Click += OnDeleteClicked;
			ivRightDelete.Click += OnDeleteClicked;
		}

		void OnDeleteClicked (object sender, EventArgs e)
		{
			// raise a dialog;
			DialogGenerator.ShowMessageDialog(
				((DraggablePublicationListAdapter)adapter).Activity.SupportFragmentManager,
				Resource.String.OrganisePub_DeletePubWarning_Title,
				Resource.String.OrganisePub_DeletePubWarning_Message,
				Resource.String.Delete,
				0,
				OrganisePublicationsFragment.DeletePubWarningDialog,
				Publication.Value.BookId.ToString());
		}

		public void Update()
		{
			tvTitle.Text = Publication.Value.Name;
			ivLeftDelete.Visibility = ViewStates.Gone;
			ivRightDelete.Visibility = ViewStates.Gone;
		}

		public void Swip(bool left)
		{
			if(left)
			{
				// to left
				if(ivLeftDelete.Visibility == ViewStates.Visible)
				{
					ivLeftDelete.Visibility = ViewStates.Gone;
					ivMoveHandler.Visibility = ViewStates.Visible;
				}
				else if(ivRightDelete.Visibility == ViewStates.Gone)
				{
					ivRightDelete.Visibility = ViewStates.Visible;
					ivMoveHandler.Visibility = ViewStates.Gone;
				}
			}
			else
			{
				// to right
				if(ivRightDelete.Visibility == ViewStates.Visible)
				{
					ivRightDelete.Visibility = ViewStates.Gone;
					ivMoveHandler.Visibility = ViewStates.Visible;
				}
				else if(ivLeftDelete.Visibility == ViewStates.Gone)
				{
					ivLeftDelete.Visibility = ViewStates.Visible;
					ivMoveHandler.Visibility = ViewStates.Gone;
				}
			}
		}

		public bool CanDrag(Android.Graphics.Rect viewRect, int longPressX, int longPressY)
		{
			return ivMoveHandler.Visibility == ViewStates.Visible
				&& viewRect.Right - longPressX < 64;
		}
	}
}

