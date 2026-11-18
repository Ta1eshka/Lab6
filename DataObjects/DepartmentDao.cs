using System.Collections.Generic;
using BusinessObjects;

namespace DataObjects
{
    public class DepartmentDao : IDepartmentDao
    {
        static Db db = Db.GetInstance();

        public List<Department> GetAll()
        {
            return db.Departments;
        }
    }
}
