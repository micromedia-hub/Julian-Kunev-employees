using Employees.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employees.Application.Parsing
{
    /// <summary>
    /// Parses a CSV file into a list of valid assignments and returns the parsed rows along with any errors.
    /// </summary>
    public interface ICsvParser
    {
        Task<(IReadOnlyList<EmployeeProjectAssignment> ValidAssignments,
              IReadOnlyList<ParsedAssignmentRow> Rows,
              IReadOnlyList<ParsedRowError> Errors)>
        ParseAsync(Stream csvStream, CancellationToken cancellationToken);
    }
}
