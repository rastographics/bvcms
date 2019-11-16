using System;
using System.ComponentModel;
using System.Data.Linq.Mapping;

namespace CmsData.View
{
    [Table(Name = "RegsettingMessages")]
    public partial class RegsettingMessage
    {
        private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs("");

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

        [Column(Name = "OrganizationId", Storage = "_OrganizationId", AutoSync = AutoSync.OnInsert, DbType = "int NOT NULL IDENTITY", IsDbGenerated = true)]
        public int OrganizationId
        {
            get => _OrganizationId;

            set
            {
                if (_OrganizationId != value)
                {
                    _OrganizationId = value;
                }
            }
        }

        [Column(Name = "OrganizationName", Storage = "_OrganizationName", DbType = "nvarchar(100) NOT NULL")]
        public string OrganizationName
        {
            get => _OrganizationName;

            set
            {
                if (_OrganizationName != value)
                {
                    _OrganizationName = value;
                }
            }
        }

        [Column(Name = "Subject", Storage = "_Subject", DbType = "varchar(100)")]
        public string Subject
        {
            get => _Subject;

            set
            {
                if (_Subject != value)
                {
                    _Subject = value;
                }
            }
        }

        [Column(Name = "Body", Storage = "_Body", DbType = "varchar")]
        public string Body
        {
            get => _Body;

            set
            {
                if (_Body != value)
                {
                    _Body = value;
                }
            }
        }

        [Column(Name = "SenderSubject", Storage = "_SenderSubject", DbType = "varchar(100)")]
        public string SenderSubject
        {
            get => _SenderSubject;

            set
            {
                if (_SenderSubject != value)
                {
                    _SenderSubject = value;
                }
            }
        }

        [Column(Name = "SenderBody", Storage = "_SenderBody", DbType = "varchar")]
        public string SenderBody
        {
            get => _SenderBody;

            set
            {
                if (_SenderBody != value)
                {
                    _SenderBody = value;
                }
            }
        }

        [Column(Name = "SupportSubject", Storage = "_SupportSubject", DbType = "varchar(100)")]
        public string SupportSubject
        {
            get => _SupportSubject;

            set
            {
                if (_SupportSubject != value)
                {
                    _SupportSubject = value;
                }
            }
        }

        [Column(Name = "SupportBody", Storage = "_SupportBody", DbType = "varchar")]
        public string SupportBody
        {
            get => _SupportBody;

            set
            {
                if (_SupportBody != value)
                {
                    _SupportBody = value;
                }
            }
        }

        [Column(Name = "ReminderSubject", Storage = "_ReminderSubject", DbType = "varchar(100)")]
        public string ReminderSubject
        {
            get => _ReminderSubject;

            set
            {
                if (_ReminderSubject != value)
                {
                    _ReminderSubject = value;
                }
            }
        }

        [Column(Name = "ReminderBody", Storage = "_ReminderBody", DbType = "varchar")]
        public string ReminderBody
        {
            get => _ReminderBody;

            set
            {
                if (_ReminderBody != value)
                {
                    _ReminderBody = value;
                }
            }
        }

        [Column(Name = "InstructionsLogin", Storage = "_InstructionsLogin", DbType = "varchar")]
        public string InstructionsLogin
        {
            get => _InstructionsLogin;

            set
            {
                if (_InstructionsLogin != value)
                {
                    _InstructionsLogin = value;
                }
            }
        }

        [Column(Name = "InstructionsOptions", Storage = "_InstructionsOptions", DbType = "varchar")]
        public string InstructionsOptions
        {
            get => _InstructionsOptions;

            set
            {
                if (_InstructionsOptions != value)
                {
                    _InstructionsOptions = value;
                }
            }
        }

        [Column(Name = "InstructionsSubmit", Storage = "_InstructionsSubmit", DbType = "varchar")]
        public string InstructionsSubmit
        {
            get => _InstructionsSubmit;

            set
            {
                if (_InstructionsSubmit != value)
                {
                    _InstructionsSubmit = value;
                }
            }
        }

        [Column(Name = "InstructionsFind", Storage = "_InstructionsFind", DbType = "varchar")]
        public string InstructionsFind
        {
            get => _InstructionsFind;

            set
            {
                if (_InstructionsFind != value)
                {
                    _InstructionsFind = value;
                }
            }
        }

        [Column(Name = "InstructionsSelect", Storage = "_InstructionsSelect", DbType = "varchar")]
        public string InstructionsSelect
        {
            get => _InstructionsSelect;

            set
            {
                if (_InstructionsSelect != value)
                {
                    _InstructionsSelect = value;
                }
            }
        }

        [Column(Name = "InstructionsSorry", Storage = "_InstructionsSorry", DbType = "varchar")]
        public string InstructionsSorry
        {
            get => _InstructionsSorry;

            set
            {
                if (_InstructionsSorry != value)
                {
                    _InstructionsSorry = value;
                }
            }
        }

        [Column(Name = "InstructionsSpecial", Storage = "_InstructionsSpecial", DbType = "varchar")]
        public string InstructionsSpecial
        {
            get => _InstructionsSpecial;

            set
            {
                if (_InstructionsSpecial != value)
                {
                    _InstructionsSpecial = value;
                }
            }
        }

        [Column(Name = "InstructionsThanks", Storage = "_InstructionsThanks", DbType = "varchar")]
        public string InstructionsThanks
        {
            get => _InstructionsThanks;

            set
            {
                if (_InstructionsThanks != value)
                {
                    _InstructionsThanks = value;
                }
            }
        }

        [Column(Name = "InstructionsTerms", Storage = "_InstructionsTerms", DbType = "varchar")]
        public string InstructionsTerms
        {
            get => _InstructionsTerms;

            set
            {
                if (_InstructionsTerms != value)
                {
                    _InstructionsTerms = value;
                }
            }
        }
    }
}
