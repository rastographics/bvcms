using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CmsCheckin
{
	[XmlRoot("BuildingChecking")]
	public class BaseBuildingInfo
	{
		[XmlAttribute]
		public string querybit { get; set; }

		[XmlAttribute]
		public int maxguests { get; set; }

		[XmlAttribute]
		public int maxvisits { get; set; }

		[XmlArray]
		public List<Activity> Activities { get; set; }

		public BaseBuildingInfo()
		{
			querybit = "";
			maxguests = -1;
		}
	}

	[Serializable]
	public class Activity
	{
		[XmlAttribute]
		public string name { get; set; }

		[XmlAttribute]
		public int org { get; set; }

		[XmlText]
		public string display { get; set; }

		public override string ToString()
		{
			return display;
		}
	}
}
