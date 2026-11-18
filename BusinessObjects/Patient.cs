using BusinessObjects.BusinessRules;

namespace BusinessObjects
{
    public class Patient : BusinessObject
    {
        public string FullName { get; set; } = string.Empty;
        public int Age { get; set; } = 30;
        public string Gender { get; set; } = "Мужской";
        public string Passport { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public Patient()
        {
            AddRule(new RequiredRule(nameof(FullName), "ФИО пациента обязательно"));
            AddRule(new RequiredRule(nameof(Passport), "Паспортные данные обязательны"));
        }

        public Patient(string fullName, int age, string gender, string passport, string phone) : this()
        {
            FullName = fullName;
            Age = age;
            Gender = gender;
            Passport = passport;
            Phone = phone;
        }

        public override string ToString() => $"{FullName} ({Age} лет, {Gender})";
    }
}
