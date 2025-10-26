using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employees.Domain.Models
{
    public class PairProjectWorkDetail
    {
        public int EmployeeId1 { get; init; }
        public int EmployeeId2 { get; init; }
        public int ProjectId { get; init; }
        public int DaysWorkedTogether { get; init; }
    }
}
