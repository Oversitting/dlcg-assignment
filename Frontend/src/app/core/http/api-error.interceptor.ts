import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { ApiClientError } from './api-client-error';

type ProblemDetails = {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
};

export const apiErrorInterceptor: HttpInterceptorFn = (request, next) =>
  next(request).pipe(
    catchError((error: unknown) => {
      if (!(error instanceof HttpErrorResponse)) {
        return throwError(() => error);
      }

      const problem = typeof error.error === 'object' && error.error !== null ? (error.error as ProblemDetails) : null;
      const validationErrors = problem?.errors ?? {};

      return throwError(
        () =>
          new ApiClientError(
            resolveMessage(error, problem),
            error.status,
            validationErrors,
          ),
      );
    }),
  );

function resolveMessage(error: HttpErrorResponse, problem: ProblemDetails | null): string {
  if (error.status === 0) {
    return 'The API is unavailable. Start the ASP.NET backend and try again.';
  }

  if (error.status === 400 && Object.keys(problem?.errors ?? {}).length > 0) {
    return 'The request failed validation. Review the highlighted fields and try again.';
  }

  return problem?.detail ?? problem?.title ?? 'The request could not be completed.';
}