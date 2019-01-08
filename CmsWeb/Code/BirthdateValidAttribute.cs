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
            if (value != null && Util.BirthDateValid(value.ToString(), out var dt))
            {
                if (dt.Year == Util.SignalNoYear && DbUtil.Db.Setting("RequiredBirthYear"))
                {
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
                }
            }
            return ValidationResult.Success;
        }
    }
}
