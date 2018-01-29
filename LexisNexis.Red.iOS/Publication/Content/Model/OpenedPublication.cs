using System;

using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.Common.BusinessModel;
using LexisNexis.Red.Common.Business;

namespace LexisNexis.Red.iOS
{
	/// <summary>
	/// Opened publication.
	/// </summary>
	public class OpenedPublication : Subject
	{
		public OpenedPublication ()
		{
		}

		private Publication p;
		public Publication P{ 
			get {
				return p;
			}
			set{
				p = value;
				opendIndex = null;
				opendTOCNode = null;
			}

		}


		#region TOC operation
		/// <summary>
		/// Gets or sets the root TOC node.
		/// </summary>
		/// <value>The root TOC node.</value>
		public TOCNode RootTOCNode{ get; set; } 

		private TOCNode opendTOCNode;//must be a leaf toc node which role is 'me' other than ancestor
		public TOCNode OpendTOCNode{ 
			get
			{
				return opendTOCNode;
			}
			set
			{
				opendTOCNode = value;
				HighlightedTOCNode = value;
				OpendContentType = PublicationContentTypeEnum.TOC;
			}
		}

		public TOCNode HighlightedTOCNode { get; set; }

		#endregion

		#region Index operation
		private Index opendIndex;
		public Index OpendIndex{ 
			get {
				return opendIndex;
			}
			set{
				opendIndex = value;
				OpendContentType = PublicationContentTypeEnum.Index;
			}
		}
		#endregion

		private PublicationContentTypeEnum opendContentType = PublicationContentTypeEnum.None;
		public PublicationContentTypeEnum OpendContentType{ 
			get{
				return opendContentType;
			}
			set{
				opendContentType = value;
				Notify ();
			}
		}
	}
}