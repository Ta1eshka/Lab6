using System;
using System.Collections.Generic;
using BusinessObjects.BusinessRules;

namespace BusinessObjects
{
    public class MedicalRecord : BusinessObject
    {
        public string Id { get; set; } = string.Empty;
        public Patient Patient { get; set; } = new Patient();
        public string DepartmentName { get; set; } = string.Empty;
        public string DutyDoctor { get; set; } = string.Empty;       
        public string AttendingDoctor { get; set; } = string.Empty;  
        public DateTime AdmissionDate { get; set; } = DateTime.Now;
        public DateTime? DischargeDate { get; set; }
        public string AdmissionDiagnosis { get; set; } = string.Empty;
        public string FinalDiagnosis { get; set; } = string.Empty;
        public string InitialNotes { get; set; } = string.Empty;     
        public string Status { get; set; } = "На лечении";          
        public List<Prescription> Prescriptions { get; set; } = new List<Prescription>();

        public MedicalRecord()
        {
            AddRule(new RequiredRule(nameof(AdmissionDiagnosis), "Диагноз при поступлении обязателен"));
            AddRule(new RequiredRule(nameof(DepartmentName), "Отделение обязательно"));
            AddRule(new RequiredRule(nameof(DutyDoctor), "Дежурный врач обязателен"));
            AddRule(new RequiredRule(nameof(AttendingDoctor), "Лечащий врач обязателен"));
        }

        public override string ToString() => $"{Id} - {Patient.FullName} ({DepartmentName})";
    }
}
