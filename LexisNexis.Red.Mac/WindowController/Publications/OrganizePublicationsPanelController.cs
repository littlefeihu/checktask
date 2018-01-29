
using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using AppKit;

using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Mac.Data;
using System.Threading.Tasks;

namespace LexisNexis.Red.Mac
{
	public partial class OrganizePublicationsPanelController : NSWindowController
	{
		#region Properties
		public List<Publication> publicationArray { get; set;} 
		nint selectedIndex { get; set;}

		//strongly typed window accessor
		public new OrganizePublicationsPanel Window {
			get {
				return (OrganizePublicationsPanel)base.Window;
			}
		}

		public bool IsDeletePublication { get; set;}
		public bool IsDraggedPublicaiton { get; set;}
		public string BookTitle { get; set;}
		public int BookIDDeleted{ get; set;}
		public List<int> BookIDArray { get; set;}
		#endregion

		#region Constructors

		// Called when created from unmanaged code
		public OrganizePublicationsPanelController (IntPtr handle) : base (handle)
		{
			Initialize ();
		}
		
		// Called when created directly from a XIB file
		[Export ("initWithCoder:")]
		public OrganizePublicationsPanelController (NSCoder coder) : base (coder)
		{
			Initialize ();
		}
		
		// Call to load from the XIB/NIB file
		public OrganizePublicationsPanelController (string bookTitle) : base ("OrganizePublicationsPanel")
		{
			BookTitle = bookTitle;
			Initialize ();
		}
		
		// Shared initialization code
		void Initialize ()
		{
			Window.Title = "Organise Publication";
			Window.BackgroundColor = NSColor.White;
			Window.MakeFirstResponder (null);
			deleteButton.Enabled = false;
		}

		#endregion


		#region IBAction Methods

		async partial void DeleteButtonClick (NSObject sender)
		{
			nint index = tableView.SelectedRow;
			if (index <0) {
				return;
			}

			int selIndex = Convert.ToInt32(index);
			string bookTitle = publicationArray[selIndex].Name;
			if (bookTitle == BookTitle) {
				AlertSheet.RunPromptModal("Error", "This title is open, please select another one.");
				return;
			}

			string title = String.Format("Are you sure you want to delete \"{0}\"?",bookTitle);
			string errMsg = "You will have to call our Customer Support if you want to re-install it.";

			nint result = AlertSheet.RunDeleteAlert(title,errMsg);
			if (result == 1) {
				
				IsDeletePublication = true;

				Publication book = publicationArray[selIndex];
				BookIDDeleted = book.BookId;

				await PublicationUtil.Instance.DeletePublicationByUser(book.BookId);
				Window.Close();
				Window.OrderOut(null);

				//await DeletePublicationByBookId(book.BookId);
			}
		}

		async Task DeletePublicationByBookId (int bookId)
		{
			await PublicationUtil.Instance.DeletePublicationByUser(bookId);
		}

		partial void CancelButtonClick (NSObject sender)
		{
			var NSApp = NSApplication.SharedApplication;
			NSApp.EndSheet(Window);
			Window.OrderOut(null);
		}

		async partial void OKButtonClick (NSObject sender)
		{
			if (IsDraggedPublicaiton) {
				BookIDArray = new List<int>(publicationArray.Count);

				foreach (var publication in publicationArray) {
					BookIDArray.Add(publication.BookId);
				}

				await PublicationUtil.Instance.OrganiseDlsOrder(BookIDArray);
			}

			var NSApp = NSApplication.SharedApplication;
			Window.Close();
			Window.OrderOut(null);
			NSApp.EndSheet(Window);
		}


		#endregion

		#region Methods
		public override void AwakeFromNib ()
		{
			publicationArray = PublicationUtil.Instance.GetPublicationOffline ();
			tableView.DataSource = new PubsTableViewDataSource (this);
			tableView.ReloadData ();

			tableView.SelectionShouldChange += (t) => true;
			tableView.SelectionDidChange += ( sender,  e) => 
				HandleSelectionDidChange ((NSNotification)sender);

			string[] typeArray = {"NSStringPboardType"};
			tableView.RegisterForDraggedTypes (typeArray);
		}


		void HandleSelectionDidChange(NSNotification sender)
		{
			var aTableView = (NSTableView)sender.Object;
			selectedIndex = aTableView.SelectedRow;
			deleteButton.Enabled = true;
		}

		public void DragItemFromIndexToIndex (NSTableView tableView, int dragRow, int toRow)
		{
			IsDraggedPublicaiton = true;

			//Console.WriteLine("from:{0} to:{1}", dragRow, toRow);
			Publication dragItem = publicationArray [dragRow];

			if (dragRow<toRow) {
				publicationArray.Insert(toRow,dragItem);
				publicationArray.RemoveAt(dragRow);
			} else {
				publicationArray.RemoveAt(dragRow);
				publicationArray.Insert(toRow,dragItem);
			}


		}

		#endregion
	}
}

