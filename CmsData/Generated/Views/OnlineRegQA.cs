using System; 
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace CmsData.View
{
	[Table(Name="OnlineRegQA")]
	public partial class OnlineRegQA
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _OrganizationId;
		
		private int _PeopleId;
		
		private string _Type;
		
		private int? _SetX;
		
		private string _Question;
		
		private string _Answer;
		
		
		public OnlineRegQA()
		{
		}

		
		
		[Column(Name="OrganizationId", Storage="_OrganizationId", DbType="int NOT NULL")]
		public int OrganizationId
		{
			get
			{
				return this._OrganizationId;
			}

			set
			{
				if (this._OrganizationId != value)
					this._OrganizationId = value;
			}

		}

		
		[Column(Name="PeopleId", Storage="_PeopleId", DbType="int NOT NULL")]
		public int PeopleId
		{
			get
			{
				return this._PeopleId;
			}

			set
			{
				if (this._PeopleId != value)
					this._PeopleId = value;
			}

		}

		
		[Column(Name="type", Storage="_Type", DbType="varchar(8) NOT NULL")]
		public string Type
		{
			get
			{
				return this._Type;
			}

			set
			{
				if (this._Type != value)
					this._Type = value;
			}

		}

		
		[Column(Name="set", Storage="_SetX", DbType="int")]
		public int? SetX
		{
			get
			{
				return this._SetX;
			}

			set
			{
				if (this._SetX != value)
					this._SetX = value;
			}

		}

		
		[Column(Name="Question", Storage="_Question", DbType="nvarchar(500)")]
		public string Question
		{
			get
			{
				return this._Question;
			}

			set
			{
				if (this._Question != value)
					this._Question = value;
			}

		}

		
		[Column(Name="Answer", Storage="_Answer", DbType="nvarchar")]
		public string Answer
		{
			get
			{
				return this._Answer;
			}

			set
			{
				if (this._Answer != value)
					this._Answer = value;
			}

		}

		
    }

}
