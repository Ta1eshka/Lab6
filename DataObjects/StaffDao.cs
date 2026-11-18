using System.Collections.Generic;
using System.Linq;
using BusinessObjects;

namespace DataObjects
{
    public class StaffDao : IStaffDao
    {
        static Db db = Db.GetInstance();

        public List<Doctor> GetDoctors()
        {
            return db.Doctors;
        }

        public List<Doctor> GetDutyDoctors()
        {
            return db.Doctors.Where(d => d.IsDutyDoctor).ToList();
        }

        public List<Doctor> GetDoctorsByDepartment(string department)
        {
            return db.Doctors.Where(d => d.DepartmentName == department).ToList();
        }

        public List<Nurse> GetNurses()
        {
            return db.Nurses;
        }

        public List<Nurse> GetNursesByDepartment(string department)
        {
            return db.Nurses.Where(n => n.DepartmentName == department).ToList();
        }
    }
}
