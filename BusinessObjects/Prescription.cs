using System;
using BusinessObjects.BusinessRules;

namespace BusinessObjects
{
    public class Prescription : BusinessObject
    {
        public string Type { get; set; } = "Медикаментозное (таблетки)"; 
        public string Name { get; set; } = string.Empty;              
        public string Details { get; set; } = string.Empty;            
        public string PrescribedBy { get; set; } = string.Empty;     
        public DateTime PrescribedDate { get; set; } = DateTime.Now;

        public bool IsCompleted { get; set; } = false;
        public string ExecutedBy { get; set; } = string.Empty;         
        public string ExecutedRole { get; set; } = string.Empty;       
        public DateTime? ExecutionDate { get; set; }
        public string ExecutionResult { get; set; } = string.Empty;    

        public Prescription()
        {
            AddRule(new RequiredRule(nameof(Name), "Название назначения обязательно"));
            AddRule(new RequiredRule(nameof(Details), "Параметры назначения обязательны"));
        }

        public bool CanExecute(string employeeRole, out string errorMessage)
        {
            if (Type.StartsWith("Медикаментозное"))
            {
                if (employeeRole != "Медсестра")
                {
                    errorMessage = "Медикаментозные назначения может осуществлять только медсестра!";
                    return false;
                }
            }
            else
            {
                if (employeeRole != "Врач")
                {
                    errorMessage = "Диагностические и профилактические назначения осуществляют только врачи!";
                    return false;
                }
            }

            errorMessage = string.Empty;
            return true;
        }

        public override string ToString() => $"{Name} ({Type}) - {(IsCompleted ? "Выполнено" : "Ожидает")}";
    }
}
