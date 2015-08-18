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
	[Table(Name="RegsettingMessages")]
	public partial class RegsettingMessage
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _OrganizationId;
		
		private string _OrganizationName;
		
		private string _Subject;
		
		private string _Body;
		
		private string _SenderSubject;
		
		private string _SenderBody;
		
		private string _SupportSubject;
		
		private string _SupportBody;
		
		private string _ReminderSubject;
		
		private string _ReminderBody;
		
		private string _InstructionsLogin;
		
		private string _InstructionsOptions;
		
		private string _InstructionsSubmit;
		
		private string _InstructionsFind;
		
		private string _InstructionsSelect;
		
		private string _InstructionsSorry;
		
		private string _InstructionsSpecial;
		
		private string _InstructionsThanks;
		
		private string _InstructionsTerms;
		
		
		public RegsettingMessage()
		{
		}

		
		
		[Column(Name="OrganizationId", Storage="_OrganizationId", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsDbGenerated=true)]
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

		
		[Column(Name="OrganizationName", Storage="_OrganizationName", DbType="nvarchar(100) NOT NULL")]
		public string OrganizationName
		{
			get
			{
				return this._OrganizationName;
			}

			set
			{
				if (this._OrganizationName != value)
					this._OrganizationName = value;
			}

		}

		
		[Column(Name="Subject", Storage="_Subject", DbType="varchar(100)")]
		public string Subject
		{
			get
			{
				return this._Subject;
			}

			set
			{
				if (this._Subject != value)
					this._Subject = value;
			}

		}

		
		[Column(Name="Body", Storage="_Body", DbType="varchar")]
		public string Body
		{
			get
			{
				return this._Body;
			}

			set
			{
				if (this._Body != value)
					this._Body = value;
			}

		}

		
		[Column(Name="SenderSubject", Storage="_SenderSubject", DbType="varchar(100)")]
		public string SenderSubject
		{
			get
			{
				return this._SenderSubject;
			}

			set
			{
				if (this._SenderSubject != value)
					this._SenderSubject = value;
			}

		}

		
		[Column(Name="SenderBody", Storage="_SenderBody", DbType="varchar")]
		public string SenderBody
		{
			get
			{
				return this._SenderBody;
			}

			set
			{
				if (this._SenderBody != value)
					this._SenderBody = value;
			}

		}

		
		[Column(Name="SupportSubject", Storage="_SupportSubject", DbType="varchar(100)")]
		public string SupportSubject
		{
			get
			{
				return this._SupportSubject;
			}

			set
			{
				if (this._SupportSubject != value)
					this._SupportSubject = value;
			}

		}

		
		[Column(Name="SupportBody", Storage="_SupportBody", DbType="varchar")]
		public string SupportBody
		{
			get
			{
				return this._SupportBody;
			}

			set
			{
				if (this._SupportBody != value)
					this._SupportBody = value;
			}

		}

		
		[Column(Name="ReminderSubject", Storage="_ReminderSubject", DbType="varchar(100)")]
		public string ReminderSubject
		{
			get
			{
				return this._ReminderSubject;
			}

			set
			{
				if (this._ReminderSubject != value)
					this._ReminderSubject = value;
			}

		}

		
		[Column(Name="ReminderBody", Storage="_ReminderBody", DbType="varchar")]
		public string ReminderBody
		{
			get
			{
				return this._ReminderBody;
			}

			set
			{
				if (this._ReminderBody != value)
					this._ReminderBody = value;
			}

		}

		
		[Column(Name="InstructionsLogin", Storage="_InstructionsLogin", DbType="varchar")]
		public string InstructionsLogin
		{
			get
			{
				return this._InstructionsLogin;
			}

			set
			{
				if (this._InstructionsLogin != value)
					this._InstructionsLogin = value;
			}

		}

		
		[Column(Name="InstructionsOptions", Storage="_InstructionsOptions", DbType="varchar")]
		public string InstructionsOptions
		{
			get
			{
				return this._InstructionsOptions;
			}

			set
			{
				if (this._InstructionsOptions != value)
					this._InstructionsOptions = value;
			}

		}

		
		[Column(Name="InstructionsSubmit", Storage="_InstructionsSubmit", DbType="varchar")]
		public string InstructionsSubmit
		{
			get
			{
				return this._InstructionsSubmit;
			}

			set
			{
				if (this._InstructionsSubmit != value)
					this._InstructionsSubmit = value;
			}

		}

		
		[Column(Name="InstructionsFind", Storage="_InstructionsFind", DbType="varchar")]
		public string InstructionsFind
		{
			get
			{
				return this._InstructionsFind;
			}

			set
			{
				if (this._InstructionsFind != value)
					this._InstructionsFind = value;
			}

		}

		
		[Column(Name="InstructionsSelect", Storage="_InstructionsSelect", DbType="varchar")]
		public string InstructionsSelect
		{
			get
			{
				return this._InstructionsSelect;
			}

			set
			{
				if (this._InstructionsSelect != value)
					this._InstructionsSelect = value;
			}

		}

		
		[Column(Name="InstructionsSorry", Storage="_InstructionsSorry", DbType="varchar")]
		public string InstructionsSorry
		{
			get
			{
				return this._InstructionsSorry;
			}

			set
			{
				if (this._InstructionsSorry != value)
					this._InstructionsSorry = value;
			}

		}

		
		[Column(Name="InstructionsSpecial", Storage="_InstructionsSpecial", DbType="varchar")]
		public string InstructionsSpecial
		{
			get
			{
				return this._InstructionsSpecial;
			}

			set
			{
				if (this._InstructionsSpecial != value)
					this._InstructionsSpecial = value;
			}

		}

		
		[Column(Name="InstructionsThanks", Storage="_InstructionsThanks", DbType="varchar")]
		public string InstructionsThanks
		{
			get
			{
				return this._InstructionsThanks;
			}

			set
			{
				if (this._InstructionsThanks != value)
					this._InstructionsThanks = value;
			}

		}

		
		[Column(Name="InstructionsTerms", Storage="_InstructionsTerms", DbType="varchar")]
		public string InstructionsTerms
		{
			get
			{
				return this._InstructionsTerms;
			}

			set
			{
				if (this._InstructionsTerms != value)
					this._InstructionsTerms = value;
			}

		}

		
    }

}
