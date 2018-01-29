using Android.OS;
using Android.Views;
using Android.Widget;
using Fragment=Android.Support.V4.App.Fragment;
using Android.Text.Method;
using Android.Text.Util;
using LexisNexis.Red.Droid.TextStyle;
using Android.Text;
using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.Droid.SettingsBoardPage
{
	public class ContactUsFragment: Fragment, ISettingsBoardFragment
	{

		public string Title
		{
			get
			{
				return SettingsBoardActivity.GetTitle(SettingsBoardActivity.Contact);
			}
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			RetainInstance = true;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.settings_contactus_fragment, container, false);

			var tvPhone = v.FindViewById<TextView>(Resource.Id.tvPhone);
			var tvTelSupportTime = v.FindViewById<TextView>(Resource.Id.tvTelSupportTime);
			var tvInternationalCall = v.FindViewById<TextView>(Resource.Id.tvInternationalCall);
			var tvFax = v.FindViewById<TextView>(Resource.Id.tvFax);
			var tvEmail = v.FindViewById<TextView>(Resource.Id.tvEmail);
			var tvLNCustomerRelations = v.FindViewById<TextView>(Resource.Id.tvLNCustomerRelations);
			var tvPost = v.FindViewById<TextView>(Resource.Id.tvPost);
			var tvPostToUsTitle = v.FindViewById<TextView>(Resource.Id.tvPostToUsTitle);
			var tvPostDx = v.FindViewById<TextView>(Resource.Id.tvPostDx);
			var tvSendByDXTitle = v.FindViewById<TextView>(Resource.Id.tvSendByDXTitle);

			var contactUs = GlobalAccess.Instance.CurrentUserInfo.Country.ContactUs;

			tvPhone.Text = contactUs.Phone;
			tvTelSupportTime.Text = contactUs.WorkingHours;
			tvInternationalCall.Text = contactUs.InternationalCallers;
			tvFax.Text = contactUs.Fax;
			tvEmail.Text = contactUs.Email;

			if(contactUs.PostToUs == null || string.IsNullOrEmpty(contactUs.PostToUs.Content))
			{
				tvPost.Visibility = ViewStates.Gone;
				tvPostToUsTitle.Visibility = ViewStates.Gone;
			}
			else
			{
				if(string.IsNullOrEmpty(contactUs.PostToUs.Title))
				{
					tvLNCustomerRelations.Visibility = ViewStates.Gone;
				}
				else
				{
					tvLNCustomerRelations.Text = contactUs.PostToUs.Title;
				}

				tvPost.Text = contactUs.PostToUs.Content;
			}

			if(string.IsNullOrEmpty(contactUs.SendByDX))
			{
				tvPostDx.Visibility = ViewStates.Gone;
				tvSendByDXTitle.Visibility = ViewStates.Gone;
			}
			else
			{
				tvPostDx.Text = contactUs.SendByDX;
			}

			var ssb = new SpannableStringBuilder(tvEmail.Text);
			tvEmail.MovementMethod = LinkMovementMethod.Instance;
			Linkify.AddLinks(ssb, MatchOptions.WebUrls | MatchOptions.EmailAddresses);
			FixedTextUtils.RemoveLinkUnderline(ssb);
			tvEmail.TextFormatted = ssb;

			return v;
		}
	}
}

