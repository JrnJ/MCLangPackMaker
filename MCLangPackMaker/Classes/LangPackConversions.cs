using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCLangPackMaker.Classes
{
    public class LangPackConversions
    {
        private MCVersion[] _versions;

        public MCVersion[] Versions
        {
            get { return _versions; }
            set { _versions = value; }
        }

        private List<string[]> _conversions;

		public List<string[]> Conversions
		{
			get { return _conversions; }
			set { _conversions = value; }
		}

		public LangPackConversions(MCVersion[] versions, List<string[]> conversions)
		{
			Versions = versions;
			Conversions = conversions;
        }
	}
}
