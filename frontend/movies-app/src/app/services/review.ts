import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Review } from '../models/review';

@Injectable({
  providedIn: 'root'
})
export class ReviewService {
  private http = inject(HttpClient);
  private apiUrl = this.buildApiUrl();

  private buildApiUrl(): string {
    const base = (window as any).API_BASE_URL;
    if (base === '' && window.location.port === '4200') {
      return 'http://localhost:5062/api/Reviews';
    }
    if (base) {
      return `${base}/api/Reviews`;
    }
    return '/api/Reviews';
  }

  getReviewsByMovie(movieId: string): Observable<Review[]> {
    return this.http.get<Review[]>(`${this.apiUrl}?movieId=${movieId}`);
  }

  createReview(review: Review): Observable<Review> {
    return this.http.post<Review>(this.apiUrl, review);
  }

  deleteReview(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}