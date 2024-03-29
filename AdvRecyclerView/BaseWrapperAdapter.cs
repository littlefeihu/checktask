﻿using System;
using System.Collections.Generic;
using Android.Runtime;

namespace Com.H6ah4i.Android.Widget.Advrecyclerview.Utils
{
	public partial class BaseWrapperAdapter
	{
		static Delegate cb_onBindViewHolder_Landroid_support_v7_widget_RecyclerView_ViewHolder_I;
		#pragma warning disable 0169
		static Delegate GetOnBindViewHolder_Landroid_support_v7_widget_RecyclerView_ViewHolder_IHandler ()
		{
			if (cb_onBindViewHolder_Landroid_support_v7_widget_RecyclerView_ViewHolder_I == null)
				cb_onBindViewHolder_Landroid_support_v7_widget_RecyclerView_ViewHolder_I = JNINativeWrapper.CreateDelegate ((Action<IntPtr, IntPtr, IntPtr, int>) n_OnBindViewHolder_Landroid_support_v7_widget_RecyclerView_ViewHolder_I);
			return cb_onBindViewHolder_Landroid_support_v7_widget_RecyclerView_ViewHolder_I;
		}

		static void n_OnBindViewHolder_Landroid_support_v7_widget_RecyclerView_ViewHolder_I (IntPtr jnienv, IntPtr native__this, IntPtr native_p0, int p1)
		{
			global::Com.H6ah4i.Android.Widget.Advrecyclerview.Utils.BaseWrapperAdapter __this = global::Java.Lang.Object.GetObject<global::Com.H6ah4i.Android.Widget.Advrecyclerview.Utils.BaseWrapperAdapter> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			global::Android.Support.V7.Widget.RecyclerView.ViewHolder p0 = global::Java.Lang.Object.GetObject<global::Android.Support.V7.Widget.RecyclerView.ViewHolder> (native_p0, JniHandleOwnership.DoNotTransfer);
			__this.OnBindViewHolder (p0, p1);
		}
		#pragma warning restore 0169

		static IntPtr id_onBindViewHolder_Landroid_support_v7_widget_RecyclerView_ViewHolder_I;
		// Metadata.xml XPath method reference: path="/api/package[@name='com.h6ah4i.android.widget.advrecyclerview.utils']/class[@name='BaseWrapperAdapter']/method[@name='onBindViewHolder' and count(parameter)=2 and parameter[1][@type='VH'] and parameter[2][@type='int']]"
		[Register ("onBindViewHolder", "(Landroid/support/v7/widget/RecyclerView$ViewHolder;I)V", "GetOnBindViewHolder_Landroid_support_v7_widget_RecyclerView_ViewHolder_IHandler")]
		public override unsafe void OnBindViewHolder (global::Android.Support.V7.Widget.RecyclerView.ViewHolder p0, int p1)
		{
			if (id_onBindViewHolder_Landroid_support_v7_widget_RecyclerView_ViewHolder_I == IntPtr.Zero)
				id_onBindViewHolder_Landroid_support_v7_widget_RecyclerView_ViewHolder_I = JNIEnv.GetMethodID (class_ref, "onBindViewHolder", "(Landroid/support/v7/widget/RecyclerView$ViewHolder;I)V");
			IntPtr native_p0 = JNIEnv.ToLocalJniHandle (p0);
			try {
				JValue* __args = stackalloc JValue [2];
				__args [0] = new JValue (native_p0);
				__args [1] = new JValue (p1);

				if (GetType () == ThresholdType)
					JNIEnv.CallVoidMethod  (Handle, id_onBindViewHolder_Landroid_support_v7_widget_RecyclerView_ViewHolder_I, __args);
				else
					JNIEnv.CallNonvirtualVoidMethod  (Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "onBindViewHolder", "(Landroid/support/v7/widget/RecyclerView$ViewHolder;I)V"), __args);
			} finally {
				JNIEnv.DeleteLocalRef (native_p0);
			}
		}


		static Delegate cb_onCreateViewHolder_Landroid_view_ViewGroup_I;
		#pragma warning disable 0169
		static Delegate GetOnCreateViewHolder_Landroid_view_ViewGroup_IHandler ()
		{
			if (cb_onCreateViewHolder_Landroid_view_ViewGroup_I == null)
				cb_onCreateViewHolder_Landroid_view_ViewGroup_I = JNINativeWrapper.CreateDelegate ((Func<IntPtr, IntPtr, IntPtr, int, IntPtr>) n_OnCreateViewHolder_Landroid_view_ViewGroup_I);
			return cb_onCreateViewHolder_Landroid_view_ViewGroup_I;
		}

		static IntPtr n_OnCreateViewHolder_Landroid_view_ViewGroup_I (IntPtr jnienv, IntPtr native__this, IntPtr native_p0, int p1)
		{
			global::Com.H6ah4i.Android.Widget.Advrecyclerview.Utils.BaseWrapperAdapter __this = global::Java.Lang.Object.GetObject<global::Com.H6ah4i.Android.Widget.Advrecyclerview.Utils.BaseWrapperAdapter> (jnienv, native__this, JniHandleOwnership.DoNotTransfer);
			global::Android.Views.ViewGroup p0 = global::Java.Lang.Object.GetObject<global::Android.Views.ViewGroup> (native_p0, JniHandleOwnership.DoNotTransfer);
			IntPtr __ret = JNIEnv.ToLocalJniHandle (__this.OnCreateViewHolder (p0, p1));
			return __ret;
		}
		#pragma warning restore 0169

		static IntPtr id_onCreateViewHolder_Landroid_view_ViewGroup_I;
		// Metadata.xml XPath method reference: path="/api/package[@name='com.h6ah4i.android.widget.advrecyclerview.utils']/class[@name='BaseWrapperAdapter']/method[@name='onCreateViewHolder' and count(parameter)=2 and parameter[1][@type='android.view.ViewGroup'] and parameter[2][@type='int']]"
		[Register ("onCreateViewHolder", "(Landroid/view/ViewGroup;I)Landroid/support/v7/widget/RecyclerView$ViewHolder;", "GetOnCreateViewHolder_Landroid_view_ViewGroup_IHandler")]
		public override unsafe global::Android.Support.V7.Widget.RecyclerView.ViewHolder OnCreateViewHolder (global::Android.Views.ViewGroup p0, int p1)
		{
			if (id_onCreateViewHolder_Landroid_view_ViewGroup_I == IntPtr.Zero)
				id_onCreateViewHolder_Landroid_view_ViewGroup_I = JNIEnv.GetMethodID (class_ref, "onCreateViewHolder", "(Landroid/view/ViewGroup;I)Landroid/support/v7/widget/RecyclerView$ViewHolder;");
			try {
				JValue* __args = stackalloc JValue [2];
				__args [0] = new JValue (p0);
				__args [1] = new JValue (p1);

				global::Android.Support.V7.Widget.RecyclerView.ViewHolder __ret;
				if (GetType () == ThresholdType)
					__ret = (global::Android.Support.V7.Widget.RecyclerView.ViewHolder) global::Java.Lang.Object.GetObject<global::Android.Support.V7.Widget.RecyclerView.ViewHolder> (JNIEnv.CallObjectMethod  (Handle, id_onCreateViewHolder_Landroid_view_ViewGroup_I, __args), JniHandleOwnership.TransferLocalRef);
				else
					__ret = (global::Android.Support.V7.Widget.RecyclerView.ViewHolder) global::Java.Lang.Object.GetObject<global::Android.Support.V7.Widget.RecyclerView.ViewHolder> (JNIEnv.CallNonvirtualObjectMethod  (Handle, ThresholdClass, JNIEnv.GetMethodID (ThresholdClass, "onCreateViewHolder", "(Landroid/view/ViewGroup;I)Landroid/support/v7/widget/RecyclerView$ViewHolder;"), __args), JniHandleOwnership.TransferLocalRef);
				return __ret;
			} finally {
			}
		}
	}
}

