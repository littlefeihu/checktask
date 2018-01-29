using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;
using LexisNexis.Red.Common.Entity;

namespace LexisNexis.Red.Mac.Data
{
	public class PublicationsDataManager
	{
		#region properties
		public List<Publication> offlinePublicationList;
		public List<Publication> onlinePublicationList;
		public Publication CurrentPublication { get; set; }
		public PublicationView CurrentPublicationView { get; set;}

		public static PublicationsDataManager SharedInstance
		{
			get {
				return sharedInstance;
			}
		}

		private static readonly PublicationsDataManager sharedInstance;

		#endregion

		#region constructors
		static PublicationsDataManager ()
		{
			if (sharedInstance == null) {
				sharedInstance = new PublicationsDataManager (); 
			}
		}

		private PublicationsDataManager()
		{
		}

		//initialize and load db
		async void LoadDB ()
		{
			GetLocalDBPublicationsList ();
			await GetServerPublicationsList ();
			if (onlinePublicationList != null) {
				offlinePublicationList = onlinePublicationList;
			}
		}

		#endregion

		#region methods
		public async Task InitAndLoadDB ()
		{
			await GetServerPublicationsList ();
			if (onlinePublicationList != null) {
				offlinePublicationList = onlinePublicationList;
			}
		}

		public List<Publication> GetLocalDBPublicationsList ()
		{
			offlinePublicationList = PublicationUtil.Instance.GetPublicationOffline ();
			return offlinePublicationList;
		}

		//the list will have values if online, otherwise null
		public async Task<List<Publication>> GetServerPublicationsList ()
		{
			OnlinePublicationResult onlinePublicationResult  = await PublicationUtil.Instance.GetPublicationOnline ();
			if (onlinePublicationResult.RequestStatus == RequestStatusEnum.Success) {
				onlinePublicationList = onlinePublicationResult.Publications;
			} else {
				onlinePublicationList = null;
			}

			return onlinePublicationList;
		}
			
		public List<Publication> ReplacePublicationByBookID(Publication bookInfo)
		{				
			int index = offlinePublicationList.FindIndex (n=>(n.BookId==bookInfo.BookId));

			offlinePublicationList.RemoveAt (index);
			offlinePublicationList.Insert (index, bookInfo);

			return offlinePublicationList;
		}

		public List<Publication> DeletePublicationByBookID(int bookID)
		{
			int index = offlinePublicationList.FindIndex (n=>(n.BookId==bookID));
			offlinePublicationList.RemoveAt (index);

			return offlinePublicationList;
		}

		public Publication GetCurrentPublicationByBookID(int bookID)
		{
			return offlinePublicationList.Find (n=>(n.BookId==bookID));
		}
			
		#endregion
	}
}

