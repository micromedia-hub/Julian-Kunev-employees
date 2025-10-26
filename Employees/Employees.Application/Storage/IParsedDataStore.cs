using Employees.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employees.Application.Storage
{
    /// <summary>
    /// Temporary storage for the most recently parsed records.
    /// A simple in-memory implementation (Singleton) is used, but the interface
    /// allows easy replacement with a database, distributed cache, etc.
    /// </summary>
    public interface IParsedDataStore
    {
        void SetAssignments(IReadOnlyList<EmployeeProjectAssignment> assignments);
        IReadOnlyList<EmployeeProjectAssignment> GetAssignmentsOrEmpty();
        bool HasData { get; }
        void Clear();
    }
}
