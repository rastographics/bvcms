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
	[Table(Name="SenderGifts")]
	public partial class SenderGift
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		
		private int _OrgId;
		
		private string _Trip;
		
		private int _SenderId;
		
		private string _Sender;
		
		private int? _GoerId;
		
		private string _Goer;
		
		private DateTime? _DateGiven;
		
		private decimal? _Amt;
		
		private string _NoticeSent;
		
		
		public SenderGift()
		{
		}

		
		
		[Column(Name="OrgId", Storage="_OrgId", DbType="int NOT NULL")]
		public int OrgId
		{
			get
			{
				return this._OrgId;
			}

			set
			{
				if (this._OrgId != value)
					this._OrgId = value;
			}

		}

		
		[Column(Name="Trip", Storage="_Trip", DbType="nvarchar(100) NOT NULL")]
		public string Trip
		{
			get
			{
				return this._Trip;
			}

			set
			{
				if (this._Trip != value)
					this._Trip = value;
			}

		}

		
		[Column(Name="SenderId", Storage="_SenderId", DbType="int NOT NULL")]
		public int SenderId
		{
			get
			{
				return this._SenderId;
			}

			set
			{
				if (this._SenderId != value)
					this._SenderId = value;
			}

		}

		
		[Column(Name="Sender", Storage="_Sender", DbType="nvarchar(139)")]
		public string Sender
		{
			get
			{
				return this._Sender;
			}

			set
			{
				if (this._Sender != value)
					this._Sender = value;
			}

		}

		
		[Column(Name="GoerId", Storage="_GoerId", DbType="int")]
		public int? GoerId
		{
			get
			{
				return this._GoerId;
			}

			set
			{
				if (this._GoerId != value)
					this._GoerId = value;
			}

		}

		
		[Column(Name="Goer", Storage="_Goer", DbType="nvarchar(139)")]
		public string Goer
		{
			get
			{
				return this._Goer;
			}

			set
			{
				if (this._Goer != value)
					this._Goer = value;
			}

		}

		
		[Column(Name="DateGiven", Storage="_DateGiven", DbType="datetime")]
		public DateTime? DateGiven
		{
			get
			{
				return this._DateGiven;
			}

			set
			{
				if (this._DateGiven != value)
					this._DateGiven = value;
			}

		}

		
		[Column(Name="Amt", Storage="_Amt", DbType="money")]
		public decimal? Amt
		{
			get
			{
				return this._Amt;
			}

			set
			{
				if (this._Amt != value)
					this._Amt = value;
			}

		}

		
		[Column(Name="NoticeSent", Storage="_NoticeSent", DbType="varchar(9) NOT NULL")]
		public string NoticeSent
		{
			get
			{
				return this._NoticeSent;
			}

			set
			{
				if (this._NoticeSent != value)
					this._NoticeSent = value;
			}

		}

		
    }

}
