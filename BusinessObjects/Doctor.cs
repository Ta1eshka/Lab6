using BusinessObjects.BusinessRules;

namespace BusinessObjects
{
    public class Doctor : BusinessObject
    {
        public string FullName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public bool IsDutyDoctor { get; set; } = false;

        public Doctor()
        {
            AddRule(new RequiredRule(nameof(FullName), "ФИО врача обязательно для заполнения"));
        }

        public Doctor(string fullName, string specialization, string departmentName, bool isDutyDoctor = false) : this()
        {
            FullName = fullName;
            Specialization = specialization;
            DepartmentName = departmentName;
            IsDutyDoctor = isDutyDoctor;
        }

        public override string ToString() => $"{FullName} ({Specialization}){(IsDutyDoctor ? " [Дежурный]" : "")}";
    }
}
