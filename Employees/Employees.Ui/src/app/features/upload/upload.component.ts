import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ApiService, ParsedAssignmentRow, AnalyzeResponse } from '../../core/api.service';

@Component({
  selector: 'app-upload',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './upload.component.html',
  styleUrls: ['./upload.component.css']
})
export class UploadComponent {
  /** Избраният от потребителя CSV файл. */
  file?: File;

  /** Echo редове от парсването (диагностика). */
  echo: ParsedAssignmentRow[] = [];

  /** Флаг за визуализация на зареждане. */
  loading = false;

  /** Съобщение за грешка за показване в UI. */
  errorMessage = '';

  /** Резултат от анализа – за бонус таблицата. */
  analyzeResult?: AnalyzeResponse;

  constructor(private api: ApiService) { }

  onFileChange(ev: Event) {
    const input = ev.target as HTMLInputElement;
    if (input.files && input.files.length) {
      this.file = input.files[0];
    }
  }

  /** Качва и парсва CSV. Записът отива и в in-memory store на сървъра. */
  onParse() {
    if (!this.file) { this.errorMessage = 'Please select a CSV file.'; return; }

    this.loading = true;
    this.errorMessage = '';
    this.echo = [];
    this.analyzeResult = undefined;

    this.api.parseCsv(this.file).subscribe({
      next: result => {
        this.echo = result.echo ?? [];
        this.loading = false;
      },
      error: error => {
        this.errorMessage = (error?.error || 'Upload failed');
        this.loading = false;
      }
    });
  }

  /** Стартира анализа върху вече парснатите данни на сървъра. */
  onAnalyze() {
    this.loading = true;
    this.errorMessage = '';
    this.analyzeResult = undefined;

    this.api.analyze().subscribe({
      next: result => {
        this.analyzeResult = result;
        this.loading = false;
      },
      error: error => {
        this.errorMessage = (error?.error || 'Analyze failed');
        this.loading = false;
      }
    });
  }
}
