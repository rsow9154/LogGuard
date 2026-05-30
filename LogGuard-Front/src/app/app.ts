import { Component, NgZone, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule, HttpHeaders } from '@angular/common/http';
import { timeout, catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';
import { marked } from 'marked';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './app.html',
  styleUrls: []
})
export class AppComponent {
  title = 'LogGuard-Front';
  logsInput: string = '';
  estEnCours: boolean = false;
  rapportAnalyse: string = '';
  rapportHtml: string = '';
  private apiUrl = 'http://localhost:5027/api/logevaluation/process';

  constructor(private http: HttpClient, private zone: NgZone, private cdr: ChangeDetectorRef) {}

  analyserLogs(): void {
    if (!this.logsInput.trim()) {
      this.rapportAnalyse = "Veuillez coller des lignes de logs.";
      return;
    }
    this.estEnCours = true;
    this.rapportAnalyse = '';
    this.rapportHtml = '';
    this.cdr.detectChanges();

    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const body = { logs: this.logsInput };

    this.http.post(this.apiUrl, body, { headers: headers, responseType: 'text' })
      .pipe(
        timeout(600000),
        catchError(err => {
          if (err.name === 'TimeoutError') {
            return throwError(() => ({ message: 'Timeout 10 min depasse.' }));
          }
          return throwError(() => err);
        })
      )
      .subscribe({
        next: (response) => {
          this.zone.run(() => {
            this.rapportAnalyse = response || 'Reponse vide';
            this.rapportHtml = marked(this.rapportAnalyse) as string;
            this.estEnCours = false;
            this.cdr.detectChanges();
          });
        },
        error: (err) => {
          this.zone.run(() => {
            this.rapportAnalyse = err.message ?? "Erreur serveur.";
            this.rapportHtml = this.rapportAnalyse;
            this.estEnCours = false;
            this.cdr.detectChanges();
          });
        }
      });
  }
}
