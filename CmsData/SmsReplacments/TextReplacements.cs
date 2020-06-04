using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UtilityExtensions;

namespace CmsData
{
    public partial class TextReplacements
    {
        public CMSDataContext CurrentDatabase { get; set; }

        private readonly string[] stringlist;
        private const string MatchCodeRe = "(?<!{){(?!{)[^}]*?}";
        private readonly Dictionary<string, Func<Person, string>> codeFunctions;

        private readonly string pattern = $"({MatchCodeRe}|{MatchSpecialLinkRe}|{MatchOtherLinkRe})";

        public TextReplacements(CMSDataContext callingContext, string text)
        {
            CurrentDatabase = callingContext;
            stringlist = Regex.Split(text, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);
            codeFunctions = new Dictionary<string, Func<Person, string>>
            {
                { "{address}", PersonPrimaryAddress },
                { "{address2}", PersonPrimaryAddress2 },
                { "{birthdate}", BirthDate },
                { "{campus}", Campus },
                { "{cellphone}", CellPhone },
                { "{city}", City },
                { "{csz}", Csz },
                { "{country}", Country },
                { "{church}", Church },
                { "{churchphone}", ChurchPhone },
                { "{cmshost}", CmsHost },
                { "{dob}", Dob },
                { "{employer}", Employer },
                { "{estatement}", EStatement },
                { "{first}", First },
                { "{homephone}", HomePhone },
                { "{last}", Last },
                { "{name}", Name },
                { "{occupation}", Occupation },
                { "{peopleid}", PeopleId },
                { "{state}", State },
                { "{email}", Email },
                { "{today}", Today },
                { "{title}", Title },
            };
        }

        public string DoReplacements(SMSItem item)
        {
            var person = CurrentDatabase.LoadPersonById(item.PeopleID ?? 0);

            var texta = new List<string>(stringlist);
            for (var i = 1; i < texta.Count; i++)
            {
                var part = texta[i];
                if (part.StartsWith("{") || part.StartsWith("http"))
                    texta[i] = DoReplaceCode(part, person, item);
            }
            return string.Join("", texta);
        }
        private string DoReplaceCode(string code, Person person, SMSItem item)
        {
            if (codeFunctions.ContainsKey(code))
            {
                var func = codeFunctions[code];
                return func(person);
            }
            if (SpecialLinkRe.IsMatch(code))
            {
                return SpecialLinkReplacement(code, item);
            }
            if (OtherLinkRe.IsMatch(code))
            {
                return OtherLinkReplacement(code);
            }
            return code; // nothing matched
        }

        private string PersonPrimaryAddress(Person p) => p.PrimaryAddress;
        private string PersonPrimaryAddress2(Person p) => p.PrimaryAddress2.HasValue() ? $"<br>{p.PrimaryAddress}" : "";
        private string BirthDate(Person p) => Person.FormatBirthday(p.BirthYr, p.BirthMonth, p.BirthDay, "not available", p.PeopleId);
        private string Campus(Person p) => p.CampusId != null ? p.Campu.Description : $"No {Util2.CampusLabel} Specified";
        private string CellPhone(Person p) => p.CellPhone.HasValue() ? p.CellPhone.FmtFone() : "no cellphone on record";
        private string City(Person p) => p.PrimaryCity;
        private string Csz(Person p) => Util.FormatCSZ(p.PrimaryCity, p.PrimaryState, p.PrimaryZip);
        private string Country(Person p) => p.PrimaryCountry;
        private string Church(Person p) => CurrentDatabase.Setting("NameOfChurch", "No NameOfChurch in Settings");
        private string ChurchPhone(Person p) => CurrentDatabase.Setting("ChurchPhone", "No ChurchPhone in Settings");
        private string CmsHost(Person p) => CurrentDatabase.ServerLink();
        private string Dob(Person p) => p.DOB;
        private string Employer(Person p) => p.EmployerOther;
        private string EStatement(Person p) => p.ElectronicStatement == true ? "Online Electronic Statement Only" : "Printed Statement in Addition to Online Option";
        private string First(Person p) => p.PreferredName.Contains("?") || p.PreferredName.Contains("unknown", true) ? "" : p.PreferredName;
        private string HomePhone(Person p) => p.HomePhone.HasValue() ? p.HomePhone.FmtFone() : "no homephone on record";
        private string Last(Person p) => p.LastName;
        private string Name(Person p) => p.Name.Contains("?") || p.Name.Contains("unknown", true) ? "" : p.Name;
        private string Occupation(Person p) => p.OccupationOther;
        private string PeopleId(Person p) => p.PeopleId.ToString();
        private string State(Person p) => p.PrimaryState;
        private string Email(Person p) => p.EmailAddress;
        private string Today(Person p) => Util.Today.ToShortDateString();
        private string Title(Person p) => p.TitleCode.HasValue() ? p.TitleCode : p.ComputeTitle();
    }
}
