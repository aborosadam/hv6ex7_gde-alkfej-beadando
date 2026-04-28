import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MovieService } from '../../services/movie';
import { Movie } from '../../models/movie';

@Component({
  selector: 'app-movie-list',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './movie-list.html',
  styleUrl: './movie-list.css'
})
export class MovieList implements OnInit {
  private movieService = inject(MovieService);

  movies = signal<Movie[]>([]);
  totalCount = signal(0);
  currentPage = signal(1);
  pageSize = signal(5);
  loading = signal(false);

  // Form for creating new movie
  newMovie: Movie = this.emptyMovie();
  showAddForm = signal(false);

  ngOnInit() {
    this.loadMovies();
  }

  loadMovies() {
    this.loading.set(true);
    this.movieService.getMovies(this.currentPage(), this.pageSize()).subscribe({
      next: (data) => {
        this.movies.set(data.items);
        this.totalCount.set(data.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load movies:', err);
        this.loading.set(false);
      }
    });
  }

  totalPages(): number {
    return Math.ceil(this.totalCount() / this.pageSize());
  }

  goToPage(page: number) {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadMovies();
  }

  toggleAddForm() {
    this.showAddForm.update(v => !v);
    if (!this.showAddForm()) this.newMovie = this.emptyMovie();
  }

  createMovie() {
    if (!this.newMovie.title.trim()) return;
    this.movieService.createMovie(this.newMovie).subscribe({
      next: () => {
        this.newMovie = this.emptyMovie();
        this.showAddForm.set(false);
        this.loadMovies();
      },
      error: (err) => alert('Failed to create movie: ' + err.message)
    });
  }

  deleteMovie(id: string, event: Event) {
    event.stopPropagation();
    if (!confirm('Delete this movie?')) return;
    this.movieService.deleteMovie(id).subscribe({
      next: () => this.loadMovies(),
      error: (err) => alert('Failed to delete: ' + err.message)
    });
  }

  private emptyMovie(): Movie {
    return { title: '', director: '', year: new Date().getFullYear(), genre: '', description: '', posterUrl: '' };
  }
}