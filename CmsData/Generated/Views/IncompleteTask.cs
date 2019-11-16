using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "IncompleteTasks")]
    public partial class IncompleteTask
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

        private DateTime _CreatedOn;

        private string _Owner;

        private string _DelegatedTo;

        private string _Description;

        private string _DeclineReason;

        private string _About;

        private string _Notes;

        private string _Status;

        private bool? _ForceCompleteWContact;

        private int _Id;

        private int _OwnerId;

        private int? _CoOwnerId;

        private int? _WhoId;

        private int? _StatusId;

        private int? _SourceContactId;

        private DateTime? _Due;

        private int? _AboutPictureId;

        private int? _PictureX;

        private int? _PictureY;

        public IncompleteTask()
        {
        }

        [Column(Name = "CreatedOn", Storage = "_CreatedOn", DbType = "datetime NOT NULL")]
        public DateTime CreatedOn
        {
            get => _CreatedOn;

            set
            {
                if (_CreatedOn != value)
                {
                    _CreatedOn = value;
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

        [Column(Name = "DelegatedTo", Storage = "_DelegatedTo", DbType = "nvarchar(138)")]
        public string DelegatedTo
        {
            get => _DelegatedTo;

            set
            {
                if (_DelegatedTo != value)
                {
                    _DelegatedTo = value;
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

        [Column(Name = "Due", Storage = "_Due", DbType = "datetime")]
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

        [Column(Name = "AboutPictureId", Storage = "_AboutPictureId", DbType = "int")]
        public int? AboutPictureId
        {
            get => _AboutPictureId;

            set
            {
                if (_AboutPictureId != value)
                {
                    _AboutPictureId = value;
                }
            }
        }

        [Column(Name = "PictureX", Storage = "_PictureX", DbType = "int")]
        public int? PictureX
        {
            get => _PictureX;

            set
            {
                if (_PictureX != value)
                {
                    _PictureX = value;
                }
            }
        }

        [Column(Name = "PictureY", Storage = "_PictureY", DbType = "int")]
        public int? PictureY
        {
            get => _PictureY;

            set
            {
                if (_PictureY != value)
                {
                    _PictureY = value;
                }
            }
        }
    }
}
