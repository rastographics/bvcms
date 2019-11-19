using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "TaskSearch")]
    public partial class TaskSearch
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private DateTime? _Created;

        private string _Status;

        private DateTime? _Due;

        private DateTime? _Completed;

        private bool _Archive;

        private string _Notes;

        private string _Description;

        private bool? _ForceCompleteWContact;

        private string _DeclineReason;

        private string _LimitToRole;

        private string _Originator;

        private string _Owner;

        private string _DelegateX;

        private string _About;

        private string _Originator2;

        private string _Owner2;

        private string _Delegate2;

        private string _About2;

        private int _Id;

        private int? _StatusId;

        private int _OwnerId;

        private int? _CoOwnerId;

        private int? _OrginatorId;

        private int? _WhoId;

        private int? _SourceContactId;

        private int? _CompletedContactId;

        public TaskSearch()
        {
        }

        [Column(Name = "Created", Storage = "_Created", DbType = "date")]
        public DateTime? Created
        {
            get => _Created;

            set
            {
                if (_Created != value)
                {
                    _Created = value;
                }
            }
        }

        [Column(Name = "Status", Storage = "_Status", DbType = "nvarchar(100)")]
        public string Status
        {
            get => _Status;

            set
            {
                if (_Status != value)
                {
                    _Status = value;
                }
            }
        }

        [Column(Name = "Due", Storage = "_Due", DbType = "date")]
        public DateTime? Due
        {
            get => _Due;

            set
            {
                if (_Due != value)
                {
                    _Due = value;
                }
            }
        }

        [Column(Name = "Completed", Storage = "_Completed", DbType = "date")]
        public DateTime? Completed
        {
            get => _Completed;

            set
            {
                if (_Completed != value)
                {
                    _Completed = value;
                }
            }
        }

        [Column(Name = "Archive", Storage = "_Archive", DbType = "bit NOT NULL")]
        public bool Archive
        {
            get => _Archive;

            set
            {
                if (_Archive != value)
                {
                    _Archive = value;
                }
            }
        }

        [Column(Name = "Notes", Storage = "_Notes", DbType = "nvarchar")]
        public string Notes
        {
            get => _Notes;

            set
            {
                if (_Notes != value)
                {
                    _Notes = value;
                }
            }
        }

        [Column(Name = "Description", Storage = "_Description", DbType = "nvarchar(100)")]
        public string Description
        {
            get => _Description;

            set
            {
                if (_Description != value)
                {
                    _Description = value;
                }
            }
        }

        [Column(Name = "ForceCompleteWContact", Storage = "_ForceCompleteWContact", DbType = "bit")]
        public bool? ForceCompleteWContact
        {
            get => _ForceCompleteWContact;

            set
            {
                if (_ForceCompleteWContact != value)
                {
                    _ForceCompleteWContact = value;
                }
            }
        }

        [Column(Name = "DeclineReason", Storage = "_DeclineReason", DbType = "nvarchar")]
        public string DeclineReason
        {
            get => _DeclineReason;

            set
            {
                if (_DeclineReason != value)
                {
                    _DeclineReason = value;
                }
            }
        }

        [Column(Name = "LimitToRole", Storage = "_LimitToRole", DbType = "nvarchar(50)")]
        public string LimitToRole
        {
            get => _LimitToRole;

            set
            {
                if (_LimitToRole != value)
                {
                    _LimitToRole = value;
                }
            }
        }

        [Column(Name = "Originator", Storage = "_Originator", DbType = "nvarchar(138)")]
        public string Originator
        {
            get => _Originator;

            set
            {
                if (_Originator != value)
                {
                    _Originator = value;
                }
            }
        }

        [Column(Name = "Owner", Storage = "_Owner", DbType = "nvarchar(138)")]
        public string Owner
        {
            get => _Owner;

            set
            {
                if (_Owner != value)
                {
                    _Owner = value;
                }
            }
        }

        [Column(Name = "Delegate", Storage = "_DelegateX", DbType = "nvarchar(138)")]
        public string DelegateX
        {
            get => _DelegateX;

            set
            {
                if (_DelegateX != value)
                {
                    _DelegateX = value;
                }
            }
        }

        [Column(Name = "About", Storage = "_About", DbType = "nvarchar(138)")]
        public string About
        {
            get => _About;

            set
            {
                if (_About != value)
                {
                    _About = value;
                }
            }
        }

        [Column(Name = "Originator2", Storage = "_Originator2", DbType = "nvarchar(139)")]
        public string Originator2
        {
            get => _Originator2;

            set
            {
                if (_Originator2 != value)
                {
                    _Originator2 = value;
                }
            }
        }

        [Column(Name = "Owner2", Storage = "_Owner2", DbType = "nvarchar(139)")]
        public string Owner2
        {
            get => _Owner2;

            set
            {
                if (_Owner2 != value)
                {
                    _Owner2 = value;
                }
            }
        }

        [Column(Name = "Delegate2", Storage = "_Delegate2", DbType = "nvarchar(139)")]
        public string Delegate2
        {
            get => _Delegate2;

            set
            {
                if (_Delegate2 != value)
                {
                    _Delegate2 = value;
                }
            }
        }

        [Column(Name = "About2", Storage = "_About2", DbType = "nvarchar(139)")]
        public string About2
        {
            get => _About2;

            set
            {
                if (_About2 != value)
                {
                    _About2 = value;
                }
            }
        }

        [Column(Name = "Id", Storage = "_Id", DbType = "int NOT NULL")]
        public int Id
        {
            get => _Id;

            set
            {
                if (_Id != value)
                {
                    _Id = value;
                }
            }
        }

        [Column(Name = "StatusId", Storage = "_StatusId", DbType = "int")]
        public int? StatusId
        {
            get => _StatusId;

            set
            {
                if (_StatusId != value)
                {
                    _StatusId = value;
                }
            }
        }

        [Column(Name = "OwnerId", Storage = "_OwnerId", DbType = "int NOT NULL")]
        public int OwnerId
        {
            get => _OwnerId;

            set
            {
                if (_OwnerId != value)
                {
                    _OwnerId = value;
                }
            }
        }

        [Column(Name = "CoOwnerId", Storage = "_CoOwnerId", DbType = "int")]
        public int? CoOwnerId
        {
            get => _CoOwnerId;

            set
            {
                if (_CoOwnerId != value)
                {
                    _CoOwnerId = value;
                }
            }
        }

        [Column(Name = "OrginatorId", Storage = "_OrginatorId", DbType = "int")]
        public int? OrginatorId
        {
            get => _OrginatorId;

            set
            {
                if (_OrginatorId != value)
                {
                    _OrginatorId = value;
                }
            }
        }

        [Column(Name = "WhoId", Storage = "_WhoId", DbType = "int")]
        public int? WhoId
        {
            get => _WhoId;

            set
            {
                if (_WhoId != value)
                {
                    _WhoId = value;
                }
            }
        }

        [Column(Name = "SourceContactId", Storage = "_SourceContactId", DbType = "int")]
        public int? SourceContactId
        {
            get => _SourceContactId;

            set
            {
                if (_SourceContactId != value)
                {
                    _SourceContactId = value;
                }
            }
        }

        [Column(Name = "CompletedContactId", Storage = "_CompletedContactId", DbType = "int")]
        public int? CompletedContactId
        {
            get => _CompletedContactId;

            set
            {
                if (_CompletedContactId != value)
                {
                    _CompletedContactId = value;
                }
            }
        }
    }
}
