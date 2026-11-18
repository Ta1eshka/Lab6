using System.Collections.Generic;
using BusinessObjects.BusinessRules;

namespace BusinessObjects
{
    public abstract class BusinessObject
    {
        private readonly List<BusinessRule> rules = new List<BusinessRule>();

        private readonly List<string> errors = new List<string>();

        public List<string> Errors => errors;

        protected void AddRule(BusinessRule rule)
        {
            rules.Add(rule);
        }

        public bool IsValid()
        {
            errors.Clear();
            foreach (var rule in rules)
            {
                if (!rule.Validate(this))
                {
                    errors.Add(rule.Error);
                }
            }
            return errors.Count == 0;
        }
    }
}
