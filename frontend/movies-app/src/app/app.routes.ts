import { Routes } from '@angular/router';
import { MovieList } from './pages/movie-list/movie-list';
import { MovieDetails } from './pages/movie-details/movie-details';

export const routes: Routes = [
  { path: '', redirectTo: '/movies', pathMatch: 'full' },
  { path: 'movies', component: MovieList },
  { path: 'movies/:id', component: MovieDetails }
];