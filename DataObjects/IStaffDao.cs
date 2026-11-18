using System.Collections.Generic;
using BusinessObjects;

namespace DataObjects
{
    public interface IStaffDao
    {
        List<Doctor> GetDoctors();
        List<Doctor> GetDutyDoctors();
        List<Doctor> GetDoctorsByDepartment(string department);
        List<Nurse> GetNurses();
        List<Nurse> GetNursesByDepartment(string department);
    }
}
