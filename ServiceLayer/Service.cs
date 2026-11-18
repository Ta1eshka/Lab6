using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using DataObjects;

namespace ServiceLayer
{
    public class Service : IService
    {
        static readonly IDepartmentDao departmentDao = new DepartmentDao();
        static readonly IStaffDao staffDao = new StaffDao();
        static readonly IPatientDao patientDao = new PatientDao();
        static readonly IMedicalRecordDao medicalRecordDao = new MedicalRecordDao();

        #region Отделения и персонал

        public List<Department> GetDepartments()
        {
            return departmentDao.GetAll();
        }

        public List<Doctor> GetDoctors()
        {
            return staffDao.GetDoctors();
        }

        public List<Doctor> GetDutyDoctors()
        {
            return staffDao.GetDutyDoctors();
        }

        public List<Doctor> GetDoctorsByDepartment(string department)
        {
            return staffDao.GetDoctorsByDepartment(department);
        }

        public List<Nurse> GetNurses()
        {
            return staffDao.GetNurses();
        }

        public List<Nurse> GetNursesByDepartment(string department)
        {
            return staffDao.GetNursesByDepartment(department);
        }

        #endregion

        #region Пациенты

        public List<Patient> GetPatients()
        {
            return patientDao.GetAll();
        }

        public bool AddPatient(Patient patient, out string error)
        {
            error = string.Empty;
            if (patient == null)
            {
                error = "Пациент не задан";
                return false;
            }

            if (!patient.IsValid())
            {
                error = string.Join("; ", patient.Errors);
                return false;
            }

            patientDao.Add(patient);
            return true;
        }

        #endregion

        #region ЭМК и госпитализация

        public List<MedicalRecord> GetActiveRecords()
        {
            return medicalRecordDao.GetActive();
        }

        public List<MedicalRecord> GetDischargedRecords()
        {
            return medicalRecordDao.GetDischarged();
        }

        public MedicalRecord? GetRecordById(string id)
        {
            return medicalRecordDao.GetById(id);
        }

        public bool Hospitalize(
            Patient patient,
            string dutyDoctor,
            string department,
            string attendingDoctor,
            string diagnosis,
            string notes,
            out MedicalRecord? record,
            out string error)
        {
            record = null;
            error = string.Empty;

            if (patient == null)
            {
                error = "Не выбран пациент";
                return false;
            }

            var alreadyActive = medicalRecordDao.GetActive().FirstOrDefault(r => r.Patient.FullName == patient.FullName);
            if (alreadyActive != null)
            {
                error = $"Пациент {patient.FullName} уже госпитализирован в отделение {alreadyActive.DepartmentName} (карта {alreadyActive.Id})!";
                return false;
            }

            string recordId = $"ЭМК-{DateTime.Now:yy}-{new Random().Next(100, 999)}";
            var rec = new MedicalRecord
            {
                Id = recordId,
                Patient = patient,
                DepartmentName = department,
                DutyDoctor = dutyDoctor,
                AttendingDoctor = attendingDoctor,
                AdmissionDiagnosis = diagnosis,
                InitialNotes = notes,
                AdmissionDate = DateTime.Now,
                Status = "На лечении"
            };

            if (!rec.IsValid())
            {
                error = string.Join("; ", rec.Errors);
                return false;
            }

            medicalRecordDao.Add(rec);
            record = rec;
            return true;
        }

        #endregion

        #region Назначения

        public bool AddPrescription(string recordId, Prescription prescription, out string error)
        {
            error = string.Empty;
            var record = medicalRecordDao.GetById(recordId);
            if (record == null)
            {
                error = "ЭМК не найдена";
                return false;
            }

            if (!prescription.IsValid())
            {
                error = string.Join("; ", prescription.Errors);
                return false;
            }

            record.Prescriptions.Add(prescription);
            return true;
        }

        public bool ExecutePrescription(
            string recordId,
            Prescription prescription,
            string employeeName,
            string employeeRole,
            string result,
            out string error)
        {
            error = string.Empty;

            if (!prescription.CanExecute(employeeRole, out error))
            {
                return false;
            }

            prescription.IsCompleted = true;
            prescription.ExecutedBy = employeeName;
            prescription.ExecutedRole = employeeRole;
            prescription.ExecutionDate = DateTime.Now;
            prescription.ExecutionResult = string.IsNullOrWhiteSpace(result) ? "Выполнено успешно" : result;
            return true;
        }

        #endregion

        #region Выписка

        public bool DischargePatient(
            string recordId,
            string finalDiagnosis,
            string recommendations,
            out DischargeEpicrisis? epicrisis,
            out string error)
        {
            epicrisis = null;
            error = string.Empty;

            var record = medicalRecordDao.GetById(recordId);
            if (record == null)
            {
                error = "ЭМК не найдена";
                return false;
            }

            if (string.IsNullOrWhiteSpace(finalDiagnosis))
            {
                error = "Укажите заключительный диагноз для выписки";
                return false;
            }

            record.FinalDiagnosis = finalDiagnosis;
            record.DischargeDate = DateTime.Now;
            record.Status = "Выписан";

            epicrisis = new DischargeEpicrisis(record, recommendations);
            return true;
        }

        #endregion
    }
}
