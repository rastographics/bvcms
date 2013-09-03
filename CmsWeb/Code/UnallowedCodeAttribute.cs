using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Web.Mvc;

namespace CmsWeb.Code
{
    public class UnallowedCodeAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string unallowed;
        public UnallowedCodeAttribute(string code)
        {
            unallowed = code;
        }
        public override bool IsValid(object value)
        {
            var code = value as CodeInfo;
            Debug.Assert(code != null, "code != null");
            if (code.Value == unallowed)
                return false;
            return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "unallowedcode"
            };
            rule.ValidationParameters["code"] = unallowed;
            yield return rule;
        }
    }
}