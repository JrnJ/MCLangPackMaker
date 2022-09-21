using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCLangPackMaker.Classes
{
    public class LangPackValue : ObservableObject
    {
		private string _key;

		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}

		private string _value;

		public string Value
		{
			get { return _value; }
			set 
			{ 
				_value = value;
				MadeChanges = true;

				OnPropertyChanged(); 
			}
		}

		private bool _madeChanges;

		public bool MadeChanges
		{
			get { return _madeChanges; }
			set { _madeChanges = value; }
		}

		public LangPackValue(string key, string value)
		{
			Key = key;
			Value = value;

			// Set to false at LAST
			MadeChanges = false;
		}
	}
}
