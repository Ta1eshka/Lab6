using System.Collections.Generic;
using BusinessObjects;

namespace DataObjects
{
    public interface IPatientDao
    {
        List<Patient> GetAll();
        void Add(Patient patient);
    }
}
