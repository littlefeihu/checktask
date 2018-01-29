
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using DialogFragment = Android.Support.V4.App.DialogFragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using Newtonsoft.Json;
using LexisNexis.Red.Droid.Utility;
using Android.Support.V4.App;
using LexisNexis.Red.Droid.App;

namespace LexisNexis.Red.Droid.AlertDialogUtility
{
    public class SimpleDialogFragment : DialogFragment
    {
        private static readonly long ProcessSessionId = DateTimeHelper.GetTimeStamp();

        private const string SavedProvider = "SavedProvider";

        private SimpleDialogProvider cachedProvider;

        public string GetFragmentTag()
        {
            if (cachedProvider == null)
            {
                throw new InvalidProgramException("Unable to get cachedProvider for fragment tag.");
            }

            return cachedProvider.FragmentTag;
        }

        private SimpleDialogProvider GetProvider()
        {
            if (cachedProvider == null)
            {
                throw new InvalidProgramException("Unable to get cachedProvider.");
            }

            return cachedProvider;
        }

        /// <summary>
        /// Create the specified dialogProvider and releaseUI.
        /// </summary>
        /// <param name="dialogProvider">Dialog provider.</param>
        /// <param name="releaseUI">If set to <c>true</c> release UI, when the dialog resumed.</param>
        public static SimpleDialogFragment Create(SimpleDialogProvider dialogProvider/*, bool releaseUI = false*/)
        {
            dialogProvider.FragmentTag = Guid.NewGuid().ToString();
            dialogProvider.ProcessSessionId = ProcessSessionId;

            return new SimpleDialogFragment
            {
                cachedProvider = dialogProvider,
            };
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var provider = cachedProvider;

            if (provider == null)
            {
                if (savedInstanceState != null)
                {
                    var savedProviderString = savedInstanceState.GetString(SavedProvider);
                    if (!string.IsNullOrEmpty(savedProviderString))
                    {
                        cachedProvider = JsonConvert.DeserializeObject<SimpleDialogProvider>(savedProviderString);
                        provider = cachedProvider;
                    }
                }
            }

            if (provider == null)
            {
                throw new InvalidProgramException("Unable to get dialog provider.");
            }

            Dialog dialog = null;
            switch (provider.Type)
            {
                case SimpleDialogProvider.DialogType.AlertDialog:
                    var dialogBuilder = new AlertDialog.Builder(Activity);
                    if (provider.TitleResId > 0)
                    {
                        dialogBuilder.SetTitle(MainApp.ThisApp.Resources.GetString(provider.TitleResId));
                    }

                    dialogBuilder.SetMessage(MainApp.ThisApp.Resources.GetString(provider.MessageResId));
                    dialogBuilder.SetCancelable(provider.Cancelable);
                    Cancelable = provider.Cancelable;
                    dialogBuilder.SetPositiveButton(
                        provider.PositiveButtonResId > 0
                            ? MainApp.ThisApp.Resources.GetString(provider.PositiveButtonResId)
                            : Activity.Resources.GetString(Resource.String.OK),
                        new EventHandler<DialogClickEventArgs>(OnButtonClick));
                    if (provider.NegativeButtonResId >= 0)
                    {
                        dialogBuilder.SetNegativeButton(
                            provider.NegativeButtonResId == 0
                                ? Activity.Resources.GetString(Resource.String.Cancel)
                                : Activity.Resources.GetString(provider.NegativeButtonResId),
                            new EventHandler<DialogClickEventArgs>(OnButtonClick));
                    }

                    dialog = dialogBuilder.Create();
                    if (provider.TitleResId > 0)
                    {
                        dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
                    }

                    dialog.SetCanceledOnTouchOutside(provider.CanceledOnTouchOutside);
                    break;
                case SimpleDialogProvider.DialogType.PleaseWaitDialog:
                    var progressDialog = new ProgressDialog(Activity);
                    progressDialog.Indeterminate = true;
                    progressDialog.SetProgressStyle(ProgressDialogStyle.Spinner);
                    progressDialog.SetMessage(
                        provider.MessageResId > 0
                            ? Activity.Resources.GetString(provider.MessageResId)
                            : Activity.Resources.GetString(Resource.String.PleaseWaitMessage));
                    progressDialog.SetCancelable(false);
                    progressDialog.SetCanceledOnTouchOutside(false);
                    dialog = progressDialog;
                    Cancelable = false;
                    break;
                default:
                    throw new InvalidProgramException("Unknown dialog type.");
            }

            return dialog;
        }

        private void OnButtonClick(object sender, DialogClickEventArgs eventArgs)
        {
            var listener = Activity as ISimpleDialogListener;
            if (listener != null
                && listener.OnSimpleDialogButtonClick(
                    (DialogButtonType)eventArgs.Which,
                    GetFragmentTag(),
                    GetProvider().ExtTagKey,
                    GetProvider().ExtTag))
            {
                return;
            }

            foreach (var f in Activity.SupportFragmentManager.Fragments)
            {
                listener = f as ISimpleDialogListener;
                if (listener != null
                    && listener.OnSimpleDialogButtonClick(
                        (DialogButtonType)eventArgs.Which,
                        GetFragmentTag(),
                        GetProvider().ExtTagKey,
                        GetProvider().ExtTag))
                {
                    return;
                }
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            /*
			if(Arguments.GetBoolean(ReleaseUI, false))
			{
				PreventMultipleUIOperation.ReleaseUI();
				LogHelper.Debug("dbg", "### UI Released[" + DateTimeHelper.GetTimeStamp() + "]");
			}
			*/

            var provider = GetProvider();
            if (provider.DismissAfterProcessRestore
               && provider.ProcessSessionId != ProcessSessionId)
            {
                Dismiss();
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString(SavedProvider, JsonConvert.SerializeObject(cachedProvider));
            base.OnSaveInstanceState(outState);
        }

        public override void OnDestroyView()
        {
            // walkaround a bug of android os
            // http://stackoverflow.com/questions/14657490/how-to-properly-retain-a-dialogfragment-through-rotation
            // https://code.google.com/p/android/issues/detail?id=17423
            if (Dialog != null && RetainInstance)
                Dialog.SetOnDismissListener(null);
            base.OnDestroyView();
        }

        public string Show(FragmentManager manager)
        {
            var provider = GetProvider();
            Show(manager, provider.FragmentTag);
            return provider.FragmentTag;
        }

        public static void DismissDialog(FragmentActivity activity, string fragmentTag)
        {
            var fragment = activity.SupportFragmentManager.FindFragmentByTag(fragmentTag);
            if (fragment == null)
            {
                return;
            }

            ((DialogFragment)fragment).Dismiss();
        }

        public static string FindFragmentTagByExtraTag(FragmentActivity activity, Func<string, string, bool> match)
        {
            foreach (var f in activity.SupportFragmentManager.Fragments)
            {
                var simpleDialog = f as SimpleDialogFragment;
                if (simpleDialog == null)
                {
                    continue;
                }

                var provider = simpleDialog.GetProvider();
                if (match(provider.ExtTagKey, provider.ExtTag))
                {
                    return provider.FragmentTag;
                }
            }

            return null;
        }
    }
}

