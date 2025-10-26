using Employees.Application.Parsing;
using Employees.Domain.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employees.Infrastructure.Parsing
{
    /// <summary>
    /// Parses a CSV file without a header (or with a header that is automatically skipped).
    /// Supports multiple date formats (see DateFormats) and interprets "NULL" as today.
    /// Returns valid assignments together with the parsed rows and a list of errors (if any).
    /// </summary>
    public sealed class CsvParser : ICsvParser
    {
        private static readonly string[] DateFormats = new[]
        {
            "yyyy-MM-dd", "dd.MM.yyyy", "MM/dd/yyyy", "dd/MM/yyyy", "yyyy/MM/dd"
        };

        public async Task<(IReadOnlyList<EmployeeProjectAssignment> ValidAssignments,
                           IReadOnlyList<ParsedAssignmentRow> Rows,
                           IReadOnlyList<ParsedRowError> Errors)>
        ParseAsync(Stream csvStream, CancellationToken cancellationToken)
        {
            var valid = new List<EmployeeProjectAssignment>();
            var echo = new List<ParsedAssignmentRow>();
            var errors = new List<ParsedRowError>();
            using var reader = new StreamReader(csvStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
            int lineNumber = 0;
            while (!reader.EndOfStream)
            {
                cancellationToken.ThrowIfCancellationRequested();
                string? rawLine = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(rawLine))
                {
                    continue;
                }
                lineNumber++;
                var columns = rawLine.Split(',', StringSplitOptions.TrimEntries);
                // Expected at least 4 columns: EmployeeID, ProjectID, DateFrom, DateTo
                if (columns.Length < 4)
                {
                    errors.Add(new ParsedRowError(lineNumber, rawLine, "Not enough columns"));
                    echo.Add(new ParsedAssignmentRow
                    {
                        EmployeeId = null,
                        ProjectId = null,
                        DateFromRaw = columns.ElementAtOrDefault(2),
                        DateToRaw = columns.ElementAtOrDefault(3),
                        IsValid = false,
                        Error = "Not enough columns"
                    });
                    continue;
                }
                // Attempts to parse EmployeeId and ProjectId
                if (!int.TryParse(columns[0], out int employeeId) || !int.TryParse(columns[1], out int projectId))
                {
                    // POssibly header – skip if this is the first line.
                    if (lineNumber == 1)
                    {
                        continue;
                    }
                    errors.Add(new ParsedRowError(lineNumber, rawLine, "Invalid EmployeeId or ProjectId"));
                    echo.Add(new ParsedAssignmentRow
                    {
                        EmployeeId = null,
                        ProjectId = null,
                        DateFromRaw = columns[2],
                        DateToRaw = columns[3],
                        IsValid = false,
                        Error = "Invalid EmployeeId or ProjectId"
                    });
                    continue;
                }
                string dateFromRaw = columns[2];
                string dateToRaw = columns[3];
                DateOnly? dateFrom = TryParseDateOnly(dateFromRaw);
                DateOnly? dateTo = TryParseDateOnly(dateToRaw);
                bool isValid = dateFrom.HasValue && dateTo.HasValue && dateFrom.Value <= dateTo.Value;
                echo.Add(new ParsedAssignmentRow
                {
                    EmployeeId = employeeId,
                    ProjectId = projectId,
                    DateFromRaw = dateFromRaw,
                    DateToRaw = dateToRaw,
                    IsValid = isValid,
                    Error = isValid ? null : "Invalid or missing date"
                });
                if (!isValid)
                {
                    errors.Add(new ParsedRowError(lineNumber, rawLine, "Invalid or missing date"));
                    continue;
                }
                valid.Add(new EmployeeProjectAssignment(employeeId, projectId, dateFrom!.Value, dateTo!.Value));
            }
            return (valid, echo, errors);
        }

        /// <summary>
        /// Parses a value into a DateOnly.
        /// "NULL" (case-insensitive) means today's date.
        /// Supports multiple formats defined in DateFormats.
        /// </summary>
        private static DateOnly? TryParseDateOnly(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
            {
                return null;
            }
            raw = raw.Trim();
            if (raw.Equals("NULL", StringComparison.OrdinalIgnoreCase))
            {
                var today = DateTime.Today;
                return DateOnly.FromDateTime(today);
            }
            // Try provided formats
            if (DateTime.TryParseExact(raw, DateFormats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out var dtExact))
            {
                return DateOnly.FromDateTime(dtExact);
            }
            // Default
            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
            {
                return DateOnly.FromDateTime(dt);
            }
            return null;
        }
    }
}
