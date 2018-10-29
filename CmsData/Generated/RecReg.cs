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
	[Table(Name="dbo.RecReg")]
	public partial class RecReg : INotifyPropertyChanging, INotifyPropertyChanged
	{
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
	#region Private Fields
		
		private int _Id;
		
		private int? _PeopleId;
		
		private int? _ImgId;
		
		private bool? _IsDocument;
		
		private bool? _ActiveInAnotherChurch;
		
		private string _ShirtSize;
		
		private bool? _MedAllergy;
		
		private string _Email;
		
		private string _MedicalDescription;
		
		private string _Fname;
		
		private string _Mname;
		
		private bool? _Coaching;
		
		private bool? _Member;
		
		private string _Emcontact;
		
		private string _Emphone;
		
		private string _Doctor;
		
		private string _Docphone;
		
		private string _Insurance;
		
		private string _Policy;
		
		private string _Comments;
		
		private bool? _Tylenol;
		
		private bool? _Advil;
		
		private bool? _Maalox;
		
		private bool? _Robitussin;
		
   		
    	
		private EntityRef<Person> _Person;
		
	#endregion
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
		
		partial void OnIdChanging(int value);
		partial void OnIdChanged();
		
		partial void OnPeopleIdChanging(int? value);
		partial void OnPeopleIdChanged();
		
		partial void OnImgIdChanging(int? value);
		partial void OnImgIdChanged();
		
		partial void OnIsDocumentChanging(bool? value);
		partial void OnIsDocumentChanged();
		
		partial void OnActiveInAnotherChurchChanging(bool? value);
		partial void OnActiveInAnotherChurchChanged();
		
		partial void OnShirtSizeChanging(string value);
		partial void OnShirtSizeChanged();
		
		partial void OnMedAllergyChanging(bool? value);
		partial void OnMedAllergyChanged();
		
		partial void OnEmailChanging(string value);
		partial void OnEmailChanged();
		
		partial void OnMedicalDescriptionChanging(string value);
		partial void OnMedicalDescriptionChanged();
		
		partial void OnFnameChanging(string value);
		partial void OnFnameChanged();
		
		partial void OnMnameChanging(string value);
		partial void OnMnameChanged();
		
		partial void OnCoachingChanging(bool? value);
		partial void OnCoachingChanged();
		
		partial void OnMemberChanging(bool? value);
		partial void OnMemberChanged();
		
		partial void OnEmcontactChanging(string value);
		partial void OnEmcontactChanged();
		
		partial void OnEmphoneChanging(string value);
		partial void OnEmphoneChanged();
		
		partial void OnDoctorChanging(string value);
		partial void OnDoctorChanged();
		
		partial void OnDocphoneChanging(string value);
		partial void OnDocphoneChanged();
		
		partial void OnInsuranceChanging(string value);
		partial void OnInsuranceChanged();
		
		partial void OnPolicyChanging(string value);
		partial void OnPolicyChanged();
		
		partial void OnCommentsChanging(string value);
		partial void OnCommentsChanged();
		
		partial void OnTylenolChanging(bool? value);
		partial void OnTylenolChanged();
		
		partial void OnAdvilChanging(bool? value);
		partial void OnAdvilChanged();
		
		partial void OnMaaloxChanging(bool? value);
		partial void OnMaaloxChanged();
		
		partial void OnRobitussinChanging(bool? value);
		partial void OnRobitussinChanged();
		
    #endregion
		public RecReg()
		{
			
			
			this._Person = default(EntityRef<Person>); 
			
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

		
		[Column(Name="PeopleId", UpdateCheck=UpdateCheck.Never, Storage="_PeopleId", DbType="int")]
		[IsForeignKey]
		public int? PeopleId
		{
			get { return this._PeopleId; }

			set
			{
				if (this._PeopleId != value)
				{
				
					if (this._Person.HasLoadedOrAssignedValue)
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
				
                    this.OnPeopleIdChanging(value);
					this.SendPropertyChanging();
					this._PeopleId = value;
					this.SendPropertyChanged("PeopleId");
					this.OnPeopleIdChanged();
				}

			}

		}

		
		[Column(Name="ImgId", UpdateCheck=UpdateCheck.Never, Storage="_ImgId", DbType="int")]
		public int? ImgId
		{
			get { return this._ImgId; }

			set
			{
				if (this._ImgId != value)
				{
				
                    this.OnImgIdChanging(value);
					this.SendPropertyChanging();
					this._ImgId = value;
					this.SendPropertyChanged("ImgId");
					this.OnImgIdChanged();
				}

			}

		}

		
		[Column(Name="IsDocument", UpdateCheck=UpdateCheck.Never, Storage="_IsDocument", DbType="bit")]
		public bool? IsDocument
		{
			get { return this._IsDocument; }

			set
			{
				if (this._IsDocument != value)
				{
				
                    this.OnIsDocumentChanging(value);
					this.SendPropertyChanging();
					this._IsDocument = value;
					this.SendPropertyChanged("IsDocument");
					this.OnIsDocumentChanged();
				}

			}

		}

		
		[Column(Name="ActiveInAnotherChurch", UpdateCheck=UpdateCheck.Never, Storage="_ActiveInAnotherChurch", DbType="bit")]
		public bool? ActiveInAnotherChurch
		{
			get { return this._ActiveInAnotherChurch; }

			set
			{
				if (this._ActiveInAnotherChurch != value)
				{
				
                    this.OnActiveInAnotherChurchChanging(value);
					this.SendPropertyChanging();
					this._ActiveInAnotherChurch = value;
					this.SendPropertyChanged("ActiveInAnotherChurch");
					this.OnActiveInAnotherChurchChanged();
				}

			}

		}

		
		[Column(Name="ShirtSize", UpdateCheck=UpdateCheck.Never, Storage="_ShirtSize", DbType="nvarchar(50)")]
		public string ShirtSize
		{
			get { return this._ShirtSize; }

			set
			{
				if (this._ShirtSize != value)
				{
				
                    this.OnShirtSizeChanging(value);
					this.SendPropertyChanging();
					this._ShirtSize = value;
					this.SendPropertyChanged("ShirtSize");
					this.OnShirtSizeChanged();
				}

			}

		}

		
		[Column(Name="MedAllergy", UpdateCheck=UpdateCheck.Never, Storage="_MedAllergy", DbType="bit")]
		public bool? MedAllergy
		{
			get { return this._MedAllergy; }

			set
			{
				if (this._MedAllergy != value)
				{
				
                    this.OnMedAllergyChanging(value);
					this.SendPropertyChanging();
					this._MedAllergy = value;
					this.SendPropertyChanged("MedAllergy");
					this.OnMedAllergyChanged();
				}

			}

		}

		
		[Column(Name="email", UpdateCheck=UpdateCheck.Never, Storage="_Email", DbType="nvarchar(80)")]
		public string Email
		{
			get { return this._Email; }

			set
			{
				if (this._Email != value)
				{
				
                    this.OnEmailChanging(value);
					this.SendPropertyChanging();
					this._Email = value;
					this.SendPropertyChanged("Email");
					this.OnEmailChanged();
				}

			}

		}

		
		[Column(Name="MedicalDescription", UpdateCheck=UpdateCheck.Never, Storage="_MedicalDescription", DbType="nvarchar(1000)")]
		public string MedicalDescription
		{
			get { return this._MedicalDescription; }

			set
			{
				if (this._MedicalDescription != value)
				{
				
                    this.OnMedicalDescriptionChanging(value);
					this.SendPropertyChanging();
					this._MedicalDescription = value;
					this.SendPropertyChanged("MedicalDescription");
					this.OnMedicalDescriptionChanged();
				}

			}

		}

		
		[Column(Name="fname", UpdateCheck=UpdateCheck.Never, Storage="_Fname", DbType="nvarchar(80)")]
		public string Fname
		{
			get { return this._Fname; }

			set
			{
				if (this._Fname != value)
				{
				
                    this.OnFnameChanging(value);
					this.SendPropertyChanging();
					this._Fname = value;
					this.SendPropertyChanged("Fname");
					this.OnFnameChanged();
				}

			}

		}

		
		[Column(Name="mname", UpdateCheck=UpdateCheck.Never, Storage="_Mname", DbType="nvarchar(80)")]
		public string Mname
		{
			get { return this._Mname; }

			set
			{
				if (this._Mname != value)
				{
				
                    this.OnMnameChanging(value);
					this.SendPropertyChanging();
					this._Mname = value;
					this.SendPropertyChanged("Mname");
					this.OnMnameChanged();
				}

			}

		}

		
		[Column(Name="coaching", UpdateCheck=UpdateCheck.Never, Storage="_Coaching", DbType="bit")]
		public bool? Coaching
		{
			get { return this._Coaching; }

			set
			{
				if (this._Coaching != value)
				{
				
                    this.OnCoachingChanging(value);
					this.SendPropertyChanging();
					this._Coaching = value;
					this.SendPropertyChanged("Coaching");
					this.OnCoachingChanged();
				}

			}

		}

		
		[Column(Name="member", UpdateCheck=UpdateCheck.Never, Storage="_Member", DbType="bit")]
		public bool? Member
		{
			get { return this._Member; }

			set
			{
				if (this._Member != value)
				{
				
                    this.OnMemberChanging(value);
					this.SendPropertyChanging();
					this._Member = value;
					this.SendPropertyChanged("Member");
					this.OnMemberChanged();
				}

			}

		}

		
		[Column(Name="emcontact", UpdateCheck=UpdateCheck.Never, Storage="_Emcontact", DbType="nvarchar(100)")]
		public string Emcontact
		{
			get { return this._Emcontact; }

			set
			{
				if (this._Emcontact != value)
				{
				
                    this.OnEmcontactChanging(value);
					this.SendPropertyChanging();
					this._Emcontact = value;
					this.SendPropertyChanged("Emcontact");
					this.OnEmcontactChanged();
				}

			}

		}

		
		[Column(Name="emphone", UpdateCheck=UpdateCheck.Never, Storage="_Emphone", DbType="nvarchar(50)")]
		public string Emphone
		{
			get { return this._Emphone; }

			set
			{
				if (this._Emphone != value)
				{
				
                    this.OnEmphoneChanging(value);
					this.SendPropertyChanging();
					this._Emphone = value;
					this.SendPropertyChanged("Emphone");
					this.OnEmphoneChanged();
				}

			}

		}

		
		[Column(Name="doctor", UpdateCheck=UpdateCheck.Never, Storage="_Doctor", DbType="nvarchar(100)")]
		public string Doctor
		{
			get { return this._Doctor; }

			set
			{
				if (this._Doctor != value)
				{
				
                    this.OnDoctorChanging(value);
					this.SendPropertyChanging();
					this._Doctor = value;
					this.SendPropertyChanged("Doctor");
					this.OnDoctorChanged();
				}

			}

		}

		
		[Column(Name="docphone", UpdateCheck=UpdateCheck.Never, Storage="_Docphone", DbType="nvarchar(50)")]
		public string Docphone
		{
			get { return this._Docphone; }

			set
			{
				if (this._Docphone != value)
				{
				
                    this.OnDocphoneChanging(value);
					this.SendPropertyChanging();
					this._Docphone = value;
					this.SendPropertyChanged("Docphone");
					this.OnDocphoneChanged();
				}

			}

		}

		
		[Column(Name="insurance", UpdateCheck=UpdateCheck.Never, Storage="_Insurance", DbType="nvarchar(100)")]
		public string Insurance
		{
			get { return this._Insurance; }

			set
			{
				if (this._Insurance != value)
				{
				
                    this.OnInsuranceChanging(value);
					this.SendPropertyChanging();
					this._Insurance = value;
					this.SendPropertyChanged("Insurance");
					this.OnInsuranceChanged();
				}

			}

		}

		
		[Column(Name="policy", UpdateCheck=UpdateCheck.Never, Storage="_Policy", DbType="nvarchar(100)")]
		public string Policy
		{
			get { return this._Policy; }

			set
			{
				if (this._Policy != value)
				{
				
                    this.OnPolicyChanging(value);
					this.SendPropertyChanging();
					this._Policy = value;
					this.SendPropertyChanged("Policy");
					this.OnPolicyChanged();
				}

			}

		}

		
		[Column(Name="Comments", UpdateCheck=UpdateCheck.Never, Storage="_Comments", DbType="nvarchar")]
		public string Comments
		{
			get { return this._Comments; }

			set
			{
				if (this._Comments != value)
				{
				
                    this.OnCommentsChanging(value);
					this.SendPropertyChanging();
					this._Comments = value;
					this.SendPropertyChanged("Comments");
					this.OnCommentsChanged();
				}

			}

		}

		
		[Column(Name="Tylenol", UpdateCheck=UpdateCheck.Never, Storage="_Tylenol", DbType="bit")]
		public bool? Tylenol
		{
			get { return this._Tylenol; }

			set
			{
				if (this._Tylenol != value)
				{
				
                    this.OnTylenolChanging(value);
					this.SendPropertyChanging();
					this._Tylenol = value;
					this.SendPropertyChanged("Tylenol");
					this.OnTylenolChanged();
				}

			}

		}

		
		[Column(Name="Advil", UpdateCheck=UpdateCheck.Never, Storage="_Advil", DbType="bit")]
		public bool? Advil
		{
			get { return this._Advil; }

			set
			{
				if (this._Advil != value)
				{
				
                    this.OnAdvilChanging(value);
					this.SendPropertyChanging();
					this._Advil = value;
					this.SendPropertyChanged("Advil");
					this.OnAdvilChanged();
				}

			}

		}

		
		[Column(Name="Maalox", UpdateCheck=UpdateCheck.Never, Storage="_Maalox", DbType="bit")]
		public bool? Maalox
		{
			get { return this._Maalox; }

			set
			{
				if (this._Maalox != value)
				{
				
                    this.OnMaaloxChanging(value);
					this.SendPropertyChanging();
					this._Maalox = value;
					this.SendPropertyChanged("Maalox");
					this.OnMaaloxChanged();
				}

			}

		}

		
		[Column(Name="Robitussin", UpdateCheck=UpdateCheck.Never, Storage="_Robitussin", DbType="bit")]
		public bool? Robitussin
		{
			get { return this._Robitussin; }

			set
			{
				if (this._Robitussin != value)
				{
				
                    this.OnRobitussinChanging(value);
					this.SendPropertyChanging();
					this._Robitussin = value;
					this.SendPropertyChanged("Robitussin");
					this.OnRobitussinChanged();
				}

			}

		}

		
    #endregion
        
    #region Foreign Key Tables
   		
	#endregion
	
	#region Foreign Keys
    	
		[Association(Name="FK_RecReg_People", Storage="_Person", ThisKey="PeopleId", IsForeignKey=true)]
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
						previousValue.RecRegs.Remove(this);
					}

					this._Person.Entity = value;
					if (value != null)
					{
						value.RecRegs.Add(this);
						
						this._PeopleId = value.PeopleId;
						
					}

					else
					{
						
						this._PeopleId = default(int?);
						
					}

					this.SendPropertyChanged("Person");
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

