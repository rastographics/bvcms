using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;
using UtilityExtensions.Session;

namespace UtilityExtensions
{
    public static partial class Util
    {
        public static string UserName
        {
            get
            {
                if (HttpContextFactory.Current != null)
                {
                    return GetUserName(HttpContextFactory.Current.User?.Identity?.Name);
                }

                return ConfigurationManager.AppSettings["TestName"];
            }
        }

        private static ISessionProvider GetSessionProvider()
        {
            return HttpContextFactory.Current?.Items?["SessionProvider"] as ISessionProvider;
        }

        public static void SetValueInSession(string name, object value)
        {
            var session = GetSessionProvider();
            if (session != null)
            {
                session.Add(name, value);
            }
            else
            {
                Thread.SetData(Thread.GetNamedDataSlot(name), value);
            }
        }

        public static T GetFromSession<T>(string name, T defaultValue = null) where T : class
        {
            T value;
            var session = GetSessionProvider();
            if (session != null)
            {
                value = session.Get(name, defaultValue);
            }
            else
            {
                value = Thread.GetData(Thread.GetNamedDataSlot(name)) as T;
            }
            return value ?? defaultValue;
        }

        public static string GetFromSessionTemp(string name, string defaultValue = null)
        {
            var value = GetFromSession<string>(name);
            if (value.HasValue())
            {
                SetValueInSession(name, null);
            }
            return value;
        }

        public static bool GetFromSession(string name, bool defaultValue = false)
        {
            var strValue = GetFromSession<string>(name);
            return bool.TryParse(strValue, out bool v) ? v : defaultValue;
        }

        public static int? GetFromSession(string name, int? defaultValue = null)
        {
            var strValue = GetFromSession<string>(name);
            return int.TryParse(strValue, out int v) ? v : defaultValue;
        }

        public static int GetFromSession(string name, int defaultValue = 0)
        {
            var strValue = GetFromSession<string>(name);
            return int.TryParse(strValue, out int v) ? v : defaultValue;
        }

        public static DateTime? GetFromSession(string name, DateTime? defaultValue = null)
        {
            var strValue = GetFromSession<string>(name);
            return DateTime.TryParse(strValue, out DateTime v) ? v : defaultValue;
        }

        private const string STR_ActiveOrganization = "ActiveOrganization";
        public static string ActiveOrganization
        {
            get => GetFromSession<string>(STR_ActiveOrganization);
            set => SetValueInSession(STR_ActiveOrganization, value);
        }

        private const string STR_ActivePerson = "ActivePerson";
        public static string ActivePerson
        {
            get => GetFromSession<string>(STR_ActivePerson);
            set => SetValueInSession(STR_ActivePerson, value);
        }

        private const string STR_ContentKeywordFilter = "ContentKeywordFilter";
        public static string ContentKeywordFilter
        {
            get => GetFromSession<string>(STR_ContentKeywordFilter);
            set => SetValueInSession(STR_ContentKeywordFilter, value);
        }

        private const string STR_CreditCardOnFile = "CreditCardOnFile";
        public static string CreditCardOnFile
        {
            get => GetFromSession<string>(STR_CreditCardOnFile);
            set => SetValueInSession(STR_CreditCardOnFile, value);
        }

        private const string STR_DefaultFunds = "DefaultFunds";
        public static string DefaultFunds
        {
            get => GetFromSession<string>(STR_DefaultFunds);
            set => SetValueInSession(STR_DefaultFunds, value);
        }

        private const string STR_DefaultGivingYear = "DefaultGivingYear";
        public static string DefaultGivingYear
        {
            get => GetFromSession<string>(STR_DefaultGivingYear);
            set => SetValueInSession(STR_DefaultGivingYear, value);
        }

        private const string STR_ExpiresOnFile = "ExpiresOnFile";
        public static string ExpiresOnFile
        {
            get => GetFromSession<string>(STR_ExpiresOnFile);
            set => SetValueInSession(STR_ExpiresOnFile, value);
        }

        private const string STR_FormId = "FormId";
        public static Guid? FormId
        {
            get => GetFromSession<string>(STR_FormId).ToGuid();
            set => SetValueInSession(STR_FormId, value.HasValue ? ToCode(value.Value) : null);
        }

        private const string STR_IsNonFinanceImpersonator = "IsNonFinanceImpersonator";
        public static bool? IsNonFinanceImpersonator
        {
            get => GetFromSession(STR_IsNonFinanceImpersonator, false);
            set => SetValueInSession(STR_IsNonFinanceImpersonator, value);
        }

        private const string STR_MFAUserId = "MFAUserId";
        public static int? MFAUserId
        {
            get => GetFromSession(STR_MFAUserId, (int?)null);
            set => SetValueInSession(STR_MFAUserId, value);
        }

        private const string STR_OnlineRegLogin = "OnlineRegLogin";
        public static bool OnlineRegLogin
        {
            get => GetFromSession(STR_OnlineRegLogin, false);
            set => SetValueInSession(STR_OnlineRegLogin, value);
        }

        private const string STR_OrgCopySettings = "OrgCopySettings";
        public static int? OrgCopySettings
        {
            get => GetFromSession(STR_OrgCopySettings, (int?)null);
            set => SetValueInSession(STR_OrgCopySettings, value);
        }

        private const string STR_OrgPickList = "OrgPickList";
        public static List<int> OrgPickList
        {
            get => GetFromSession<List<int>>(STR_OrgPickList);
            set => SetValueInSession(STR_OrgPickList, value);
        }

        private const string STR_OrgSearch = "OrgSearch";
        public static string OrgSearch
        {
            get => GetFromSession<string>(STR_OrgSearch, null);
            set => SetValueInSession(STR_OrgSearch, value);
        }

        private const string STR_QueryClipboard = "QueryClipboard";
        public static string QueryClipboard
        {
            get => GetFromSession<string>(STR_QueryClipboard);
            set => SetValueInSession(STR_QueryClipboard, value);
        }

        private const string STR_ShowAllMeetings = "ShowAllMeetings";
        public static bool? ShowAllMeetings
        {
            get => GetFromSession(STR_ShowAllMeetings, false);
            set => SetValueInSession(STR_ShowAllMeetings, value);
        }

        private const string STR_ShowCheckImages = "ShowCheckImages";
        public static bool ShowCheckImages
        {
            get => GetFromSession(STR_ShowCheckImages, false);
            set => SetValueInSession(STR_ShowCheckImages, value);
        }

        private const string STR_TempAutorun = "TempAutorun";
        public static bool TempAutorun
        {
            get => GetFromSessionTemp(STR_TempAutorun).ToBool();
            set => SetValueInSession(STR_TempAutorun, value.ToString());
        }

        private const string STR_TempCodeList = "TempCodeList";
        public static string TempCodeList
        {
            get => GetFromSessionTemp(STR_TempCodeList);
            set => SetValueInSession(STR_TempCodeList, value?.ToString());
        }

        private const string STR_TempContactEdit = "TempContactEdit";
        public static bool TempContactEdit
        {
            get => GetFromSessionTemp(STR_TempContactEdit).ToBool();
            set => SetValueInSession(STR_TempContactEdit, value.ToString());
        }

        private const string STR_TempError = "TempError";
        public static string TempError
        {
            get => GetFromSessionTemp(STR_TempError);
            set => SetValueInSession(STR_TempError, value?.ToString());
        }

        private const string STR_TempPeopleId = "TempPeopleId";
        public static int? TempPeopleId
        {
            get => GetFromSessionTemp(STR_TempPeopleId).ToInt2();
            set => SetValueInSession(STR_TempPeopleId, value.ToString());
        }

        private const string STR_TempSetRole = "TempSetRole";
        public static string TempSetRole
        {
            get => GetFromSessionTemp(STR_TempSetRole);
            set => SetValueInSession(STR_TempSetRole, value?.ToString());
        }

        private const string STR_TempSuccessMessage = "TempSuccessMessage";
        public static string TempSuccessMessage
        {
            get => GetFromSessionTemp(STR_TempSuccessMessage);
            set => SetValueInSession(STR_TempSuccessMessage, value?.ToString());
        }

        private const string STR_TestNoFinance = "TestNoFinance";
        public static bool TestNoFinance
        {
            get => GetFromSession(STR_TestNoFinance, false);
            set => SetValueInSession(STR_TestNoFinance, value);
        }

        private const string STR_UserPreferredName = "UserPreferredName";
        public static string UserPreferredName
        {
            get => GetFromSession<string>(STR_UserPreferredName);
            set => SetValueInSession(STR_UserPreferredName, value);
        }

        private const string STR_UserFullName = "UserFullName";
        public static string UserFullName
        {
            get => GetFromSession<string>(STR_UserFullName);
            set => SetValueInSession(STR_UserFullName, value);
        }

        private const string STR_UserFirstName = "UserFirstName";
        public static string UserFirstName
        {
            get => GetFromSession<string>(STR_UserFirstName);
            set => SetValueInSession(STR_UserFirstName, value);
        }

        private const string UserThumbPictureSessionKey = "UserThumbPictureUrl";
        public static string UserThumbPictureUrl
        {
            get => GetFromSession<string>(UserThumbPictureSessionKey);
            set => SetValueInSession(UserThumbPictureSessionKey, value);
        }

        private const string UserThumbPictureBgPosSessionKey = "UserThumbPictureBgPosition";
        public static string UserThumbPictureBgPosition
        {
            get => GetFromSession<string>(UserThumbPictureBgPosSessionKey) ?? "";
            set => SetValueInSession(UserThumbPictureBgPosSessionKey, value);
        }

        public static string GetUserName(string name)
        {
            if (name == null)
            {
                return null;
            }

            var a = name.Split('\\');
            if (a.Length == 2)
            {
                return a[1];
            }

            return a[0];
        }

        public static bool IsMyDataUser
        {
            get
            {
                if (HttpContextFactory.Current != null)
                {
                    return HttpContextFactory.Current.User.IsInRole("Access") == false;
                }

                return (bool?)Thread.GetData(Thread.GetNamedDataSlot("IsMyDataUser")) ?? false;
            }
            set
            {
                if (HttpContextFactory.Current == null)
                {
                    Thread.SetData(Thread.GetNamedDataSlot("IsMyDataUser"), value);
                }
            }
        }
    }
}

