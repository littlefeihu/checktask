using System;
using System.Collections.Generic;

using AppKit;
using Foundation;

namespace LexisNexis.Red.Mac
{
	class CountryDataSource : NSComboBoxDataSource
	{
		//private List<Country> regions;

		public CountryDataSource ()
		{
		}

		public override string CompletedString (NSComboBox comboBox, string uncompletedString)
		{
			return countries.Find (n => n.StartsWith (uncompletedString, StringComparison.InvariantCultureIgnoreCase));
		}

		public override nint IndexOfItem (NSComboBox comboBox, string value)
		{
			return countries.FindIndex (n => n.Equals (value, StringComparison.InvariantCultureIgnoreCase));
		}

		public override nint ItemCount (NSComboBox comboBox)
		{
			return countries.Count;
		}

		public override NSObject ObjectValueForItem (NSComboBox comboBox, nint index)
		{
			return NSObject.FromObject (countries [Convert.ToInt32(index)]);
		}

		List<string> countries = new List<string> 
		{
			"Australia",
			"Hong Kong",
			"Malaysia",
			"New Zealand",
			"Singapore"
		};
	}
}

