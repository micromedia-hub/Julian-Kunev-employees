using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employees.Domain.Models
{
    public class EmployeePairAnalysisResult
    {
        public int EmployeeId1 { get; init; }
        public int EmployeeId2 { get; init; }
        public int TotalDaysWorkedTogether { get; init; }
        public IReadOnlyList<PairProjectWorkDetail> ProjectWorkDetails { get; init; } = Array.Empty<PairProjectWorkDetail>();
    }
}
