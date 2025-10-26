using Employees.Application.Services;
using Employees.Application.Storage;
using Microsoft.AspNetCore.Mvc;

namespace Employees.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class AnalyzeController : ControllerBase
    {
        private readonly IParsedDataStore _parsedDataStore;
        private readonly EmployeePairService _employeePairService;

        public AnalyzeController(IParsedDataStore parsedDataStore, EmployeePairService employeePairService)
        {
            _parsedDataStore = parsedDataStore;
            _employeePairService = employeePairService;
        }

        /// <summary>
        /// Uses the most recently parsed records in the storage and returns the top pair along with the details.
        /// </summary>
        [HttpGet]
        public IActionResult Analyze()
        {
            if (!_parsedDataStore.HasData)
            {
                return BadRequest("No parsed data available. Please upload and parse a CSV file first.");
            }
            var assignments = _parsedDataStore.GetAssignmentsOrEmpty();
            var result = _employeePairService.Analyze(assignments);
            return Ok(new
            {
                employeeId1 = result.EmployeeId1,
                employeeId2 = result.EmployeeId2,
                totalDaysWorkedTogether = result.TotalDaysWorkedTogether,
                details = result.ProjectWorkDetails.Select(d => new
                {
                    employeeId1 = d.EmployeeId1,
                    employeeId2 = d.EmployeeId2,
                    projectId = d.ProjectId,
                    daysWorkedTogether = d.DaysWorkedTogether
                })
            });
        }
    }
}
