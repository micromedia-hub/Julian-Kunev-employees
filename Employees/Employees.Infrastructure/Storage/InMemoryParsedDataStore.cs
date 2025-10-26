using Employees.Application.Storage;
using Employees.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employees.Infrastructure.Storage
{
    /// <summary>
    /// Simple thread-safe in-memory storage that keeps the most recently parsed records
    /// </summary>
    public sealed class InMemoryParsedDataStore : IParsedDataStore
    {
        private readonly object _lock = new();
        private IReadOnlyList<EmployeeProjectAssignment> _data = Array.Empty<EmployeeProjectAssignment>();

        public void SetAssignments(IReadOnlyList<EmployeeProjectAssignment> assignments)
        {
            lock (_lock)
            {
                // Create a copy to avoid sharing the list across threads
                _data = assignments.ToList().AsReadOnly();
            }
        }

        public IReadOnlyList<EmployeeProjectAssignment> GetAssignmentsOrEmpty()
        {
            lock (_lock)
            {
                return _data;
            }
        }

        public bool HasData
        {
            get
            {
                lock (_lock)
                {
                    return _data.Count > 0;
                }
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _data = Array.Empty<EmployeeProjectAssignment>();
            }
        }
    }
}
