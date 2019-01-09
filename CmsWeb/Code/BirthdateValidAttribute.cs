using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CmsData;
using UtilityExtensions;

namespace CmsWeb.Code
{
    public class BirthdateValidAttribute : ValidationAttribute
    {
        public BirthdateValidAttribute() : base("Not a valid birthdate")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && DbUtil.Db.Setting("RequiredBirthYear") && !RequiredYear(value.ToString()))
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }

        private static bool RequiredYear(string dob)
        {
            DateTime dt2 = DateTime.MinValue;
            string[] formats = { "MM/dd/yyyy", "MM/d/yyyy", "M/dd/yyyy", "M/d/yyyy" };
            if (!DateTime.TryParseExact(dob, formats, CultureInfo.CurrentCulture, DateTimeStyles.None, out dt2))
            {
                return false;
            }
            return true;
        }
    }
}
