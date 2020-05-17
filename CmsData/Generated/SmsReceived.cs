using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using CmsData.Infrastructure;

namespace CmsData
{
	[Table(Name="dbo.SmsReceived")]
	public partial class SmsReceived : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);

	#region Private Fields

		private int _Id;

		private DateTime? _DateReceived;

		private string _FromNumber;

		private int? _FromPeopleId;

		private string _ToNumber;

		private int? _ToGroupId;

		private string _Body;

		private string _Action;

		private string _Args;

		private string _ActionResponse;

		private bool? _ErrorOccurred;

		private bool? _RepliedTo;

		private EntityRef<Person> _Person;

		private EntityRef<SMSGroup> _SMSGroup;

	#endregion

    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();

		partial void OnIdChanging(int value);
		partial void OnIdChanged();

		partial void OnDateReceivedChanging(DateTime? value);
		partial void OnDateReceivedChanged();

		partial void OnFromNumberChanging(string value);
		partial void OnFromNumberChanged();

		partial void OnFromPeopleIdChanging(int? value);
		partial void OnFromPeopleIdChanged();

		partial void OnToNumberChanging(string value);
		partial void OnToNumberChanged();

		partial void OnToGroupIdChanging(int? value);
		partial void OnToGroupIdChanged();

		partial void OnBodyChanging(string value);
		partial void OnBodyChanged();

		partial void OnActionChanging(string value);
		partial void OnActionChanged();

		partial void OnArgsChanging(string value);
		partial void OnArgsChanged();

		partial void OnActionResponseChanging(string value);
		partial void OnActionResponseChanged();

		partial void OnErrorOccurredChanging(bool? value);
		partial void OnErrorOccurredChanged();

    #endregion
		public SmsReceived()
		{
			this._Person = default(EntityRef<Person>);

			this._SMSGroup = default(EntityRef<SMSGroup>);

			OnCreated();
		}

    #region Columns

        [Column(Name="Id", UpdateCheck=UpdateCheck.Never, Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get { return this._Id; }

			set
			{
				if (this._Id != value)
				{
                    this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}

		[Column(Name="DateReceived", UpdateCheck=UpdateCheck.Never, Storage="_DateReceived", DbType="datetime")]
		public DateTime? DateReceived
		{
			get { return this._DateReceived; }

			set
			{
				if (this._DateReceived != value)
				{
                    this.OnDateReceivedChanging(value);
					this.SendPropertyChanging();
					this._DateReceived = value;
					this.SendPropertyChanged("DateReceived");
					this.OnDateReceivedChanged();
				}
			}
		}

		[Column(Name="FromNumber", UpdateCheck=UpdateCheck.Never, Storage="_FromNumber", DbType="varchar(15)")]
		public string FromNumber
		{
			get { return this._FromNumber; }

			set
			{
				if (this._FromNumber != value)
				{
                    this.OnFromNumberChanging(value);
					this.SendPropertyChanging();
					this._FromNumber = value;
					this.SendPropertyChanged("FromNumber");
					this.OnFromNumberChanged();
				}
			}
		}

		[Column(Name="FromPeopleId", UpdateCheck=UpdateCheck.Never, Storage="_FromPeopleId", DbType="int")]
		[IsForeignKey]
		public int? FromPeopleId
		{
			get { return this._FromPeopleId; }

			set
			{
				if (this._FromPeopleId != value)
				{
					if (this._Person.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();

                    this.OnFromPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._FromPeopleId = value;
					this.SendPropertyChanged("FromPeopleId");
					this.OnFromPeopleIdChanged();
				}
			}
		}

		[Column(Name="ToNumber", UpdateCheck=UpdateCheck.Never, Storage="_ToNumber", DbType="varchar(15)")]
		public string ToNumber
		{
			get { return this._ToNumber; }

			set
			{
				if (this._ToNumber != value)
				{
                    this.OnToNumberChanging(value);
					this.SendPropertyChanging();
					this._ToNumber = value;
					this.SendPropertyChanged("ToNumber");
					this.OnToNumberChanged();
				}
			}
		}

		[Column(Name="ToGroupId", UpdateCheck=UpdateCheck.Never, Storage="_ToGroupId", DbType="int")]
		[IsForeignKey]
		public int? ToGroupId
		{
			get { return this._ToGroupId; }

			set
			{
				if (this._ToGroupId != value)
				{
					if (this._SMSGroup.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();

                    this.OnToGroupIdChanging(value);
					this.SendPropertyChanging();
					this._ToGroupId = value;
					this.SendPropertyChanged("ToGroupId");
					this.OnToGroupIdChanged();
				}
			}
		}

		[Column(Name="Body", UpdateCheck=UpdateCheck.Never, Storage="_Body", DbType="varchar")]
		public string Body
		{
			get { return this._Body; }

			set
			{
				if (this._Body != value)
				{
                    this.OnBodyChanging(value);
					this.SendPropertyChanging();
					this._Body = value;
					this.SendPropertyChanged("Body");
					this.OnBodyChanged();
				}
			}
		}


		[Column(Name="Action", UpdateCheck=UpdateCheck.Never, Storage="_Action", DbType="varchar(25)")]
		public string Action
		{
			get { return this._Action; }

			set
			{
				if (this._Action != value)
				{
                    this.OnActionChanging(value);
					this.SendPropertyChanging();
					this._Action = value;
					this.SendPropertyChanged("Action");
					this.OnActionChanged();
				}
			}
		}


		[Column(Name="Args", UpdateCheck=UpdateCheck.Never, Storage="_Args", DbType="varchar(50)")]
		public string Args
		{
			get { return this._Args; }

			set
			{
				if (this._Args != value)
				{
                    this.OnArgsChanging(value);
					this.SendPropertyChanging();
					this._Args = value;
					this.SendPropertyChanged("Args");
					this.OnArgsChanged();
				}
			}
		}

		[Column(Name="ActionResponse", UpdateCheck=UpdateCheck.Never, Storage="_ActionResponse", DbType="varchar(200)")]
		public string ActionResponse
		{
			get { return this._ActionResponse; }

			set
			{
				if (this._ActionResponse != value)
				{
                    this.OnActionResponseChanging(value);
					this.SendPropertyChanging();
					this._ActionResponse = value;
					this.SendPropertyChanged("ActionResponse");
					this.OnActionResponseChanged();
				}
			}
		}


		[Column(Name="ErrorOccurred", UpdateCheck=UpdateCheck.Never, Storage="_ErrorOccurred", DbType="bit")]
		public bool? ErrorOccurred
		{
			get { return this._ErrorOccurred; }

			set
			{
				if (this._ErrorOccurred != value)
				{
                    this.OnErrorOccurredChanging(value);
					this.SendPropertyChanging();
					this._ErrorOccurred = value;
					this.SendPropertyChanged("ErrorOccurred");
					this.OnErrorOccurredChanged();
				}
			}
		}
		[Column(Name="RepliedTo", UpdateCheck=UpdateCheck.Never, Storage="_RepliedTo", DbType="bit")]
		public bool? RepliedTo
		{
			get { return this._RepliedTo; }

			set
			{
				if (this._RepliedTo != value)
				{
                    this.OnErrorOccurredChanging(value);
					this.SendPropertyChanging();
					this._ErrorOccurred = value;
					this.SendPropertyChanged("RepliedTo");
					this.OnErrorOccurredChanged();
				}
			}
		}


    #endregion

    #region Foreign Key Tables

	#endregion

	#region Foreign Keys

		[Association(Name="FK_SmsReceived_People", Storage="_Person", ThisKey="FromPeopleId", IsForeignKey=true)]
		public Person Person
		{
			get { return this._Person.Entity; }

			set
			{
				Person previousValue = this._Person.Entity;
				if (((previousValue != value)
							|| (this._Person.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._Person.Entity = null;
						previousValue.SmsReceiveds.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.SmsReceiveds.Add(this);
						this._FromPeopleId = value.PeopleId;
					}

					else
					{
						this._FromPeopleId = default(int?);
					}

					this.SendPropertyChanged("Person");
				}
			}
		}

		[Association(Name="FK_SmsReceived_SMSGroups", Storage="_SMSGroup", ThisKey="ToGroupId", IsForeignKey=true)]
		public SMSGroup SMSGroup
		{
			get { return this._SMSGroup.Entity; }

			set
			{
				SMSGroup previousValue = this._SMSGroup.Entity;
				if (((previousValue != value)
							|| (this._SMSGroup.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._SMSGroup.Entity = null;
						previousValue.SmsReceiveds.Remove(this);
					}

					this._SMSGroup.Entity = value;
					if (value != null)
					{
						value.SmsReceiveds.Add(this);
						this._ToGroupId = value.Id;
					}

					else
					{
						this._ToGroupId = default(int?);
					}
					this.SendPropertyChanged("SMSGroup");
				}
			}
		}

	#endregion

		public event PropertyChangingEventHandler PropertyChanging;
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
				this.PropertyChanging(this, emptyChangingEventArgs);
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

