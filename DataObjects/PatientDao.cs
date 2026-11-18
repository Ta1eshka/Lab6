using System.Collections.Generic;
using BusinessObjects;

namespace DataObjects
{
    public class PatientDao : IPatientDao
    {
        static Db db = Db.GetInstance();

        public List<Patient> GetAll()
        {
            return db.Patients;
        }

        public void Add(Patient patient)
        {
            db.Patients.Add(patient);
        }
    }
}
