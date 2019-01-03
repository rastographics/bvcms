using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CmsData;

namespace CmsWeb.Code
{
    public class BirthdateValidAttribute : ValidationAttribute
    {
        public BirthdateValidAttribute() : base("Not a valid birthdate")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            string[] formats = { "MM/dd/yyyy", "MM/d/yyyy", "M/dd/yyyy", "M/d/yyyy" };

            bool requireBirthYear = DbUtil.Db.Setting("RequiredBirthYear");
            if (requireBirthYear)
            {
                bool validDate = DateTime.TryParseExact(value.ToString(), formats,
                   CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime _temp);
                if (!validDate)
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }
    }
}
