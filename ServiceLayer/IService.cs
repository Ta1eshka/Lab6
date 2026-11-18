using System.Collections.Generic;
using BusinessObjects;

namespace ServiceLayer
{
    public interface IService
    {
        List<Department> GetDepartments();

        List<Doctor> GetDoctors();
        List<Doctor> GetDutyDoctors();
        List<Doctor> GetDoctorsByDepartment(string department);
        List<Nurse> GetNurses();
        List<Nurse> GetNursesByDepartment(string department);

        List<Patient> GetPatients();
        bool AddPatient(Patient patient, out string error);

        List<MedicalRecord> GetActiveRecords();
        List<MedicalRecord> GetDischargedRecords();
        MedicalRecord? GetRecordById(string id);

        bool Hospitalize(
            Patient patient,
            string dutyDoctor,
            string department,
            string attendingDoctor,
            string diagnosis,
            string notes,
            out MedicalRecord? record,
            out string error);

        bool AddPrescription(string recordId, Prescription prescription, out string error);

        bool ExecutePrescription(
            string recordId,
            Prescription prescription,
            string employeeName,
            string employeeRole,
            string result,
            out string error);

        bool DischargePatient(
            string recordId,
            string finalDiagnosis,
            string recommendations,
            out DischargeEpicrisis? epicrisis,
            out string error);
    }
}
