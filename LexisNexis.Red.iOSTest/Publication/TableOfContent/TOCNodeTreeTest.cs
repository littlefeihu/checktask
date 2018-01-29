using System;

using NUnit.Framework;

using LexisNexis.Red.Common.Entity;
using LexisNexis.Red.iOS;

namespace LexisNexis.Red.iOSTest
{
	[TestFixture]
	public class TOCNodeTreeTest
	{
		public TOCNodeTreeTest ()
		{
		}

		[Test]
		public void TestGetDisplayTOCNodeList1()
		{
			TOCNode root = new TOCNode ();
			TOCNodeTree tree = new TOCNodeTree (root);
		}
	}
}

