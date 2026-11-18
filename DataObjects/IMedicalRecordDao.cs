using System.Collections.Generic;
using BusinessObjects;

namespace DataObjects
{
    public interface IMedicalRecordDao
    {
        List<MedicalRecord> GetAll();
        List<MedicalRecord> GetActive();
        List<MedicalRecord> GetDischarged();
        MedicalRecord? GetById(string id);
        void Add(MedicalRecord record);
    }
}
