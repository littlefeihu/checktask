using System.Collections.Generic;

using LexisNexis.Red.Common.Entity;

namespace LexisNexis.Red.iOS
{
	public class TOCNodeTree
	{
		/// <summary>
		/// Gets or sets the root TOC node.
		/// </summary>
		/// <value>The root TOC node.</value>
		public TOCNode RootNode{ get; set; }

		/// <summary>
		/// Gets or sets the total level.
		/// </summary>
		/// <value>The total level.</value>
		public int TotalLevel{ get; set; }

		public TOCNodeTree (TOCNode node)
		{
			RootNode = node;

			foreach (var n in node.ChildNodes){
				RecursiveCountTotalNodeLevel (n);
			}
		}

		/// <summary>
		/// Gets the display TOC node list.
		/// </summary>
		/// <returns>The display TOC node list.</returns>
		/// <param name="latestOpenedNode">Latest opened node.</param>
		public List<TOCNode> GetDisplayTOCNodeList(TOCNode latestOpenedNode = null)
		{
			List<TOCNode> displayTOCNodeList = new List<TOCNode> ();

			if (latestOpenedNode == null) {
				latestOpenedNode = GetFirstPageNode().ParentNode;
			}

			var node = latestOpenedNode;
			while (node != null && node.NodeLevel != 0) {
				displayTOCNodeList.Insert (0, node);
				node = node.ParentNode;
			}
			if (latestOpenedNode.Role != "me") {
				displayTOCNodeList.AddRange (latestOpenedNode.ChildNodes);
			}

			return displayTOCNodeList;
		}


		/// <summary>
		/// Recursive counts the total node level.
		/// </summary>
		/// <param name="node">Node.</param>
		private void RecursiveCountTotalNodeLevel (TOCNode node)
		{
			this.TotalLevel = node.NodeLevel > this.TotalLevel ? node.NodeLevel : this.TotalLevel;
			if (node.ChildNodes != null && node.ChildNodes.Count > 0) {
				foreach (var child in node.ChildNodes) {
					RecursiveCountTotalNodeLevel (child);
				}
			}
		}


		/// <summary>
		/// Gets the first page node whose role is 'me'
		/// </summary>
		/// <returns>The first page node.</returns>
		public TOCNode GetFirstPageNode()
		{
			var node = RootNode;
			while (node.Role != "me" ) {
				node = node.ChildNodes [0];
			}
			return node;
		}
	}
}

