/* Author: David Carroll
 * Copyright (c) 2008, 2009 Bellevue Baptist Church
 * Licensed under the GNU General Public License (GPL v2)
 * you may not use this code except in compliance with the License.
 * You may obtain a copy of the License at http://bvcms.codeplex.com/license
 */
using System;
using System.Web;
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
            get
            {
                int id = 0;
                if (HttpContextFactory.Current != null)
                    if (HttpContextFactory.Current.Session != null)
                        if (HttpContextFactory.Current.Session[STR_UserId] != null)
                            id = HttpContextFactory.Current.Session[STR_UserId].ToInt();
                if (id == 0)
                    id = ConfigurationManager.AppSettings["TestId"].ToInt();
                return id;
            }
            set
            {
                if (HttpContextFactory.Current != null)
                    if (HttpContextFactory.Current.Session != null)
                        HttpContextFactory.Current.Session[STR_UserId] = value;
            }
        }

        private const string STR_UserPreferredName = "UserPreferredName";
        public static string UserPreferredName
        {
            get
            {
                string name = null;
                if (HttpContextFactory.Current != null)
                    if (HttpContextFactory.Current.Session != null)
                        if (HttpContextFactory.Current.Session[STR_UserPreferredName] != null)
                            name = HttpContextFactory.Current.Session[STR_UserPreferredName] as String;
                return name;
            }
            set
            {
                if (HttpContextFactory.Current != null)
                    if (HttpContextFactory.Current.Session != null)
                        HttpContextFactory.Current.Session[STR_UserPreferredName] = value;
            }
        }

        private const string STR_UserFullName = "UserFullName";
        public static string UserFullName
        {
            get
            {
                string name = "-";
                if (HttpContextFactory.Current != null)
                    if (HttpContextFactory.Current.Session != null)
                        if (HttpContextFactory.Current.Session[STR_UserFullName] != null)
                            name = HttpContextFactory.Current.Session[STR_UserFullName] as String;
                return name;
            }
            set
            {
                if (HttpContextFactory.Current != null)
                    if (HttpContextFactory.Current.Session != null)
                        HttpContextFactory.Current.Session[STR_UserFullName] = value;
            }
        }

        private const string UserFirstNameSessionKey = "UserFirstName";
        public static string UserFirstName
        {
            get
            {
                var name = string.Empty;
                if (HttpContextFactory.Current != null)
                    if (HttpContextFactory.Current.Session != null)
                        if (HttpContextFactory.Current.Session[UserFirstNameSessionKey] != null)
                            name = HttpContextFactory.Current.Session[UserFirstNameSessionKey] as String;
                return name;
            }
            set
            {
                if (HttpContextFactory.Current != null)
                    if (HttpContextFactory.Current.Session != null)
                        HttpContextFactory.Current.Session[UserFirstNameSessionKey] = value;
            }
        }

        public static int UserId1 => UserId == 0 ? 1 : UserId;

        private const string STR_UserPeopleId = "UserPeopleId";
        public static int? UserPeopleId
        {
            get
            {
                int? id = null;
                if (HttpContextFactory.Current != null)
                    if (HttpContextFactory.Current.Session != null)
                        if (HttpContextFactory.Current.Session[STR_UserPeopleId] != null)
                            id = HttpContextFactory.Current.Session[STR_UserPeopleId].ToInt();
                return id;
            }
            set
            {
                if (HttpContextFactory.Current != null)
                    HttpContextFactory.Current.Session[STR_UserPeopleId] = value;
            }
        }

        private const string UserThumbPictureSessionKey = "UserThumbPictureUrl";
        public static string UserThumbPictureUrl
        {
            get
            {
                if (HttpContextFactory.Current != null)
                    if (HttpContextFactory.Current.Session != null)
                        if (HttpContextFactory.Current.Session[UserThumbPictureSessionKey] != null)
                            return (string)HttpContextFactory.Current.Session[UserThumbPictureSessionKey];
                return string.Empty;
            }
            set
            {
                if (HttpContextFactory.Current != null)
                    HttpContextFactory.Current.Session[UserThumbPictureSessionKey] = value;
            }
        }

        private const string UserThumbPictureBgPosSessionKey = "UserThumbPictureBgPosition";
        public static string UserThumbPictureBgPosition
        {
            get
            {
                if (HttpContextFactory.Current != null)
                    if (HttpContextFactory.Current.Session != null)
                        if (HttpContextFactory.Current.Session[UserThumbPictureBgPosSessionKey] != null)
                            return (string)HttpContextFactory.Current.Session[UserThumbPictureBgPosSessionKey];
                return string.Empty;
            }
            set
            {
                if (HttpContextFactory.Current != null)
                    HttpContextFactory.Current.Session[UserThumbPictureBgPosSessionKey] = value;
            }
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

