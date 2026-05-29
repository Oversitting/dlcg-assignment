import '../../../testing/localize';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ApiClientError } from '../../core/http/api-client-error';
import { VideoGameApiService } from '../../core/services/video-game-api.service';
import { CataloguePageComponent } from './catalogue-page.component';

describe('CataloguePageComponent', () => {
  it('sends ordering and pagination settings to the API', async () => {
    const apiService = {
      getVideoGames: vi.fn(() =>
        of({
          items: [],
          totalCount: 0,
          pageNumber: 1,
          pageSize: 6,
        })),
    };

    await TestBed.configureTestingModule({
      imports: [CataloguePageComponent],
      providers: [
        provideRouter([]),
        { provide: VideoGameApiService, useValue: apiService },
        { provide: ActivatedRoute, useValue: { queryParamMap: of(convertToParamMap({})) } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(CataloguePageComponent);
    fixture.detectChanges();

    const component = fixture.componentInstance as unknown as {
      filters: {
        orderBy: 'releaseYear';
        orderDirection: 'desc';
      };
      pageSize: number;
      applyFilters(): void;
    };

    component.filters.orderBy = 'releaseYear';
    component.filters.orderDirection = 'desc';
    component.pageSize = 12;
    component.applyFilters();

    expect(apiService.getVideoGames).toHaveBeenLastCalledWith(
      expect.objectContaining({ orderBy: 'releaseYear', orderDirection: 'desc' }),
      1,
      12,
    );
  });

  it('renders paged catalogue results', async () => {
    const apiService = {
      getVideoGames: () =>
        of({
          items: [
            {
              id: 1,
              title: 'Sea of Stars',
              genre: 'RPG',
              platform: 'PC',
              releaseYear: 2023,
              developer: 'Sabotage Studio',
              publisher: 'Sabotage Studio',
              criticScore: 89,
              summary: null,
              updatedUtc: '2024-01-01T00:00:00Z',
            },
          ],
          totalCount: 1,
          pageNumber: 1,
          pageSize: 6,
        }),
    };

    await TestBed.configureTestingModule({
      imports: [CataloguePageComponent],
      providers: [
        provideRouter([]),
        { provide: VideoGameApiService, useValue: apiService },
        { provide: ActivatedRoute, useValue: { queryParamMap: of(convertToParamMap({ deleted: '1' })) } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(CataloguePageComponent);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Sea of Stars');
    expect(compiled.textContent).toContain('Catalogue entry deleted.');
  });

  it('resets page size to the default when filters are cleared', async () => {
    const apiService = {
      getVideoGames: vi.fn(() =>
        of({
          items: [],
          totalCount: 0,
          pageNumber: 1,
          pageSize: 6,
        })),
    };

    await TestBed.configureTestingModule({
      imports: [CataloguePageComponent],
      providers: [
        provideRouter([]),
        { provide: VideoGameApiService, useValue: apiService },
        { provide: ActivatedRoute, useValue: { queryParamMap: of(convertToParamMap({})) } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(CataloguePageComponent);
    fixture.detectChanges();

    const component = fixture.componentInstance as unknown as {
      pageSize: number;
      clearFilters(): void;
    };

    component.pageSize = 24;
    component.clearFilters();

    expect(component.pageSize).toBe(6);
    expect(apiService.getVideoGames).toHaveBeenLastCalledWith(expect.anything(), 1, 6);
  });

  it('shows an API error message when loading fails', async () => {
    const apiService = {
      getVideoGames: () => throwError(() => new ApiClientError('The catalogue could not be loaded.', 503)),
    };

    await TestBed.configureTestingModule({
      imports: [CataloguePageComponent],
      providers: [
        provideRouter([]),
        { provide: VideoGameApiService, useValue: apiService },
        { provide: ActivatedRoute, useValue: { queryParamMap: of(convertToParamMap({})) } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(CataloguePageComponent);
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('The catalogue could not be loaded.');
  });
});