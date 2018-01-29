using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using Android.Views;
using Fragment=Android.Support.V4.App.Fragment;
using Android.Support.V7.Widget;
using LexisNexis.Red.Droid.AlertDialogUtility;
using LexisNexis.Red.Droid.Business;
using LexisNexis.Red.Common.Business;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Touchguard;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Draggable;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Animator;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Utils;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.SettingsBoardPage
{
	public class OrganisePublicationsFragment : Fragment, ISettingsBoardFragment, ISimpleDialogListener
	{
		public const string DeletePubWarningDialog = "DeletePubWarning";

		private RecyclerView rvPublicationList;

		private RecyclerViewTouchActionGuardManager tagm;
		private RecyclerViewDragDropManager ddm;
		private RecyclerViewSwipeManager sm;
		private GeneralItemAnimator ia;
		private RecyclerView.Adapter wrappedAdapter;
		private LinearLayoutManager llm;

		private OrganisePublicationsListAdaptor organisePublicationsListAdaptor;

		public string Title
		{
			get
			{
				return SettingsBoardActivity.GetTitle(SettingsBoardActivity.OrganisePublications);
			}
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RetainInstance = true;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.organise_publications_fragment, container, false);

			rvPublicationList = v.FindViewById<RecyclerView>(Resource.Id.rvPublicationList);

			llm = new LinearLayoutManager(Activity);
			llm.Orientation = LinearLayoutManager.Vertical;
			llm.ScrollToPosition(0);
			rvPublicationList.SetLayoutManager(llm);

			tagm = new RecyclerViewTouchActionGuardManager();
			tagm.SetInterceptVerticalScrollingWhileAnimationRunning(true);
			tagm.Enabled = true;

			ddm = new RecyclerViewDragDropManager();
			ddm.SetInitiateOnLongPress(true);
			ddm.SetInitiateOnMove(false);

			sm = new RecyclerViewSwipeManager();
			sm.UserHandleSwipeUi = true;

			if(organisePublicationsListAdaptor == null)
			{
				organisePublicationsListAdaptor = new OrganisePublicationsListAdaptor(OnDeletePub, OnSortPub);
			}

			organisePublicationsListAdaptor.SetPubList(DataCache.INSTATNCE.PublicationManager.PublicationList);

			wrappedAdapter = ddm.CreateWrappedAdapter(organisePublicationsListAdaptor);
			wrappedAdapter = sm.CreateWrappedAdapter(wrappedAdapter);

			ia = new SwipeDismissItemAnimator();

			rvPublicationList.SetAdapter(wrappedAdapter);
			rvPublicationList.SetItemAnimator(ia);

			tagm.AttachRecyclerView(rvPublicationList);
			sm.AttachRecyclerView(rvPublicationList);
			ddm.AttachRecyclerView(rvPublicationList);
			return v;
		}

		public override void OnDestroyView()
		{
			base.OnDestroyView();

			if(tagm != null)
			{
				tagm.Release();
				tagm = null;
			}

			if(ddm != null)
			{
				ddm.Release();
				ddm = null;
			}

			if(sm != null)
			{
				sm.Release();
				sm = null;
			}

			if(wrappedAdapter != null)
			{
				WrapperAdapterUtils.ReleaseAll(wrappedAdapter);
				wrappedAdapter = null;
			}

			ia = null;
			llm = null;
		}


		private void OnDeletePub(ObjHolder<Publication> pub)
		{
			DialogGenerator.ShowMessageDialog(
				Activity.SupportFragmentManager,
				Resource.String.OrganisePub_DeletePubWarning_Title,
				Resource.String.OrganisePub_DeletePubWarning_Message,
				Resource.String.Delete,
				0,
				OrganisePublicationsFragment.DeletePubWarningDialog,
				pub.Value.BookId.ToString());
		}

		private void OnSortPub(int fromPosition, int toPosition, List<int> ids)
		{
			PublicationUtil.Instance.OrganiseDlsOrder(ids);
		}

		public bool OnSimpleDialogButtonClick(DialogButtonType buttonType, string fragmentTag, string extTagKey, string extTag)
		{
			if(extTagKey == DeletePubWarningDialog)
			{
				if(buttonType == DialogButtonType.Positive)
				{
					var bookId = Int32.Parse(extTag);
					DataCache.INSTATNCE.PublicationManager.RemovePublication(bookId);
					organisePublicationsListAdaptor.DeletePub(bookId);

					// call api
					PublicationUtil.Instance.DeletePublicationByUser(bookId);
				}

				return true;
			}

			return false;
		}
	}
}

