using System;
using System.Text;

namespace BusinessObjects
{
    public class DischargeEpicrisis : BusinessObject
    {
        public MedicalRecord Record { get; set; }
        public string Recommendations { get; set; } = string.Empty;

        public DischargeEpicrisis(MedicalRecord record, string recommendations = "")
        {
            Record = record;
            Recommendations = recommendations;
        }

        public string GetText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("========================================================================");
            sb.AppendLine("                     ГОСУДАРСТВЕННАЯ БОЛЬНИЦА");
            sb.AppendLine("                         ВЫПИСНОЙ ЭПИКРИЗ");
            sb.AppendLine($"                     Медицинская карта № {Record.Id}");
            sb.AppendLine("========================================================================");
            sb.AppendLine();
            sb.AppendLine("1. ПЕРСОНАЛЬНЫЕ ДАННЫЕ ПАЦИЕНТА:");
            sb.AppendLine($"   ФИО:                  {Record.Patient.FullName}");
            sb.AppendLine($"   Возраст и пол:        {Record.Patient.Age} лет, {Record.Patient.Gender}");
            sb.AppendLine($"   Паспортные данные:    {Record.Patient.Passport}");
            sb.AppendLine($"   Телефон:              {Record.Patient.Phone}");
            sb.AppendLine();
            sb.AppendLine("2. ПЕРИОД ПРЕБЫВАНИЯ В СТАЦИОНАРЕ:");
            sb.AppendLine($"   Отделение:            {Record.DepartmentName}");
            sb.AppendLine($"   Дата поступления:     {Record.AdmissionDate:dd.MM.yyyy}");
            sb.AppendLine($"   Дата выписки:         {(Record.DischargeDate.HasValue ? Record.DischargeDate.Value.ToString("dd.MM.yyyy") : DateTime.Now.ToString("dd.MM.yyyy"))}");
            sb.AppendLine($"   Дежурный врач:        {Record.DutyDoctor}");
            sb.AppendLine($"   Лечащий врач:         {Record.AttendingDoctor}");
            sb.AppendLine();
            sb.AppendLine("3. ДИАГНОЗ:");
            sb.AppendLine($"   Диагноз при поступлении: {Record.AdmissionDiagnosis}");
            sb.AppendLine($"   Заключительный диагноз:  {Record.FinalDiagnosis}");
            sb.AppendLine();
            sb.AppendLine("4. ПЕРЕЧЕНЬ ВЫПОЛНЕННЫХ НАЗНАЧЕНИЙ ВРАЧА:");

            int count = 0;
            foreach (var p in Record.Prescriptions)
            {
                if (p.IsCompleted)
                {
                    count++;
                    sb.AppendLine($"   {count}. [{p.Type}] {p.Name} — {p.Details}");
                    sb.AppendLine($"      Выполнил: {p.ExecutedBy} ({p.ExecutedRole}) | Дата: {p.ExecutionDate:dd.MM.yyyy HH:mm}");
                    if (!string.IsNullOrWhiteSpace(p.ExecutionResult))
                    {
                        sb.AppendLine($"      Результат: {p.ExecutionResult}");
                    }
                }
            }

            if (count == 0)
            {
                sb.AppendLine("   (Выполненных назначений нет)");
            }

            sb.AppendLine();
            sb.AppendLine("5. РЕКОМЕНДАЦИИ ПРИ ВЫПИСКЕ:");
            sb.AppendLine($"   {(string.IsNullOrWhiteSpace(Recommendations) ? "Наблюдение у участкового терапевта по месту жительства." : Recommendations)}");
            sb.AppendLine();
            sb.AppendLine("========================================================================");
            return sb.ToString();
        }
    }
}
