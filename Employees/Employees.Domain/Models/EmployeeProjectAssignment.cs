using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employees.Domain.Models
{
    /// <summary>
    /// Represents a single employee participation in a project with a start and end day.
    /// Uses DateOnly because the task is day-oriented (no time component).
    /// </summary>
    public sealed class EmployeeProjectAssignment
    {
        public EmployeeProjectAssignment(int employeeId, int projectId, DateOnly dateFrom, DateOnly dateTo)
        {
            EmployeeId = employeeId;
            ProjectId = projectId;
            DateFrom = dateFrom;
            DateTo = dateTo;
        }

        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
        public DateOnly DateFrom { get; set; }
        public DateOnly DateTo { get; set; }
    }
}
