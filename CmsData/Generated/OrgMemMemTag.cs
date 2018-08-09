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
	[Table(Name="dbo.OrgMemMemTags")]
	public partial class OrgMemMemTag : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _OrgId;
		
		private int _PeopleId;
		
		private int _MemberTagId;
		
		private int? _Number;
		
		private bool? _IsLeader;
		
   		
    	
		private EntityRef<MemberTag> _MemberTag;
		
		private EntityRef<OrganizationMember> _OrganizationMember;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnOrgIdChanging(int value);
		partial void OnOrgIdChanged();
		
		partial void OnPeopleIdChanging(int value);
		partial void OnPeopleIdChanged();
		
		partial void OnMemberTagIdChanging(int value);
		partial void OnMemberTagIdChanged();
		
		partial void OnNumberChanging(int? value);
		partial void OnNumberChanged();
		
		partial void OnIsLeaderChanging(bool? value);
		partial void OnIsLeaderChanged();
		
    #endregion
		public OrgMemMemTag()
		{
			
			
			this._MemberTag = default(EntityRef<MemberTag>); 
			
			this._OrganizationMember = default(EntityRef<OrganizationMember>); 
			
			OnCreated();
		}

		
    #region Columns
		
		[Column(Name="OrgId", UpdateCheck=UpdateCheck.Never, Storage="_OrgId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int OrgId
		{
			get { return this._OrgId; }

			set
			{
				if (this._OrgId != value)
				{
				
					if (this._OrganizationMember.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnOrgIdChanging(value);
					this.SendPropertyChanging();
					this._OrgId = value;
					this.SendPropertyChanged("OrgId");
					this.OnOrgIdChanged();
				}

			}

		}

		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int PeopleId
		{
			get { return this._PeopleId; }

			set
			{
				if (this._PeopleId != value)
				{
				
					if (this._OrganizationMember.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._PeopleId = value;
					this.SendPropertyChanged("PeopleId");
					this.OnPeopleIdChanged();
				}

			}

		}

		
		[Column(Name="MemberTagId", UpdateCheck=UpdateCheck.Never, Storage="_MemberTagId", DbType="int NOT NULL", IsPrimaryKey=true)]
		[IsForeignKey]
		public int MemberTagId
		{
			get { return this._MemberTagId; }

			set
			{
				if (this._MemberTagId != value)
				{
				
					if (this._MemberTag.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnMemberTagIdChanging(value);
					this.SendPropertyChanging();
					this._MemberTagId = value;
					this.SendPropertyChanged("MemberTagId");
					this.OnMemberTagIdChanged();
				}

			}

		}

		
		[Column(Name="Number", UpdateCheck=UpdateCheck.Never, Storage="_Number", DbType="int")]
		public int? Number
		{
			get { return this._Number; }

			set
			{
				if (this._Number != value)
				{
				
                    this.OnNumberChanging(value);
					this.SendPropertyChanging();
					this._Number = value;
					this.SendPropertyChanged("Number");
					this.OnNumberChanged();
				}

			}

		}

		
		[Column(Name="IsLeader", UpdateCheck=UpdateCheck.Never, Storage="_IsLeader", DbType="bit")]
		public bool? IsLeader
		{
			get { return this._IsLeader; }

			set
			{
				if (this._IsLeader != value)
				{
				
                    this.OnIsLeaderChanging(value);
					this.SendPropertyChanging();
					this._IsLeader = value;
					this.SendPropertyChanged("IsLeader");
					this.OnIsLeaderChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_OrgMemMemTags_MemberTags", Storage="_MemberTag", ThisKey="MemberTagId", IsForeignKey=true)]
		public MemberTag MemberTag
		{
			get { return this._MemberTag.Entity; }

			set
			{
				MemberTag previousValue = this._MemberTag.Entity;
				if (((previousValue != value) 
							|| (this._MemberTag.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._MemberTag.Entity = null;
						previousValue.OrgMemMemTags.Remove(this);
					}

					this._MemberTag.Entity = value;
					if (value != null)
					{
						value.OrgMemMemTags.Add(this);
						
						this._MemberTagId = value.Id;
						
					}

					else
					{
						
						this._MemberTagId = default(int);
						
					}

					this.SendPropertyChanged("MemberTag");
				}

			}

		}

		
		[Association(Name="FK_OrgMemMemTags_OrganizationMembers", Storage="_OrganizationMember", ThisKey="OrgId,PeopleId", IsForeignKey=true)]
		public OrganizationMember OrganizationMember
		{
			get { return this._OrganizationMember.Entity; }

			set
			{
				OrganizationMember previousValue = this._OrganizationMember.Entity;
				if (((previousValue != value) 
							|| (this._OrganizationMember.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if (previousValue != null)
					{
						this._OrganizationMember.Entity = null;
						previousValue.OrgMemMemTags.Remove(this);
					}

					this._OrganizationMember.Entity = value;
					if (value != null)
					{
						value.OrgMemMemTags.Add(this);
						
						this._OrgId = value.OrganizationId;
						
						this._PeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._OrgId = default(int);
						
						this._PeopleId = default(int);
						
					}

					this.SendPropertyChanged("OrganizationMember");
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

