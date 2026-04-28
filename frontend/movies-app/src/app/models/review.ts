export interface Review {
  id?: string;
  movieId: string;
  userName: string;
  rating: number;
  comment: string;
  createdAt?: string;
}