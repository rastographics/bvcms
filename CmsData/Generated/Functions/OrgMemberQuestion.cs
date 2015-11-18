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
	[Table(Name="OrgMemberQuestions")]
	public partial class OrgMemberQuestion
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private long? _Row;
		
		private string _Type;
		
		private int? _SetX;
		
		private string _Question;
		
		private string _Answer;
		
		
		public OrgMemberQuestion()
		{
		}

		
		
		[Column(Name="row", Storage="_Row", DbType="bigint")]
		public long? Row
		{
			get
			{
				return this._Row;
			}

			set
			{
				if (this._Row != value)
					this._Row = value;
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

		
		[Column(Name="Question", Storage="_Question", DbType="varchar(500)")]
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

		
		[Column(Name="Answer", Storage="_Answer", DbType="varchar")]
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
