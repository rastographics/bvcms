using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using UtilityExtensions;

namespace CmsWeb.Code
{
    public class DateEmptyOrValidAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            var date = (string) value;
            if (!date.HasValue())
                return true;
            DateTime dt = DateTime.MinValue;
            if (!DateTime.TryParse(date, out dt)) 
                return false;
            return dt >= DateTime.Parse("1/1/1900");
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "datenotvalid"
            };
            yield return rule;
        }
    }
}