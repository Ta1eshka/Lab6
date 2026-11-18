using BusinessObjects.BusinessRules;

namespace BusinessObjects
{
    public class Nurse : BusinessObject
    {
        public string FullName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;

        public Nurse()
        {
            AddRule(new RequiredRule(nameof(FullName), "ФИО медсестры обязательно для заполнения"));
        }

        public Nurse(string fullName, string departmentName) : this()
        {
            FullName = fullName;
            DepartmentName = departmentName;
        }

        public override string ToString() => $"{FullName} (Медсестра, {DepartmentName})";
    }
}
