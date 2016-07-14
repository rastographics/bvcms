using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace CmsData.Classes.SmallGroupFinder
{
	[XmlRoot("SGF")]
	public class SmallGroupFinder
	{
		[XmlAttribute]
		public string divisionid { get; set; }

		[XmlAttribute]
		public string shell { get; set; }

		[XmlAttribute]
		public string layout { get; set; }

		[XmlAttribute]
		public string gutter { get; set; }

		[XmlArray]
		public List<SGFSetting> SGFSettings { get; set; }

		[XmlArray]
		public List<SGFFilter> SGFFilters { get; set; }
	}

	[Serializable]
	public class SGFSetting
	{
		[XmlAttribute]
		public string name { get; set; }

		[XmlAttribute]
		public string value { get; set; }

		public override string ToString()
		{
			return value;
		}
	}

	[Serializable]
	public class SGFFilter
	{
		[XmlAttribute]
		public string name { get; set; }

		[XmlAttribute]
		public string title { get; set; }

		[XmlAttribute]
		public bool locked { get; set; }

		[XmlAttribute]
		public string lockedvalue { get; set; }

		[XmlAttribute]
		public bool weekdays { get; set; }

		[XmlAttribute]
		public bool timeofdayonly { get; set; }

        [XmlAttribute]
        public string exclude { get; set; }
	}
}
