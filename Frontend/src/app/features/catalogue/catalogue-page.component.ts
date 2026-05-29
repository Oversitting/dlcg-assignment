import { CommonModule } from '@angular/common';
import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { NgbAlertModule, NgbPaginationModule } from '@ng-bootstrap/ng-bootstrap';
import { catalogueDefaults } from '../../core/config/video-game.constants';
import { ApiClientError } from '../../core/http/api-client-error';
import {
  createEmptyVideoGameBrowseFilters,
  VideoGame,
  VideoGameBrowseFilters,
} from '../../core/models/video-game';
import { VideoGameApiService } from '../../core/services/video-game-api.service';

@Component({
  selector: 'app-catalogue-page',
  imports: [CommonModule, FormsModule, RouterLink, NgbAlertModule, NgbPaginationModule],
  templateUrl: './catalogue-page.component.html',
  styleUrl: './catalogue-page.component.scss',
})
export class CataloguePageComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);
  private readonly route = inject(ActivatedRoute);
  private readonly videoGameApiService = inject(VideoGameApiService);

  protected filters: VideoGameBrowseFilters = createEmptyVideoGameBrowseFilters();
  protected videoGames: VideoGame[] = [];
  protected totalCount = 0;
  protected loading = false;
  protected errorMessage = '';
  protected successMessage = '';
  protected page = 1;
  protected pageSize = catalogueDefaults.pageSize;
  protected readonly pageSizeOptions = [6, 12, 24];

  ngOnInit(): void {
    this.route.queryParamMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      this.successMessage = params.has('saved')
        ? 'Catalogue entry saved.'
        : params.has('deleted')
          ? 'Catalogue entry deleted.'
          : '';
    });

    this.loadVideoGames();
  }

  protected applyFilters(): void {
    this.page = 1;
    this.loadVideoGames();
  }

  protected onPageChange(page: number): void {
    this.page = page;
    this.loadVideoGames();
  }

  protected onPageSizeChange(): void {
    this.page = 1;
    this.loadVideoGames();
  }

  protected clearFilters(): void {
    this.filters = createEmptyVideoGameBrowseFilters();
    this.pageSize = catalogueDefaults.pageSize;
    this.applyFilters();
  }

  private loadVideoGames(): void {
    this.loading = true;
    this.errorMessage = '';

    this.videoGameApiService
      .getVideoGames(this.filters, this.page, this.pageSize)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          this.videoGames = response.items;
          this.totalCount = response.totalCount;
          this.loading = false;
        },
        error: (error: unknown) => {
          this.loading = false;
          this.errorMessage = error instanceof ApiClientError ? error.message : 'The catalogue could not be loaded.';
        },
      });
  }

  protected scoreBadgeClass(score: number | null): string {
    if (score === null || score === undefined) return 'score-badge score-badge-none';
    if (score >= 80) return 'score-badge score-badge-high';
    if (score >= 60) return 'score-badge score-badge-mid';
    return 'score-badge score-badge-low';
  }
}