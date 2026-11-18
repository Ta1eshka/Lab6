namespace BusinessObjects.BusinessRules
{
    public class RequiredRule : BusinessRule
    {
        public RequiredRule(string property, string error) : base(property, error)
        {
        }

        public override bool Validate(BusinessObject businessObject)
        {
            var value = GetPropertyValue(businessObject);
            if (value == null) return false;
            if (value is string str) return !string.IsNullOrWhiteSpace(str);
            return true;
        }
    }
}
