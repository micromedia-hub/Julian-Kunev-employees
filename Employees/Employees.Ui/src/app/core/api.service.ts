import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

export interface ParsedAssignmentRow {
  employeeId?: number;
  projectId?: number;
  dateFromRaw?: string;
  dateToRaw?: string;
  isValid: boolean;
  error?: string;
}

export interface AnalyzeDetail {
  employeeId1: number;
  employeeId2: number;
  projectId: number;
  daysWorkedTogether: number;
}

export interface AnalyzeResponse {
  employeeId1: number;
  employeeId2: number;
  totalDaysWorkedTogether: number;
  details: AnalyzeDetail[];
}

@Injectable({ providedIn: 'root' })
export class ApiService {
  constructor(private http: HttpClient) { }

  parseCsv(file: File) {
    const form = new FormData();
    form.append('file', file, file.name);

    return this.http.post<{
      totalValid: number;
      totalErrors: number;
      echo: ParsedAssignmentRow[];
    }>('/api/files/parse', form);
  }

  analyze() {
    return this.http.get<AnalyzeResponse>('/api/analyze');
  }
}
