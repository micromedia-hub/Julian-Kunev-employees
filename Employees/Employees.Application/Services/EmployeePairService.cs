using Employees.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employees.Application.Services
{
    /// <summary>
    /// The algorithm itself.
    /// Basically it is finding the pair of employees with the highest number of overlapping days on common projects.
    /// </summary>
    public sealed class EmployeePairService
    {
        public EmployeePairAnalysisResult Analyze(IEnumerable<EmployeeProjectAssignment> assignments)
        {
            // Group by ProjectId
            var groupByProject = assignments.GroupBy(a => a.ProjectId);
            // Merge per (Employee, Project)
            var mergedByProjectEmployee = new Dictionary<(int ProjectId, int EmployeeId), List<(DateOnly From, DateOnly To)>>();
            foreach (var projectGroup in groupByProject)
            {
                foreach (var employeeGroup in projectGroup.GroupBy(a => a.EmployeeId))
                {
                    var intervals = employeeGroup
                        .Select(a => (a.DateFrom, a.DateTo))
                        .OrderBy(x => x.DateFrom)
                        .ToList();
                    mergedByProjectEmployee[(projectGroup.Key, employeeGroup.Key)] = MergeIntervals(intervals);
                }
            }
            // Compute per-project overlaps for every pair of employees
            var totalDaysByEmployeePair = new Dictionary<(int EmployeeId1, int EmployeeId2), int>();
            var detailsByEmployeePair = new Dictionary<(int EmployeeId1, int EmployeeId2), List<PairProjectWorkDetail>>();
            var intervalsByProject = mergedByProjectEmployee
                .GroupBy(kv => kv.Key.ProjectId)
                .ToDictionary(g => g.Key, g => g.Select(x => (x.Key.EmployeeId, x.Value)).OrderBy(x => x.EmployeeId).ToList());
            foreach (var projectEntry in intervalsByProject)
            {
                int projectId = projectEntry.Key;
                var employeeIntervalsInProject = projectEntry.Value;
                for (int i = 0; i < employeeIntervalsInProject.Count; i++)
                {
                    for (int j = i + 1; j < employeeIntervalsInProject.Count; j++)
                    {
                        var (employee1Id, employee1Intervals) = employeeIntervalsInProject[i];
                        var (employee2Id, employee2Intervals) = employeeIntervalsInProject[j];
                        int daysWorkedTogether = CalculateOverlapDays(employee1Intervals, employee2Intervals);
                        if (daysWorkedTogether > 0)
                        {
                            var pairKey = NormalizePairKey(employee1Id, employee2Id);
                            if (!totalDaysByEmployeePair.ContainsKey(pairKey))
                            {
                                totalDaysByEmployeePair[pairKey] = 0;
                                detailsByEmployeePair[pairKey] = new List<PairProjectWorkDetail>();
                            }
                            totalDaysByEmployeePair[pairKey] += daysWorkedTogether;
                            detailsByEmployeePair[pairKey].Add(new PairProjectWorkDetail
                            {
                                EmployeeId1 = pairKey.EmployeeId1,
                                EmployeeId2 = pairKey.EmployeeId2,
                                ProjectId = projectId,
                                DaysWorkedTogether = daysWorkedTogether
                            });
                        }
                    }
                }
            }
            if (totalDaysByEmployeePair.Count == 0)
            {
                return new EmployeePairAnalysisResult
                {
                    EmployeeId1 = 0,
                    EmployeeId2 = 0,
                    TotalDaysWorkedTogether = 0,
                    ProjectWorkDetails = Array.Empty<PairProjectWorkDetail>()
                };
            }
            var top = totalDaysByEmployeePair
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key.EmployeeId1)
                .ThenBy(kv => kv.Key.EmployeeId2)
                .First();
            var topPair = top.Key;
            var topDetails = detailsByEmployeePair[topPair]
                .OrderBy(d => d.ProjectId)
                .ToList();
            return new EmployeePairAnalysisResult
            {
                EmployeeId1 = topPair.EmployeeId1,
                EmployeeId2 = topPair.EmployeeId2,
                TotalDaysWorkedTogether = top.Value,
                ProjectWorkDetails = topDetails
            };
        }

        /// <summary>
        /// Merges overlapping or adjacent intervals (inclusive).
        /// Examples:
        /// [1..3] and [3..5] -> [1..5];
        /// [1..3] and [2..5] -> [1..5];
        /// </summary>
        private static List<(DateOnly From, DateOnly To)> MergeIntervals(List<(DateOnly From, DateOnly To)> intervals)
        {
            var mergedIntervals = new List<(DateOnly From, DateOnly To)>();
            if (intervals.Count == 0)
            {
                return mergedIntervals;
            }
            var currentStart = intervals[0].From;
            var currentEnd = intervals[0].To;
            for (int i = 1; i < intervals.Count; i++)
            {
                var (nextStart, nextEnd) = intervals[i];
                // "Adjacent" means the next interval starts on or before (currentEnd + 1 day)
                if (nextStart <= currentEnd.AddDays(1))
                {
                    if (nextEnd > currentEnd)
                    {
                        currentEnd = nextEnd;
                    }
                }
                else
                {
                    mergedIntervals.Add((currentStart, currentEnd));
                    currentStart = nextStart;
                    currentEnd = nextEnd;
                }
            }
            mergedIntervals.Add((currentStart, currentEnd));
            return mergedIntervals;
        }

        /// <summary>
        /// Sums the inclusive overlaps between two already merged lists of intervals.
        /// </summary>
        private static int CalculateOverlapDays(List<(DateOnly From, DateOnly To)> employee1Intervals, List<(DateOnly From, DateOnly To)> employee2Intervals)
        {
            int i = 0;
            int j = 0;
            int totalDays = 0;
            while (i < employee1Intervals.Count && j < employee2Intervals.Count)
            {
                var interval1 = employee1Intervals[i];
                var interval2 = employee2Intervals[j];
                var overlapStart = interval1.From > interval2.From ? interval1.From : interval2.From;
                var overlapEnd = interval1.To < interval2.To ? interval1.To : interval2.To;
                if (overlapStart <= overlapEnd)
                {
                    // Inclusive
                    totalDays += (overlapEnd.DayNumber - overlapStart.DayNumber) + 1;
                }
                if (interval1.To < interval2.To)
                {
                    i++;
                }
                else
                {
                    j++;
                }
            }
            return totalDays;
        }

        private static (int EmployeeId1, int EmployeeId2) NormalizePairKey(int employeeId1, int employeeId2)
        {
            return employeeId1 <= employeeId2 ? (employeeId1, employeeId2) : (employeeId2, employeeId1);
        }
    }
}
