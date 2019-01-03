using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CmsData;

namespace CmsWeb.Code
{
    public class BirthdateValidAttribute : ValidationAttribute
    {
        private DateTime _dt;
        private bool _required = false;
        
        public BirthdateValidAttribute() : base("Not valid birthdate")
        {                        
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            _required = DbUtil.Db.Setting("RequiredBirthYear");

            if (_required && value == null)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }            

            string[] formats = { "MM/dd/yyyy", "MM/d/yyyy","M/dd/yyyy","M/d/yyyy"};

            bool validDate = DateTime.TryParseExact(value.ToString(), formats,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out _dt);

            if (_required && !validDate)
            {
                return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }
            return ValidationResult.Success;
        }
    }
}
