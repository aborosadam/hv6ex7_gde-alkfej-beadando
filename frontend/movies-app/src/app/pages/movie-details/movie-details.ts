import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { MovieService } from '../../services/movie';
import { ReviewService } from '../../services/review';
import { Movie } from '../../models/movie';
import { Review } from '../../models/review';

@Component({
  selector: 'app-movie-details',
  imports: [CommonModule, RouterLink, FormsModule],
  templateUrl: './movie-details.html',
  styleUrl: './movie-details.css'
})
export class MovieDetails implements OnInit {
  private route = inject(ActivatedRoute);
  private movieService = inject(MovieService);
  private reviewService = inject(ReviewService);

  movie = signal<Movie | null>(null);
  reviews = signal<Review[]>([]);
  loading = signal(true);
  movieId = '';

  // Review form
  newReview: Review = this.emptyReview();

  ngOnInit() {
    this.movieId = this.route.snapshot.paramMap.get('id') || '';
    if (this.movieId) {
      this.loadMovie();
      this.loadReviews();
    }
  }

  loadMovie() {
    this.movieService.getMovie(this.movieId).subscribe({
      next: (m) => {
        this.movie.set(m);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  loadReviews() {
    this.reviewService.getReviewsByMovie(this.movieId).subscribe({
      next: (r) => this.reviews.set(r)
    });
  }

  submitReview() {
    if (!this.newReview.userName.trim() || !this.newReview.comment.trim()) return;
    this.newReview.movieId = this.movieId;
    this.reviewService.createReview(this.newReview).subscribe({
      next: () => {
        this.newReview = this.emptyReview();
        this.loadReviews();
      },
      error: (err) => alert('Failed to add review: ' + err.message)
    });
  }

  deleteReview(id: string) {
    if (!confirm('Delete this review?')) return;
    this.reviewService.deleteReview(id).subscribe({
      next: () => this.loadReviews()
    });
  }

  averageRating(): string {
    const r = this.reviews();
    if (r.length === 0) return 'No ratings yet';
    const avg = r.reduce((sum, x) => sum + x.rating, 0) / r.length;
    return `${avg.toFixed(1)} / 5 (${r.length} review${r.length === 1 ? '' : 's'})`;
  }

  private emptyReview(): Review {
    return { movieId: '', userName: '', rating: 5, comment: '' };
  }
}