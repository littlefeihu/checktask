using System;
using System.Collections.Generic;
using System.Linq;
using Foundation;
using AppKit;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.BusinessModel;

namespace LexisNexis.Red.Mac
{
	public partial class AnnotationOrganiserViewController : AppKit.NSViewController
	{
		public List<AnnotationTag> TagsList { get; set; }
		public List<Annotation> AnnotationList { get; set; }
		#region Constructors

		// Called when created from unmanaged code
		public AnnotationOrganiserViewController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}

		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public AnnotationOrganiserViewController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}

		// Call to load from the XIB/NIB file
		public AnnotationOrganiserViewController () : base ("AnnotationOrganiserView", NSBundle.MainBundle)
		{
			Initialize ();
		}

		// Shared initialization code
		void Initialize ()
		{
		}

		#endregion


		//strongly typed view accessor
		public new AnnotationOrganiserView View {
			get {
				return (AnnotationOrganiserView)base.View;
			}
		}

		enum AnnotationMode {
			AM_All = 1,
			AM_Notes = 2,
			AM_Highlights = 3,
			AM_Orphans = 4
		};

		AnnotationMode currentViewMode {get; set;}
		public int ViewMode{get {
				if (currentViewMode == AnnotationMode.AM_All) {
					return 1;
				} else {
					return 2;
				}
			}}
		
		#region methods
		public override void ViewDidLoad ()
		{
			ReloadAnnotationData ();

			if (AnnotationList == null) {
				AnnotationList = new List<Annotation> ();
			} else {
				AnnotationList.Clear ();
			}

			if (AnnotationUtil.Instance == null) {
				return;
			}

			AnnotationList.AddRange (AnnotationUtil.Instance.GetAnnotations());

			InfoLabelTF.StringValue = "No annotations match your search criteria.";
			PostInitialization ();
			//
			AllButton.Cell.Bordered = false;
			AllButton.Cell.SetButtonType (NSButtonType.MomentaryChange); 

			NotesButton.Cell.Bordered = false;
			NotesButton.Cell.SetButtonType (NSButtonType.MomentaryChange); 

			HighlightsButton.Cell.Bordered = false;
			HighlightsButton.Cell.SetButtonType (NSButtonType.MomentaryChange);

			OrphansButton.Cell.Bordered = false;
			OrphansButton.Cell.SetButtonType (NSButtonType.MomentaryChange);

			AllButtonClick (AllButton);
		}
	
		private void PostInitialization ()
		{
			if (TagsList == null || TagsList.Count == 0) {
			} else {
				TagsTableView.EnclosingScrollView.ScrollerKnobStyle = NSScrollerKnobStyle.Default;
				TagsTableView.EnclosingScrollView.VerticalScroller.ControlSize = NSControlSize.Small;
				TagsTableView.EnclosingScrollView.BackgroundColor = NSColor.White;

				TagsTableView.GridStyleMask = NSTableViewGridStyle.None;
				TagsTableView.EnclosingScrollView.BorderType = NSBorderType.NoBorder;

				TagsTableView.DataSource = new TagsTableDataSource (TagsList);
				TagsTableView.Delegate = new TagsTableDelegate (TagsList,this);
				TagsTableView.SelectionHighlightStyle = NSTableViewSelectionHighlightStyle.None;
			}

			if (AnnotationList == null || AnnotationList.Count == 0) {
				AnnotationTableView.EnclosingScrollView.Hidden = true;
				InfoLabelTF.Hidden = false;
			} else {
				AnnotationTableView.EnclosingScrollView.Hidden = false;
				InfoLabelTF.Hidden = true;
			}
		}
		#endregion

		public void ReloadAnnotationData ()
		{
			if (AnnCategoryTagUtil.Instance == null) {
				return;
			}
			if (TagsList == null) {
				TagsList = new List<AnnotationTag> ();
			} else {
				TagsList.Clear ();
			}

			var tag = new AnnotationTag ();
			tag.Color = string.Empty;
			tag.Title = "All Tags";
			tag.TagId = Guid.Empty;
			TagsList.Add (tag);

			var noTag = new AnnotationTag ();
			noTag.Color = string.Empty;
			noTag.Title = "No tag";
			noTag.TagId = Guid.Empty;
			TagsList.Add (noTag);

			var tags = AnnCategoryTagUtil.Instance.GetTags ();
			TagsList.AddRange (tags);

			if (TagsTableView != null) {
				TagsTableView.ReloadData ();
			}
		}

		#region action
		partial void AllButtonClick (NSObject sender)
		{
			if (currentViewMode == AnnotationMode.AM_All) {
				return;
			}

			currentViewMode = AnnotationMode.AM_All;
			AllButton.State = NSCellStateValue.On;
			NotesButton.State = NSCellStateValue.Off;
			HighlightsButton.State = NSCellStateValue.Off;
			OrphansButton.State = NSCellStateValue.Off;

			SetButtonAttributedTitle(AllButton,LNRConstants.TITLE_ALL,true);
			SetButtonAttributedTitle(NotesButton,LNRConstants.TITLE_NOTES,false);
			SetButtonAttributedTitle(HighlightsButton,LNRConstants.TITLE_HIGHLIGHTS,false);
			SetButtonAttributedTitle(OrphansButton,LNRConstants.TITLE_ORPHANS,false);
		}

		partial void NotesButtonClick (NSObject sender)
		{
			if (currentViewMode == AnnotationMode.AM_Notes) {
				return;
			}

			currentViewMode = AnnotationMode.AM_Notes;
			AllButton.State = NSCellStateValue.Off;
			NotesButton.State = NSCellStateValue.On;
			HighlightsButton.State = NSCellStateValue.Off;
			OrphansButton.State = NSCellStateValue.Off;

			SetButtonAttributedTitle(AllButton,LNRConstants.TITLE_ALL,false);
			SetButtonAttributedTitle(NotesButton,LNRConstants.TITLE_NOTES,true);
			SetButtonAttributedTitle(HighlightsButton,LNRConstants.TITLE_HIGHLIGHTS,false);
			SetButtonAttributedTitle(OrphansButton,LNRConstants.TITLE_ORPHANS,false);

		}

		partial void HighlightButtonClick (NSObject sender)
		{
			if (currentViewMode == AnnotationMode.AM_Highlights) {
				return;
			}

			currentViewMode = AnnotationMode.AM_Highlights;
			AllButton.State = NSCellStateValue.Off;
			NotesButton.State = NSCellStateValue.Off;
			HighlightsButton.State = NSCellStateValue.On;
			OrphansButton.State = NSCellStateValue.Off;

			SetButtonAttributedTitle(AllButton,LNRConstants.TITLE_ALL,false);
			SetButtonAttributedTitle(NotesButton,LNRConstants.TITLE_NOTES,false);
			SetButtonAttributedTitle(HighlightsButton,LNRConstants.TITLE_HIGHLIGHTS,true);
			SetButtonAttributedTitle(OrphansButton,LNRConstants.TITLE_ORPHANS,false);

		}

		partial void OrphansButtonClick (NSObject sender)
		{
			if (currentViewMode == AnnotationMode.AM_Orphans) {
				return;
			}

			currentViewMode = AnnotationMode.AM_Orphans;
			AllButton.State = NSCellStateValue.Off;
			NotesButton.State = NSCellStateValue.Off;
			HighlightsButton.State = NSCellStateValue.Off;
			OrphansButton.State = NSCellStateValue.On;

			SetButtonAttributedTitle(AllButton,LNRConstants.TITLE_ALL,false);
			SetButtonAttributedTitle(NotesButton,LNRConstants.TITLE_NOTES,false);
			SetButtonAttributedTitle(HighlightsButton,LNRConstants.TITLE_HIGHLIGHTS,false);
			SetButtonAttributedTitle(OrphansButton,LNRConstants.TITLE_ORPHANS,true);

		}

		void SetButtonAttributedTitle(NSButton button, string title, bool isStateOn)
		{
			float fontSize = 14;
			if (isStateOn) {
				var attributedTitle = Utility.AttributeTitle (title, NSColor.Red, fontSize);
				button.AttributedTitle = attributedTitle;
				var alterTitle = Utility.AttributeTitle (title, NSColor.Black, fontSize);
				button.AttributedAlternateTitle = alterTitle;
				button.WantsLayer = true;
				button.Layer.BackgroundColor = Utility.ColorWithRGB (251, 212, 213, 1.0f).CGColor;
				button.Layer.CornerRadius = 5;
			} else {
				var attributedTitle = Utility.AttributeTitle (title, NSColor.Black, fontSize);
				button.AttributedTitle = attributedTitle;
				var alterTitle = Utility.AttributeTitle (title, NSColor.Red, fontSize);
				button.AttributedAlternateTitle = alterTitle;
				button.WantsLayer = true;
				button.Layer.BackgroundColor = Utility.ColorWithRGB (255, 255, 255, 1.0f).CGColor;
				button.Layer.CornerRadius = 5;
			}
		}
		#endregion
	}
}
