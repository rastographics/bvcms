/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */
using System;
using System.Configuration;
using System.Threading;

namespace UtilityExtensions
{
    public static partial class Util
    {
        public static string UserName
        {
            get
            {
                if (HttpContextFactory.Current != null)
                    return GetUserName(HttpContextFactory.Current.User.Identity.Name);
                return ConfigurationManager.AppSettings["TestName"];
            }
        }

        private const string STR_UserId = "UserId";
        public static int UserId
        {
            get => GetFromSession(STR_UserId).ToInt();

            set => SetValueInSession(STR_UserId, value);
        }

        private static void SetValueInSession(string name, object value)
        {
            if (HttpContextFactory.Current != null)
            {
                if (HttpContextFactory.Current.Session != null)
                {
                    HttpContextFactory.Current.Session[name] = value;
                }
                else
                {
                    HttpContextFactory.Current.Items[name] = value;
                }
            }
        }

        private static object GetFromSession(string name, object defaultValue = null)
        {
            object value = defaultValue;
            if (HttpContextFactory.Current != null)
            {
                if (HttpContextFactory.Current.Session != null)
                {
                    if (HttpContextFactory.Current.Session[name] != null)
                    {
                        value = HttpContextFactory.Current.Session[name];
                    }
                }
                else
                {
                    value = HttpContextFactory.Current.Items[name];
                }
            }
            return value;
        }

        private const string STR_ActivePerson = "ActivePerson";
        public static string ActivePerson
        {
            get => GetFromSession(STR_ActivePerson) as string;
            set => SetValueInSession(STR_ActivePerson, value);
        }

        private const string STR_UserPreferredName = "UserPreferredName";
        public static string UserPreferredName
        {
            get => GetFromSession(STR_UserPreferredName) as string;
            set => SetValueInSession(STR_UserPreferredName, value);
        }

        private const string STR_UserFullName = "UserFullName";
        public static string UserFullName
        {
            get => GetFromSession(STR_UserFullName) as string;
            set => SetValueInSession(STR_UserFullName, value);
        }

        private const string STR_UserFirstName = "UserFirstName";
        public static string UserFirstName
        {
            get => GetFromSession(STR_UserFirstName) as string;
            set => SetValueInSession(STR_UserFirstName, value);
        }

        public static int UserId1 => UserId == 0 ? 1 : UserId;

        private const string STR_UserPeopleId = "UserPeopleId";
        public static int? UserPeopleId
        {
            get => GetFromSession(STR_UserPeopleId).ToInt();
            set => SetValueInSession(STR_UserPeopleId, value);
        }

        private const string UserThumbPictureSessionKey = "UserThumbPictureUrl";
        public static string UserThumbPictureUrl
        {
            get => GetFromSession(UserThumbPictureSessionKey) as string;
            set => SetValueInSession(UserThumbPictureSessionKey, value);
        }

        private const string UserThumbPictureBgPosSessionKey = "UserThumbPictureBgPosition";
        public static string UserThumbPictureBgPosition
        {
            get => GetFromSession(UserThumbPictureBgPosSessionKey) as string ?? "";
            set => SetValueInSession(UserThumbPictureBgPosSessionKey, value);
        }

        public static string GetUserName(string name)
        {
            if (name == null)
                return null;
            var a = name.Split('\\');
            if (a.Length == 2)
                return a[1];
            return a[0];
        }

        public static bool IsMyDataUser
        {
            get
            {
                if (HttpContextFactory.Current != null)
                    return HttpContextFactory.Current.User.IsInRole("Access") == false; 
                return (bool?) Thread.GetData(Thread.GetNamedDataSlot("IsMyDataUser")) ?? false;
            }
            set
            {
                if (HttpContextFactory.Current == null)
                    Thread.SetData(Thread.GetNamedDataSlot("IsMyDataUser"), value);
            }
        }
    }
}

