import { CommonModule } from '@angular/common';
import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormControl, NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { NgbAlertModule } from '@ng-bootstrap/ng-bootstrap';
import { videoGameValidation } from '../../core/config/video-game.constants';
import { ApiClientError } from '../../core/http/api-client-error';
import { SaveVideoGameRequest, VideoGame } from '../../core/models/video-game';
import { VideoGameApiService } from '../../core/services/video-game-api.service';

type VideoGameForm = {
  title: FormControl<string>;
  genre: FormControl<string>;
  platform: FormControl<string>;
  releaseYear: FormControl<number>;
  developer: FormControl<string>;
  publisher: FormControl<string>;
  criticScore: FormControl<number>;
  summary: FormControl<string>;
};

@Component({
  selector: 'app-video-game-editor-page',
  imports: [CommonModule, ReactiveFormsModule, RouterLink, NgbAlertModule],
  templateUrl: './video-game-editor-page.component.html',
  styleUrl: './video-game-editor-page.component.scss',
})
export class VideoGameEditorPageComponent implements OnInit {
  private readonly destroyRef = inject(DestroyRef);
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly videoGameApiService = inject(VideoGameApiService);

  protected readonly form = this.formBuilder.group<VideoGameForm>({
    title: this.formBuilder.control('', [Validators.required, Validators.maxLength(videoGameValidation.titleMaxLength)]),
    genre: this.formBuilder.control('', [Validators.required, Validators.maxLength(videoGameValidation.genreMaxLength)]),
    platform: this.formBuilder.control('', [Validators.required, Validators.maxLength(videoGameValidation.platformMaxLength)]),
    releaseYear: this.formBuilder.control(new Date().getFullYear(), [
      Validators.required,
      Validators.min(videoGameValidation.minReleaseYear),
      Validators.max(videoGameValidation.maxReleaseYear),
    ]),
    developer: this.formBuilder.control('', [Validators.required, Validators.maxLength(videoGameValidation.developerMaxLength)]),
    publisher: this.formBuilder.control('', [Validators.required, Validators.maxLength(videoGameValidation.publisherMaxLength)]),
    criticScore: this.formBuilder.control(80, [
      Validators.required,
      Validators.min(videoGameValidation.minCriticScore),
      Validators.max(videoGameValidation.maxCriticScore),
    ]),
    summary: this.formBuilder.control('', [Validators.maxLength(videoGameValidation.summaryMaxLength)]),
  });

  protected isNew = true;
  protected loading = false;
  protected saving = false;
  protected errorMessage = '';
  protected validationMessages: string[] = [];
  protected currentVideoGameId: number | null = null;
  protected lastUpdatedUtc = '';

  protected get pageTitle(): string {
    return this.isNew ? 'Add a catalogue entry' : 'Edit catalogue entry';
  }

  protected get submitLabel(): string {
    return this.saving ? 'Saving...' : this.isNew ? 'Create entry' : 'Save changes';
  }

  protected get canDelete(): boolean {
    return !this.isNew && this.currentVideoGameId !== null;
  }

  protected get footerMessage(): string {
    if (!this.isNew && this.currentVideoGameId === null) {
      return 'Review the route or return to the catalogue.';
    }

    return 'New entries are seeded back into the browse view after save.';
  }

  protected get releaseYearHint(): string {
    return `Release year must be between ${videoGameValidation.minReleaseYear} and ${videoGameValidation.maxReleaseYear}.`;
  }

  protected get criticScoreHint(): string {
    return `Critic score must be between ${videoGameValidation.minCriticScore} and ${videoGameValidation.maxCriticScore}.`;
  }

  ngOnInit(): void {
    this.route.paramMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      const rawId = params.get('id');

      this.isNew = rawId === null;
      this.errorMessage = '';
      this.validationMessages = [];
      this.lastUpdatedUtc = '';

      if (rawId === null) {
        this.currentVideoGameId = null;
        this.form.reset({
          title: '',
          genre: '',
          platform: '',
          releaseYear: new Date().getFullYear(),
          developer: '',
          publisher: '',
          criticScore: 80,
          summary: '',
        });
        return;
      }

      const videoGameId = Number(rawId);
      if (Number.isNaN(videoGameId)) {
        this.currentVideoGameId = null;
        this.errorMessage = 'The requested catalogue entry is invalid.';
        return;
      }

      this.currentVideoGameId = videoGameId;
      this.loadVideoGame(videoGameId);
    });
  }

  protected save(): void {
    if (!this.isNew && this.currentVideoGameId === null) {
      this.errorMessage = 'The requested catalogue entry is invalid.';
      return;
    }

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const request = this.createRequest();
    const saveOperation = this.isNew || this.currentVideoGameId === null
      ? this.videoGameApiService.createVideoGame(request)
      : this.videoGameApiService.updateVideoGame(this.currentVideoGameId, request);

    this.saving = true;
    this.errorMessage = '';
    this.validationMessages = [];

    saveOperation.pipe(takeUntilDestroyed(this.destroyRef)).subscribe({
      next: () => {
        this.saving = false;
        void this.router.navigate(['/'], { queryParams: { saved: 1 } });
      },
      error: (error: unknown) => {
        this.saving = false;
        this.handleError(error, 'The catalogue entry could not be saved.');
      },
    });
  }

  protected deleteVideoGame(): void {
    if (this.isNew || this.currentVideoGameId === null || !globalThis.confirm('Delete this catalogue entry?')) {
      return;
    }

    this.saving = true;
    this.errorMessage = '';
    this.validationMessages = [];

    this.videoGameApiService
      .deleteVideoGame(this.currentVideoGameId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.saving = false;
          void this.router.navigate(['/'], { queryParams: { deleted: 1 } });
        },
        error: (error: unknown) => {
          this.saving = false;
          this.handleError(error, 'The catalogue entry could not be deleted.');
        },
      });
  }

  protected hasError(controlName: keyof VideoGameForm, errorName: string): boolean {
    const control = this.form.controls[controlName];
    return control.touched && control.hasError(errorName);
  }

  protected maxLengthMessage(controlName: keyof VideoGameForm, label: string): string {
    const maxLength = this.form.controls[controlName].errors?.['maxlength']?.requiredLength;
    return `${label} must be ${maxLength} characters or fewer.`;
  }

  private loadVideoGame(videoGameId: number): void {
    this.loading = true;
    this.validationMessages = [];

    this.videoGameApiService
      .getVideoGame(videoGameId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (videoGame) => {
          this.loading = false;
          this.populateForm(videoGame);
        },
        error: (error: unknown) => {
          this.loading = false;
          this.handleError(error, 'The requested catalogue entry could not be loaded.');
        },
      });
  }

  private populateForm(videoGame: VideoGame): void {
    this.form.reset({
      title: videoGame.title,
      genre: videoGame.genre,
      platform: videoGame.platform,
      releaseYear: videoGame.releaseYear,
      developer: videoGame.developer,
      publisher: videoGame.publisher,
      criticScore: videoGame.criticScore,
      summary: videoGame.summary ?? '',
    });

    this.lastUpdatedUtc = videoGame.updatedUtc;
  }

  private createRequest(): SaveVideoGameRequest {
    const value = this.form.getRawValue();

    return {
      title: value.title.trim(),
      genre: value.genre.trim(),
      platform: value.platform.trim(),
      releaseYear: value.releaseYear,
      developer: value.developer.trim(),
      publisher: value.publisher.trim(),
      criticScore: value.criticScore,
      summary: value.summary.trim() ? value.summary.trim() : null,
    };
  }

  private handleError(error: unknown, fallbackMessage: string): void {
    if (error instanceof ApiClientError) {
      this.errorMessage = error.message;
      this.validationMessages = Object.values(error.validationErrors).flat();
      return;
    }

    this.errorMessage = fallbackMessage;
    this.validationMessages = [];
  }
}