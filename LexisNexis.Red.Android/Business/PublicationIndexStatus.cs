using System;
using System.Collections.Generic;
using LexisNexis.Red.Droid.Utility;
using LexisNexis.Red.Common.BusinessModel;
using Newtonsoft.Json;

namespace LexisNexis.Red.Droid.Business
{
	public class PublicationIndexStatus
	{
		public const string GetIndexListTask = "GetIndexListTask";

		[JsonIgnore]
		public ObjHolder<Publication> Publication
		{
			get;
			private set;
		}

		[JsonIgnore]
		public List<Index> IndexList
		{
			get;
			private set;
		}

		public int CurrentIndex
		{
			get;
			set;
		}

		public string UserSelectedIndexTitle
		{
			get;
			set;
		}

		public PublicationIndexStatus()
		{
			CurrentIndex = 0;
		}

		public void SetPublicationIndex(ObjHolder<Publication> publication, List<Index> indexList)
		{
			Publication = publication;
			IndexList = indexList;
			CurrentIndex = 0;
		}

		public void ResetPublicationIndex()
		{
			Publication = null;
			IndexList = null;
			CurrentIndex = 0;
			UserSelectedIndexTitle = null;
		}

		public bool IsCurrentPublication(int bookId)
		{
			return Publication != null && Publication.Value.BookId == bookId;
		}

		public Index GetCurrentIndex()
		{
			if(IndexList == null || IndexList.Count == 0)
			{
				return null;
			}

			if(CurrentIndex >= IndexList.Count)
			{
				CurrentIndex = 0;
			}

			return IndexList[CurrentIndex];
		}
	}
}

