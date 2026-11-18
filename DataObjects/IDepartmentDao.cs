using System.Collections.Generic;
using BusinessObjects;

namespace DataObjects
{
    public interface IDepartmentDao
    {
        List<Department> GetAll();
    }
}
