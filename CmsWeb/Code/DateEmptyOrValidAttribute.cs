using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Web.Mvc;
using RestSharp.Extensions;

namespace CmsWeb.Code
{
    public class DateEmptyOrValidAttribute : ValidationAttribute, IClientValidatable
    {
        public override bool IsValid(object value)
        {
            var date = value as string;
            DateTime dt = DateTime.MinValue;
            if (date.HasValue() && !DateTime.TryParse(date, out dt)) 
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