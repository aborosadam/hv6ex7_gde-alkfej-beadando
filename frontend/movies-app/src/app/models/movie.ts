export interface Movie {
  id?: string;
  title: string;
  director: string;
  year: number;
  genre: string;
  description: string;
  posterUrl: string;
}

export interface MoviesPage {
  items: Movie[];
  totalCount: number;
  page: number;
  pageSize: number;
}