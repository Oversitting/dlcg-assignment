import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  PagedResponse,
  SaveVideoGameRequest,
  VideoGame,
  VideoGameBrowseFilters,
} from '../models/video-game';

@Injectable({ providedIn: 'root' })
export class VideoGameApiService {
  private readonly httpClient = inject(HttpClient);
  private readonly baseUrl = '/api/video-games';

  getVideoGames(filters: VideoGameBrowseFilters, pageNumber: number, pageSize: number): Observable<PagedResponse<VideoGame>> {
    let params = new HttpParams();

    if (filters.searchTerm.trim()) {
      params = params.set('searchTerm', filters.searchTerm.trim());
    }

    if (filters.genre.trim()) {
      params = params.set('genre', filters.genre.trim());
    }

    if (filters.platform.trim()) {
      params = params.set('platform', filters.platform.trim());
    }

    params = params
      .set('orderBy', filters.orderBy)
      .set('orderDirection', filters.orderDirection);

    params = params.set('pageNumber', pageNumber).set('pageSize', pageSize);

    return this.httpClient.get<PagedResponse<VideoGame>>(this.baseUrl, { params });
  }

  getVideoGame(id: number): Observable<VideoGame> {
    return this.httpClient.get<VideoGame>(`${this.baseUrl}/${id}`);
  }

  createVideoGame(request: SaveVideoGameRequest): Observable<VideoGame> {
    return this.httpClient.post<VideoGame>(this.baseUrl, request);
  }

  updateVideoGame(id: number, request: SaveVideoGameRequest): Observable<VideoGame> {
    return this.httpClient.put<VideoGame>(`${this.baseUrl}/${id}`, request);
  }

  deleteVideoGame(id: number): Observable<void> {
    return this.httpClient.delete<void>(`${this.baseUrl}/${id}`);
  }
}