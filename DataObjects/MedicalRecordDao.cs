using System.Collections.Generic;
using System.Linq;
using BusinessObjects;

namespace DataObjects
{
    public class MedicalRecordDao : IMedicalRecordDao
    {
        static Db db = Db.GetInstance();

        public List<MedicalRecord> GetAll()
        {
            return db.MedicalRecords;
        }

        public List<MedicalRecord> GetActive()
        {
            return db.MedicalRecords.Where(r => r.Status == "На лечении").ToList();
        }

        public List<MedicalRecord> GetDischarged()
        {
            return db.MedicalRecords.Where(r => r.Status == "Выписан").ToList();
        }

        public MedicalRecord? GetById(string id)
        {
            return db.MedicalRecords.FirstOrDefault(r => r.Id == id);
        }

        public void Add(MedicalRecord record)
        {
            db.MedicalRecords.Add(record);
        }
    }
}
