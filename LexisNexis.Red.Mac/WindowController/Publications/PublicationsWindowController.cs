
using System;
using System.Linq;
using Foundation;
using AppKit;
using CoreGraphics;
using LexisNexis.Red.Mac.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using ObjCRuntime;

namespace LexisNexis.Red.Mac
{
	public partial class PublicationsWindowController : NSWindowController
	{
		#region Constructors

		// Called when created from unmanaged code
		public PublicationsWindowController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public PublicationsWindowController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public PublicationsWindowController () : base ("PublicationsWindow")
		{
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize()
		{
			// Additional validation for inheriting implementations
			Window.TitleVisibility = NSWindowTitleVisibility.Hidden;
			Window.MakeFirstResponder (null);
			Window.DidResize += HandleWindowDidResize;
		}

		#endregion

		#region properties
		//strongly typed window accessor
		public new PublicationsWindow Window {
			get {
				return (PublicationsWindow)base.Window;
			}
		}

		// Search menu template tags. Special items in the search menu are tagged so when the actual dynamic search menu is constructed, we know which items to show or hide.

		const int	NSSearchFieldRecentsTitleMenuItemTag  =   1000;
		// Title of recents menu group. Hidden if no recents. Also use for separators that my go away with recents

		const int	NSSearchFieldRecentsMenuItemTag       =   1001;
		// Recent items have this tag. Use to indicate location of recents in custom menu if no title specified

		const int	NSSearchFieldClearRecentsMenuItemTag  =   1002;
		// The 'Clear Recents' item has this tag. Hidden if no recents

		const int	NSSearchFieldNoRecentsMenuItemTag     =   1003;
		// The item describing a lack of recents uses this tag. Hidden if recents

		public PublicationsViewController PublicationsVC { get; set;}
		public AnnotationOrganiserViewController AnnotationsVC {get; set;}
		public PublicationContentViewController ContentVC { get; set;}
		enum ViewMode {
			VM_Publications = 1,
			VM_Annotations = 2,
			VM_Content = 3
		};
		ViewMode currentViewMode {get; set;}
		public int CurrentViewMode{get {
				if (currentViewMode == ViewMode.VM_Publications) {
					return 1;
				} else if (currentViewMode == ViewMode.VM_Annotations) {
					return 2;
				} else {
					return 3;
				}
			}}

		private bool isEditTag = false;
		#endregion

		public override void AwakeFromNib ()
		{
			//Console.WriteLine ("publications view awakefromnib");

			Window.BackgroundColor = NSColor.White;
			Window.Title = "Publications";

			//
			PublicationBtnClick (SegmentContol);
			currentViewMode = ViewMode.VM_Publications;

			//toolbar initialize
			EditAnnotationButton.Image = NSImage.ImageNamed ("NSShareTemplate");
			HistoryButton.Image = Utility.ImageWithFilePath ("/Images/Content/@1x/History-Icon.png");


			SplitSwithButton.Cell.ImageScale = NSImageScale.None;
			SplitSwithButton.Image = Utility.ImageWithFilePath ("/Images/Content/@1x/Sidebar-Icon.png");
			SplitSwithButton.ToolTip = "SideBar";

			InfoButton.Cell.ImageScale = NSImageScale.None;
			InfoButton.Image = Utility.ImageWithFilePath ("/Images/Content/@1x/Info-Icon.png");
			InfoButton.ToolTip = "Infomation";

			ShareButton.Cell.ImageScale = NSImageScale.None;
			//ShareButton.Image = Utility.ImageWithFilePath ("/Images/Content/@1x/Share-Icon.png");
			ShareButton.Image = NSImage.ImageNamed("NSShareTemplate");
			ShareButton.ToolTip = "Share";
			ShareButton.SendActionOn (NSEventType.LeftMouseUp);
			ShareButton.Action = new Selector ("ShareButtonClick:");
			ShareButton.Target = this;

			HistoryButton.Cell.ImageScale = NSImageScale.None;
			HistoryButton.Image = Utility.ImageWithFilePath ("/Images/Content/@1x/History-Icon.png");
			HistoryButton.ToolTip = "History";

			EditAnnotationButton.Cell.ImageScale = NSImageScale.None;
			EditAnnotationButton.Image = NSImage.ImageNamed("NSShareTemplate");
			EditAnnotationButton.ToolTip = "Edit Annotation";

			SetToolbarFuncBtnByViewMode ();
		}

		void ValidateSystemMenu()
		{
			var appDelegate = (AppDelegate)NSApplication.SharedApplication.Delegate;
			appDelegate.validateMenuItem (null);
		}

		#region public methods
		//titlebar history + href link to InternalHyperlink
		public async Task UpdateContentView(int bookID, int tocID)
		{
			PublicationsDataManager.SharedInstance.CurrentPublication = 
				PublicationsDataManager.SharedInstance.GetCurrentPublicationByBookID (bookID);
			
			UpdateTitleBar ();
			await ContentVC.RefreshContentViewAtBookIDandTocNode (tocID, false);
		}

		public void UpdateTitleBar ()
		{
			ClearSearchHistory ();

			SetToolbarTitle ();
			SetToolBarInfoUpdateState ();
		}

		//info modal or publication cover
		public async Task SwitchToContentView (int tocID)
		{
			if (currentViewMode == ViewMode.VM_Content) {
				return;
			}

			Window.Title = "Content";
			currentViewMode = ViewMode.VM_Content;
			SetToolbarFuncBtnByViewMode ();
			UpdateTitleBar ();
			SegmentContol.SetSelected (false, 0);

			if (ContentVC == null) {
				ContentVC = new PublicationContentViewController ();
				CGRect newframe = Window.ContentView.Frame;
				ContentVC.View.SetFrameSize (newframe.Size);
				await ContentVC.InitializeContentPage (tocID);
				MainView.AddSubview (ContentVC.View);

			} else {
				await ContentVC.InitializeContentPage (tocID);
				ContentVC.View.Hidden = false;
			}

			if ((PublicationsVC!=null) & (!PublicationsVC.View.Hidden)) {
				PublicationsVC.View.Hidden = true;
			}

			if ((AnnotationsVC!=null)&&(!AnnotationsVC.View.Hidden)) {
				AnnotationsVC.View.Hidden = true;
			}

			ValidateSystemMenu ();
		}

		public void AnnotationBtnClick (NSObject sender)
		{
			Window.Title = "Annotations";

			TitleTField.StringValue = "Annotations";
			TitleTField.ToolTip = "Annotations";

			if ((PublicationsVC!=null) & (!PublicationsVC.View.Hidden)) {
				PublicationsVC.View.Hidden = true;
			}

			if ((ContentVC!=null)&&(!ContentVC.View.Hidden)) {
				ContentVC.View.Hidden = true;
			}

			if (AnnotationsVC == null) {
				AnnotationsVC = new AnnotationOrganiserViewController ();

				CGRect newframe = Window.ContentView.Frame;
				AnnotationsVC.View.SetFrameOrigin (newframe.Location);
				AnnotationsVC.View.SetFrameSize (newframe.Size);

				MainView.AddSubview (AnnotationsVC.View);
			} else {
				AnnotationsVC.View.Hidden = false;
				AnnotationsVC.ReloadAnnotationData ();
			}
		}

		public void PublicationBtnClick (NSObject sender)
		{
			Window.Title = "Publications";

			TitleTField.StringValue = "Publications";
			TitleTField.ToolTip = "Publications";

			if ((AnnotationsVC!=null)&&(!AnnotationsVC.View.Hidden)) {
				AnnotationsVC.View.Hidden = true;
			}

			if ((ContentVC!=null)&&(!ContentVC.View.Hidden)) {
				ContentVC.View.Hidden = true;
				ContentVC.ClearMemory ();
			}

			if (PublicationsVC == null) {
				PublicationsVC = new PublicationsViewController ();
				CGRect newframe = Window.ContentView.Frame;
				PublicationsVC.View.SetFrameOrigin (newframe.Location);
				PublicationsVC.View.SetFrameSize (newframe.Size);
			
				MainView.AddSubview (PublicationsVC.View);
			} else {
				PublicationsVC.View.Hidden = false;
				if (currentViewMode == ViewMode.VM_Content) {
					PublicationsVC.RefreshHistoryView ();
				}
			}
		}
		#endregion

		#region action
		partial void SegmentClick (NSObject sender) 
		{
			var control = (NSSegmentedControl)sender;
			nint index = control.SelectedSegment;
			if (index == 0) {
				PublicationBtnClick(control);
				currentViewMode = ViewMode.VM_Publications;
			}else {
				AnnotationBtnClick(control);
				currentViewMode = ViewMode.VM_Annotations;
			}

			SetToolbarFuncBtnByViewMode ();
			ValidateSystemMenu ();
		}
			
		//content page
		partial void SplitButtonClick (NSObject sender)
		{
			ContentVC.SplitButtonClick(sender);
		}

		partial void InfoButtonClick (NSObject sender)
		{
			OpenInfoModal();
		}

		//annotaion + content
		partial void ShareButtonClick (NSObject sender)
		{
			switch(currentViewMode) {
			case ViewMode.VM_Annotations:
				EditAnnotaionBtnClick(sender);
				break;

			case ViewMode.VM_Content: {
					var button = (NSButton)sender;
					CGRect frame = button.Frame;

					nfloat x = frame.X;
					nfloat y = frame.Bottom;
					CGPoint location = new CGPoint(x,y);
					location = button.ConvertPointToView(location, Window.ContentView);
				
					NSWindow window = NSApplication.SharedApplication.KeyWindow;
					if (window == null) {
						window = NSApplication.SharedApplication.MainWindow;
					}
					nint windowNumber = 1;
					if (window != null) {
						windowNumber = window.WindowNumber;
					}
					NSEvent fakeMouseEvent = 
						NSEvent.MouseEvent (NSEventType.LeftMouseUp,
							location,
							(NSEventModifierMask)NSEventMask.LeftMouseUp,
							0,
							windowNumber,
							window.GraphicsContext, 0, 1, 1);
					NSMenu.PopUpContextMenu(ShareCustomMenu,fakeMouseEvent,button);

//					List<NSString> shareItems = new List<NSString>(0);
//					shareItems.Add(new NSString("Email document"));
//					var sharingServicePicker = new NSSharingServicePicker(shareItems.ToArray());
//					var shareView = (NSButton)sender;
//					sharingServicePicker.Delegate = new SharingServicePickerDelegate();
//					sharingServicePicker.ShowRelativeToRect(shareView.Bounds,shareView,NSRectEdge.MinYEdge);
				}
				break;
			}
		}

		async partial void EmailDocument (NSObject sender)
		{
			await ContentVC.PageController.SavehtmlToPDF();
		}

		partial void PrintDocument (NSObject sender)
		{
			ContentVC.PageController.PrintPDF(new NSUrl(""));
		}

		partial void HistoryBtnClick (NSObject sender)
		{
			switch(currentViewMode) {
			case ViewMode.VM_Annotations:
				break;

			case ViewMode.VM_Content:
				ContentVC.HistoryButtonClick(sender);
				break;
			}
		}

		partial void SearchFieldClick (NSObject sender)
		{
			switch(currentViewMode) {
			case ViewMode.VM_Annotations:
				break;

			case ViewMode.VM_Content:
				ContentVC.SearchFieldClick(sender);
				break;
			}
		}

		private void actionMenuItem()
		{
			Console.WriteLine ("actionMenuItem");
		}
			
		private void SearchMenu ()
		{
			if (SearchField.RespondsToSelector (new Selector ("setRecentSearches:"))) {
				NSMenu searchMenu = new NSMenu("Search Menu") {
					AutoEnablesItems = true
				};

				var recentsTitleItem = new NSMenuItem("Recent Searches","");
				// tag this menu item so NSSearchField can use it and respond to it appropriately
				recentsTitleItem.Tag = NSSearchFieldRecentsTitleMenuItemTag;
				searchMenu.InsertItem (recentsTitleItem,0);

				var norecentsTitleItem = new NSMenuItem("No recent searches","");
				// tag this menu item so NSSearchField can use it and respond to it appropriately
				norecentsTitleItem.Tag = NSSearchFieldNoRecentsMenuItemTag;
				searchMenu.InsertItem (norecentsTitleItem,1);

				var recentsItem = new NSMenuItem("Recents","");
				// tag this menu item so NSSearchField can use it and respond to it appropriately
				recentsItem.Tag = NSSearchFieldRecentsMenuItemTag;
				searchMenu.InsertItem (recentsItem,2);

				var separatorItem = NSMenuItem.SeparatorItem;
				// tag this menu item so NSSearchField can use it, by hiding/show it appropriately:
				separatorItem.Tag = NSSearchFieldRecentsTitleMenuItemTag;
				searchMenu.InsertItem (separatorItem,3);

				var clearItem = new NSMenuItem ("Clear", "");

				// tag this menu item so NSSearchField can use it
				clearItem.Tag = NSSearchFieldClearRecentsMenuItemTag;
				searchMenu.InsertItem (clearItem, 4);

				//				var item = new NSMenuItem("Custom","",(o,e) => actionMenuItem());
				//				searchMenu.InsertItem(item,0);

				//				var separator = NSMenuItem.SeparatorItem;
				//				searchMenu.InsertItem(separator,1);

				var searchCell = SearchField.Cell;
				searchCell.MaximumRecents = 20;
				searchCell.SearchMenuTemplate = searchMenu;
			}
		}

		void ClearSearchHistory()
		{
			SearchField.Cell.RecentSearches = new string[0];
		}

		//annotation
		partial void EditAnnotaionBtnClick (NSObject sender)
		{
			if (this.isEditTag) {
				return;
			}
			this.isEditTag = true;
			this.Invoke (() => {
				var popover = new NSPopover ();
				popover.Behavior = NSPopoverBehavior.ApplicationDefined;
				popover.ContentViewController = new EditTagsViewController(popover, this);
				popover.Show (new CGRect (0, 0, 0, 0), (NSView)sender, NSRectEdge.MinYEdge);
			}, 0.1f);
		}

		public void SetEditTagState(bool isEditing) 
		{
			this.isEditTag = isEditing;
			if (AnnotationsVC!=null) {
			    AnnotationsVC.ReloadAnnotationData ();
			}
		}

		#if false
		partial void EditAnnotaionBtnClick (NSObject sender)
		{
			var NSApp = NSApplication.SharedApplication;
			var panelController = new EditTagPanelController();
			var panel = panelController.Window;

			panel.WindowShouldClose +=(t)=>true;
			panel.WillClose += delegate(object obj, EventArgs e){
				NSApp.EndSheet(panel);
			};

			NSApp.BeginSheet(panel,NSApp.MainWindow);
		}
		#endif

		#endregion

		#region public methods
		public void SetToolbarFuncBtnByViewMode ()
		{
			switch (currentViewMode) {
			case ViewMode.VM_Publications:
				SplitSwithButton.Superview.Hidden = true;
				InfoButton.Superview.Hidden = true;
				ShareButton.Superview.Hidden = true;
				HistoryButton.Superview.Hidden = true;
				SearchField.Superview.Hidden = true;
				break;

			case ViewMode.VM_Annotations:
				SplitSwithButton.Superview.Hidden = true;
				InfoButton.Superview.Hidden = true;
				ShareButton.Superview.Hidden = false;
				ShareButton.Image = NSImage.ImageNamed ("NSBookmarksTemplate");
				HistoryButton.Superview.Hidden = false;
				SearchField.Superview.Hidden = false;
				SearchField.StringValue = "";
				SearchField.PlaceholderAttributedString = Utility.AttributedTitle ("Search Annotation", NSColor.Gray,
					"Helvetica Neue", 11.0f, NSTextAlignment.Left);
				SearchMenu ();
				break;

			case ViewMode.VM_Content:
				SplitSwithButton.Superview.Hidden = false;
				InfoButton.Superview.Hidden = false;
				ShareButton.Superview.Hidden = false;
				ShareButton.Image = NSImage.ImageNamed ("NSShareTemplate");
				HistoryButton.Superview.Hidden = false;
				SearchField.Superview.Hidden = false;
				SearchField.StringValue = "";
				SearchField.PlaceholderAttributedString = Utility.AttributedTitle ("Search Publication", NSColor.Gray,
					"Helvetica Neue",11.0f,NSTextAlignment.Left);
				SearchMenu ();
				break;
			}
		}

		public void OpenInfoModal ()
		{
			CGPoint orgPoint = Utility.GetModalPanelLocation(690.0f, LNRConstants.WindowHeight_MIN);  //658+22
			using (NSAutoreleasePool pool = new NSAutoreleasePool ()) {
				var currentView = PublicationsDataManager.SharedInstance.CurrentPublicationView;
				using (var infoPanelController = new PublicationInfoPanelController (orgPoint, currentView)) {
					int updateStatus = 0;
					if (currentView != null) {
						currentView.infoController = infoPanelController;
						updateStatus = currentView.UpdateStatus;
					}
					infoPanelController.BookInfo = PublicationsDataManager.SharedInstance.CurrentPublication;
					infoPanelController.InitializeInfoView (updateStatus);

					var infoPanel = infoPanelController.Window;
					infoPanel.MakeFirstResponder (null);

					NSApplication NSApp = NSApplication.SharedApplication;

					infoPanel.WindowShouldClose += (t) => true;
					infoPanel.WillClose += delegate(object asender, EventArgs e) {
						NSApp.StopModal ();
						updateStatus = infoPanelController.UpdateStatus;
					};

					NSApplication.SharedApplication.RunModalForWindow (infoPanel);
					infoPanelController.Window.OrderOut (null);
					infoPanelController.Dispose ();

					if (currentView != null) {
						currentView.UpdateStatus = updateStatus;
						currentView.RefreshUpdateStatus (updateStatus);
						currentView.infoController = null;
					}
				}
			}
		}

		public void SetToolBarInfoUpdateState ()
		{
			NSView superView = InfoButton.Superview;
			nint updateCount = PublicationsDataManager.SharedInstance.CurrentPublication.UpdateCount;

			NSView[] views = superView.Subviews;
			nint viewCount = views.Length;

			if (updateCount <= 0) {
				while (viewCount > 2) {
					NSButton button = (NSButton)views [viewCount - 1];
					button.RemoveFromSuperview ();
					viewCount--;
				}
			}else {
				if (viewCount <= 2) {
					CGRect frame = InfoButton.Frame;
					CGRect newFrame = new CGRect (frame.Right - 15, frame.Bottom - 15, 13, 13);
					NSButton newButton = new NSButton (newFrame);
					newButton.Cell.BezelStyle = NSBezelStyle.Circular;
					newButton.Cell.Bordered = false;
					newButton.WantsLayer = true;
					newButton.Layer.BackgroundColor = NSColor.Red.CGColor;
					newButton.Layer.CornerRadius = 6.5f;
					newButton.Alignment = NSTextAlignment.Center;
					//button.Enabled = false;

					string infoTitle = updateCount.ToString ();
					newButton.AttributedTitle = Utility.AttributeTitle (infoTitle, NSColor.White, 8);
					superView.AddSubview (newButton);
				}
			}
		}

		public void SetSidebarImageByState(bool isStateOn)
		{
			if (!isStateOn) {
				SplitSwithButton.Image = Utility.ImageWithFilePath ("/Images/Content/@1x/Sidebar-Icon-White.png");
			} else {
				SplitSwithButton.Image = Utility.ImageWithFilePath ("/Images/Content/@1x/Sidebar-Icon.png");
			}
		}

		public void SetToolbarTitle ()
		{
			var currentPublication = PublicationsDataManager.SharedInstance.CurrentPublication;
			if (currentPublication == null) {
				return;
			}

			bool isFTC = currentPublication.IsFTC;
			string bookTitle = PublicationsDataManager.SharedInstance.CurrentPublication.Name;

			if (isFTC) {
				int location = bookTitle.IndexOf ("+ Cases");
				if (location < 0) {
					location = bookTitle.IndexOf ("+Case");
					if (location < 0) {
						bookTitle += " + Cases";
					}
				}
			}

			if (bookTitle!=null) {
				TitleTField.StringValue = bookTitle;
				TitleTField.ToolTip = bookTitle;
			}
		}

		#endregion

		protected virtual void HandleWindowDidResize (object sender, EventArgs e)
		{
			switch(currentViewMode) {
			case ViewMode.VM_Annotations:
				break;

			case ViewMode.VM_Content:
				ContentVC.HandleWindowDidResize (sender, e);
				break;

			case ViewMode.VM_Publications:
				PublicationsVC.HandleWindowDidResize (sender, e);
				break;
			}
		}
	
	}
}

