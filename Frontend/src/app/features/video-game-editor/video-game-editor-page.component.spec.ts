import '../../../testing/localize';
import { ActivatedRoute, convertToParamMap, provideRouter } from '@angular/router';
import { TestBed } from '@angular/core/testing';
import { BehaviorSubject, of, throwError } from 'rxjs';
import { ApiClientError } from '../../core/http/api-client-error';
import { VideoGameApiService } from '../../core/services/video-game-api.service';
import { VideoGameEditorPageComponent } from './video-game-editor-page.component';

describe('VideoGameEditorPageComponent', () => {
  it('submits a valid create request', async () => {
    const requests: unknown[] = [];
    const apiService = {
      getVideoGame: () => of(),
      createVideoGame: (request: unknown) => {
        requests.push(request);
        return of({ id: 1 });
      },
      updateVideoGame: () => of({ id: 1 }),
      deleteVideoGame: () => of(void 0),
    };

    await TestBed.configureTestingModule({
      imports: [VideoGameEditorPageComponent],
      providers: [
        provideRouter([]),
        { provide: VideoGameApiService, useValue: apiService },
        { provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({})) } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(VideoGameEditorPageComponent);
    fixture.detectChanges();

    const component = fixture.componentInstance as unknown as {
      form: { setValue(value: unknown): void };
      save(): void;
    };

    component.form.setValue({
      title: 'Sea of Stars',
      genre: 'RPG',
      platform: 'PC',
      releaseYear: 2023,
      developer: 'Sabotage Studio',
      publisher: 'Sabotage Studio',
      criticScore: 89,
      summary: 'A modern retro-inspired RPG.',
    });

    component.save();

    expect(requests).toHaveLength(1);
  });

  it('renders validation messages returned by the API', async () => {
    const apiService = {
      getVideoGame: () => of(),
      createVideoGame: () =>
        throwError(() => new ApiClientError('The request failed validation. Review the highlighted fields and try again.', 400, {
          title: ['Title is required.'],
        })),
      updateVideoGame: () => of({ id: 1 }),
      deleteVideoGame: () => of(void 0),
    };

    await TestBed.configureTestingModule({
      imports: [VideoGameEditorPageComponent],
      providers: [
        provideRouter([]),
        { provide: VideoGameApiService, useValue: apiService },
        { provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({})) } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(VideoGameEditorPageComponent);
    fixture.detectChanges();

    const component = fixture.componentInstance as unknown as {
      form: { setValue(value: unknown): void };
      save(): void;
    };

    component.form.setValue({
      title: ' ',
      genre: 'RPG',
      platform: 'PC',
      releaseYear: 2023,
      developer: 'Sabotage Studio',
      publisher: 'Sabotage Studio',
      criticScore: 89,
      summary: '',
    });

    component.save();
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Title is required.');
  });

  it('does not submit a create request when the edit route id is invalid', async () => {
    const createVideoGame = vi.fn(() => of({ id: 1 }));
    const updateVideoGame = vi.fn(() => of({ id: 1 }));
    const routeParamMap = new BehaviorSubject(convertToParamMap({ id: 'not-a-number' }));
    const apiService = {
      getVideoGame: () => of(),
      createVideoGame,
      updateVideoGame,
      deleteVideoGame: () => of(void 0),
    };

    await TestBed.configureTestingModule({
      imports: [VideoGameEditorPageComponent],
      providers: [
        provideRouter([]),
        { provide: VideoGameApiService, useValue: apiService },
        { provide: ActivatedRoute, useValue: { paramMap: routeParamMap.asObservable() } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(VideoGameEditorPageComponent);
    fixture.detectChanges();

    const component = fixture.componentInstance as unknown as {
      errorMessage: string;
      form: { setValue(value: unknown): void };
      save(): void;
    };

    component.form.setValue({
      title: 'Sea of Stars',
      genre: 'RPG',
      platform: 'PC',
      releaseYear: 2023,
      developer: 'Sabotage Studio',
      publisher: 'Sabotage Studio',
      criticScore: 89,
      summary: 'A modern retro-inspired RPG.',
    });

    component.save();

    expect(component.errorMessage).toContain('invalid');
    expect(createVideoGame).not.toHaveBeenCalled();
    expect(updateVideoGame).not.toHaveBeenCalled();
  });

  it('renders client-side validation messages for numeric and max-length errors', async () => {
    const apiService = {
      getVideoGame: () => of(),
      createVideoGame: () => of({ id: 1 }),
      updateVideoGame: () => of({ id: 1 }),
      deleteVideoGame: () => of(void 0),
    };

    await TestBed.configureTestingModule({
      imports: [VideoGameEditorPageComponent],
      providers: [
        provideRouter([]),
        { provide: VideoGameApiService, useValue: apiService },
        { provide: ActivatedRoute, useValue: { paramMap: of(convertToParamMap({})) } },
      ],
    }).compileComponents();

    const fixture = TestBed.createComponent(VideoGameEditorPageComponent);
    fixture.detectChanges();

    const component = fixture.componentInstance as unknown as {
      form: { setValue(value: unknown): void };
      save(): void;
    };

    component.form.setValue({
      title: 'T'.repeat(121),
      genre: 'RPG',
      platform: 'PC',
      releaseYear: 2200,
      developer: 'Sabotage Studio',
      publisher: 'Sabotage Studio',
      criticScore: 120,
      summary: '',
    });

    component.save();
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Title must be 120 characters or fewer.');
    expect(compiled.textContent).toContain('Release year must be between 1970 and 2100.');
    expect(compiled.textContent).toContain('Critic score must be between 0 and 100.');
  });
});