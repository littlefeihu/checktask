using LexisNexis.Red.Common.HelpClass;

namespace LexisNexis.Red.Droid.Business
{
	public class DataCache
	{
		private static readonly DataCache SINGLETON_INSTANCE = new DataCache();

		private string email;
		private string countryCode;

		private DataCache()
		{
		}

		public static DataCache INSTATNCE
		{
			get
			{
				if(GlobalAccess.Instance.CurrentUserInfo != null)
				{
					if(SINGLETON_INSTANCE.email != GlobalAccess.Instance.CurrentUserInfo.Email
						|| SINGLETON_INSTANCE.countryCode != GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode)
					{
						SINGLETON_INSTANCE.PublicationManager = new PublicationManager();
						SINGLETON_INSTANCE.ClosePublication();

						SINGLETON_INSTANCE.email = GlobalAccess.Instance.CurrentUserInfo.Email;
						SINGLETON_INSTANCE.countryCode = GlobalAccess.Instance.CurrentUserInfo.Country.CountryCode;
					}
				}

				return SINGLETON_INSTANCE;
			}
		}

		public string[] ServiceCountryList
		{
			get;
			set;
		}

		public PublicationManager PublicationManager
		{
			get;
			set;
		}

		#region Publication Specific
		public TOCBreadcrumbs Toc
		{
			get;
			set;
		}

		private PublicationIndexStatus indexList;
		public PublicationIndexStatus IndexList
		{
			get
			{
				if(indexList == null)
				{
					indexList = new PublicationIndexStatus();
				}

				return indexList;
			}

			set
			{
				indexList = value;
			}
		}

		public AnnotationsStatusKeeper AnnotationsStatus
		{
			get;
			set;
		}


		public SearchResultKeeper SearchResult
		{
			get;
			set;
		}

		public void ClosePublication()
		{
			Toc = null;
			IndexList.ResetPublicationIndex();
			SearchResult = null;
			AnnotationsStatus = null;
		}

		#endregion
	}
}

