
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Fragment=Android.Support.V4.App.Fragment;
using LexisNexis.Red.Droid.App;
using LexisNexis.Red.Droid.Widget.StatusBar;
using Android.Support.V7.Widget;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Touchguard;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Draggable;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Swipeable;
using Com.H6ah4i.Android.Widget.Advrecyclerview.Animator;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Droid.AnnotationOrganiserPage
{
	[Activity(Label = "EditTagsActivity")]			
	public class EditTagsActivity : AppCompatActivity
	{
		private Toolbar toolbar;
		private LinearLayout llRootView;

		private RecyclerView rcAnnotationList;

		private static readonly List<AnnotationTag> tagList;
		static EditTagsActivity()
		{
			tagList = new List<AnnotationTag>();
			tagList.Add(new AnnotationTag{
				Color = "#ff0000", Title = "Tag 1", TagId = Guid.NewGuid()});
			tagList.Add(new AnnotationTag{
				Color = "#00ff00", Title = "Tag 2", TagId = Guid.NewGuid()});
			tagList.Add(new AnnotationTag{
				Color = "#0000ff", Title = "Tag 3", TagId = Guid.NewGuid()});
			tagList.Add(new AnnotationTag{
				Color = "#808000", Title = "Tag 4", TagId = Guid.NewGuid()});
			tagList.Add(new AnnotationTag{
				Color = "#008080", Title = "Tag 5", TagId = Guid.NewGuid()});
			tagList.Add(new AnnotationTag{
				Color = "#800080", Title = "Tag 6", TagId = Guid.NewGuid()});
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			StatusBarTintHelper.SetStatusBarColor(this);

			// Create your application here
			SetContentView(Resource.Layout.annotations_edittags_activity);

			llRootView = FindViewById<LinearLayout>(Resource.Id.llRootView);

			FindViewById<View>(Resource.Id.ivAddTag).Click += delegate
			{
				var intent=new Intent(this, typeof(NewTagActivity));
				StartActivity(intent);
			};

			FindViewById<LinearLayout>(Resource.Id.llStatusBarStub).LayoutParameters =
				new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, StatusBarTintHelper.GetStatusBarHeight());

			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar_actionbar);
			SetSupportActionBar(toolbar);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);
			SupportActionBar.SetDisplayShowHomeEnabled(true);
			SupportActionBar.SetHomeButtonEnabled(true);

			Title = MainApp.ThisApp.Resources.GetString(Resource.String.AnnotationEditTagsPage_EditTags);

			//------------------------
			rcAnnotationList = FindViewById<RecyclerView>(Resource.Id.rcAnnotationList);
			var llm = new LinearLayoutManager(this);
			llm.Orientation = LinearLayoutManager.Vertical;
			llm.ScrollToPosition(0);
			rcAnnotationList.SetLayoutManager(llm);

			var tagm = new RecyclerViewTouchActionGuardManager();
			tagm.SetInterceptVerticalScrollingWhileAnimationRunning(true);
			tagm.Enabled = true;

			var ddm = new RecyclerViewDragDropManager();
			ddm.SetInitiateOnLongPress(true);
			ddm.SetInitiateOnMove(false);

			var sm = new RecyclerViewSwipeManager();
			sm.UserHandleSwipeUi = true;

			var sa = new EditTagsListAdaptor(this);
			sa.SetTagList(tagList);

			RecyclerView.Adapter wrappedAdapter = ddm.CreateWrappedAdapter(sa);
			wrappedAdapter = sm.CreateWrappedAdapter(wrappedAdapter);

			GeneralItemAnimator ia = new SwipeDismissItemAnimator();

			rcAnnotationList.SetAdapter(wrappedAdapter);
			rcAnnotationList.SetItemAnimator(ia);

			tagm.AttachRecyclerView(rcAnnotationList);
			sm.AttachRecyclerView(rcAnnotationList);
			ddm.AttachRecyclerView(rcAnnotationList);
		}


		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
			case Android.Resource.Id.Home:
				{
					Finish();
					return true;
				}
			default:
				return base.OnOptionsItemSelected(item);
			}
		}
	}
}

