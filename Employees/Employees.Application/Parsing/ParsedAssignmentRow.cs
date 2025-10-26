using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employees.Application.Parsing
{
    public class ParsedAssignmentRow
    {
        public int? EmployeeId { get; init; }
        public int? ProjectId { get; init; }
        public string? DateFromRaw { get; init; }
        public string? DateToRaw { get; init; }
        public bool IsValid { get; init; }
        public string? Error { get; init; }
    }
}
