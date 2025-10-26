Employees Pairs:


A small demo project that finds the pair of employees who have worked together on common projects for the longest total time.

The solution is split into two parts:
Employees.Api – ASP.NET Core Web API (.NET 9)
Employees.Ui – Angular front-end 


Project structure:


1. Employees.Api - ASP.NET Core Web API (.NET 9)
   1.1. Controllers
   1.1.1. FilesController - Handles CSV upload and parsing
   1.1.2. AnalyzeController - Runs the analysis on parsed data
2. Employees.Application
   2.1. Parsing
   2.1.1. ICsvParser - Parses a CSV file
   2.1.2. ParsedAssignmentRow
   2.1.3. ParsedRowError
   2.2. Services
   2.2.1. EmployeePairService - The algorithm itself
   2.3. Storage
   2.3.1. IParsedDataStore - Temporary storage for the most recently parsed records
3. Employees.Domain
   3.1. Models
   3.1.1. EmployeePairAnalysisResult
   3.1.2. EmployeeProjectAssignment
   3.1.3. PairProjectWorkDetail
4. Employees.Infrastructure
   4.1. Parsing
   4.1.1. CsvParser - Implements ICsvParser
   4.2. Storage
   4.2.1. InMemoryParsedDataStore - Implements IParsedDataStore
5. Employees.Ui - Angular application
   5.1. upload
   5.1.1. upload.component
6. Sample data
   6.1. sample.csv
    

Running the application:


Start the backend (Startup project: Employees.Api, F5). Swagger is available.
Start the frontend (Open a terminal in Employees.Ui, npm install, npm start)
The app will open automatically at http://localhost:60698


How it works:


1. Upload
Choose the CSV file (e.g. the provided sample.csv)
2. Parse
Click Parse to load and validate the file. The parsed rows are displayed in a table.
3. Analyze
After parsing, click Analyze.
The server processes the file. The result is shown above the table
4. Example output:
Top Pair: 101 | 102
Total days together: 51



