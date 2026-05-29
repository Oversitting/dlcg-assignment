export interface VideoGame {
  id: number;
  title: string;
  genre: string;
  platform: string;
  releaseYear: number;
  developer: string;
  publisher: string;
  criticScore: number;
  summary: string | null;
  updatedUtc: string;
}

export interface PagedResponse<TItem> {
  items: TItem[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface SaveVideoGameRequest {
  title: string;
  genre: string;
  platform: string;
  releaseYear: number;
  developer: string;
  publisher: string;
  criticScore: number;
  summary: string | null;
}

export interface VideoGameBrowseFilters {
  searchTerm: string;
  genre: string;
  platform: string;
  orderBy: VideoGameOrderBy;
  orderDirection: VideoGameOrderDirection;
}

export type VideoGameOrderBy = 'id' | 'title' | 'releaseYear' | 'criticScore' | 'updatedUtc';

export type VideoGameOrderDirection = 'asc' | 'desc';

export function createEmptyVideoGameBrowseFilters(): VideoGameBrowseFilters {
  return {
    searchTerm: '',
    genre: '',
    platform: '',
    orderBy: 'id',
    orderDirection: 'asc',
  };
}