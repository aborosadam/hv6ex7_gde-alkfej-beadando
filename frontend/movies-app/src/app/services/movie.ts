import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Movie, MoviesPage } from '../models/movie';

@Injectable({
  providedIn: 'root'
})
export class MovieService {
  private http = inject(HttpClient);
  private apiUrl = this.buildApiUrl();

  private buildApiUrl(): string {
    const base = (window as any).API_BASE_URL;
    if (base === '' && window.location.port === '4200') {
      return 'http://localhost:5062/api/Movies'; // ng serve dev
    }
    if (base) {
      return `${base}/api/Movies`; // docker-compose
    }
    return '/api/Movies'; // K8s ingress (relative URL)
  }

  getMovies(page: number = 1, pageSize: number = 10): Observable<MoviesPage> {
    return this.http.get<MoviesPage>(`${this.apiUrl}?page=${page}&pageSize=${pageSize}`);
  }

  getMovie(id: string): Observable<Movie> {
    return this.http.get<Movie>(`${this.apiUrl}/${id}`);
  }

  createMovie(movie: Movie): Observable<Movie> {
    return this.http.post<Movie>(this.apiUrl, movie);
  }

  updateMovie(id: string, movie: Movie): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, movie);
  }

  deleteMovie(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}