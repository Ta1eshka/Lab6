namespace BusinessObjects.BusinessRules
{
    public abstract class BusinessRule
    {
        public string Property { get; set; }
        public string Error { get; set; }

        public BusinessRule(string property)
        {
            Property = property;
            Error = $"{property} указано неверно";
        }

        public BusinessRule(string property, string error)
        {
            Property = property;
            Error = error;
        }

        public abstract bool Validate(BusinessObject businessObject);

        protected object? GetPropertyValue(BusinessObject businessObject)
        {
            return businessObject.GetType().GetProperty(Property)?.GetValue(businessObject, null);
        }
    }
}
