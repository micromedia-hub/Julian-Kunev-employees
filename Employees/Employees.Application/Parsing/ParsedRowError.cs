using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employees.Application.Parsing
{
    public class ParsedRowError
    {
        public ParsedRowError(int lineNumber, string rawLine, string message)
        {
            LineNumber = lineNumber;
            RawLine = rawLine;
            Message = message;
        }

        public int LineNumber { get; set; }
        public string RawLine { get; set; }
        public string Message { get; set; }
    }
}
