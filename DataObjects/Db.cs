using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using BusinessObjects;

namespace DataObjects
{
    public class Db
    {
        private static Db? _instance;
        private static readonly object _lock = new object();

        public List<Department> Departments { get; set; } = new List<Department>();
        public List<Doctor> Doctors { get; set; } = new List<Doctor>();
        public List<Nurse> Nurses { get; set; } = new List<Nurse>();
        public List<Patient> Patients { get; set; } = new List<Patient>();
        public List<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

        private Db()
        {
            SeedData();
        }

        public static Db GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Db();
                    }
                }
            }
            return _instance;
        }

        private void SeedData()
        {
            Departments.Add(new Department("Кардиологическое"));
            Departments.Add(new Department("Хирургическое"));
            Departments.Add(new Department("Неврологическое"));

            Doctors.Add(new Doctor("Иванов И.И.", "Терапевт", "Приемное отделение", isDutyDoctor: true));
            Doctors.Add(new Doctor("Петров П.С.", "Кардиолог", "Кардиологическое", isDutyDoctor: true));
            Doctors.Add(new Doctor("Сидоров А.М.", "Хирург", "Хирургическое", isDutyDoctor: false));
            Doctors.Add(new Doctor("Смирнова Е.В.", "Невролог", "Неврологическое", isDutyDoctor: true));

            Nurses.Add(new Nurse("Морозова А.Н.", "Кардиологическое"));
            Nurses.Add(new Nurse("Орлова Е.А.", "Хирургическое"));
            Nurses.Add(new Nurse("Федорова М.С.", "Неврологическое"));

            var p1 = new Patient("Кузнецов В.С.", 56, "Мужской", "4512 890123", "+7 (903) 123-45-67");
            var p2 = new Patient("Соколова Н.П.", 42, "Женский", "4508 678432", "+7 (905) 234-56-78");
            var p3 = new Patient("Михайлов Д.А.", 29, "Мужской", "4518 901234", "+7 (910) 345-67-89");
            Patients.Add(p1);
            Patients.Add(p2);
            Patients.Add(p3);

            var rec1 = new MedicalRecord
            {
                Id = "ЭМК-101",
                Patient = p1,
                DepartmentName = "Кардиологическое",
                DutyDoctor = "Иванов И.И.",
                AttendingDoctor = "Петров П.С.",
                AdmissionDiagnosis = "Ишемическая болезнь сердца, стенокардия",
                InitialNotes = "Жалобы на давящие боли за грудиной. Состояние средней тяжести.",
                AdmissionDate = DateTime.Now.AddDays(-3),
                Status = "На лечении"
            };

            var pr1 = new Prescription
            {
                Type = "Медикаментозное (таблетки)",
                Name = "Аспирин Кардио",
                Details = "100 мг, 1 раз в день, 14 дней",
                PrescribedBy = "Петров П.С.",
                IsCompleted = true,
                ExecutedBy = "Морозова А.Н.",
                ExecutedRole = "Медсестра",
                ExecutionDate = DateTime.Now.AddDays(-2),
                ExecutionResult = "Таблетка выдана пациенту"
            };

            var pr2 = new Prescription
            {
                Type = "Медикаментозное (инъекции)",
                Name = "Фраксипарин",
                Details = "5 инъекций, 0.4 мл п/к",
                PrescribedBy = "Петров П.С.",
                IsCompleted = false
            };

            var pr3 = new Prescription
            {
                Type = "Диагностическое",
                Name = "Анализ крови биохимический",
                Details = "Тропонин I, липидный профиль",
                PrescribedBy = "Петров П.С.",
                IsCompleted = true,
                ExecutedBy = "Петров П.С.",
                ExecutedRole = "Врач",
                ExecutionDate = DateTime.Now.AddDays(-3),
                ExecutionResult = "Показатели в норме"
            };

            var pr4 = new Prescription
            {
                Type = "Профилактическое",
                Name = "Лечебная физкультура (ЛФК)",
                Details = "7 сеансов по 20 минут",
                PrescribedBy = "Петров П.С.",
                IsCompleted = false
            };

            rec1.Prescriptions.Add(pr1);
            rec1.Prescriptions.Add(pr2);
            rec1.Prescriptions.Add(pr3);
            rec1.Prescriptions.Add(pr4);
            MedicalRecords.Add(rec1);

            var rec2 = new MedicalRecord
            {
                Id = "ЭМК-099",
                Patient = p2,
                DepartmentName = "Хирургическое",
                DutyDoctor = "Иванов И.И.",
                AttendingDoctor = "Сидоров А.М.",
                AdmissionDiagnosis = "Острый аппендицит",
                FinalDiagnosis = "Острый катаральный аппендицит. Аппендэктомия.",
                InitialNotes = "Острые боли в правой подвздошной области.",
                AdmissionDate = DateTime.Now.AddDays(-10),
                DischargeDate = DateTime.Now.AddDays(-2),
                Status = "Выписан"
            };
            MedicalRecords.Add(rec2);
        }
    }
}
