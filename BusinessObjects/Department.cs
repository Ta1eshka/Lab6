using BusinessObjects.BusinessRules;

namespace BusinessObjects
{
    public class Department : BusinessObject
    {
        public string Name { get; set; } = string.Empty;

        public Department()
        {
            AddRule(new RequiredRule(nameof(Name), "Название отделения не может быть пустым"));
        }

        public Department(string name) : this()
        {
            Name = name;
        }

        public override string ToString() => Name;
    }
}
