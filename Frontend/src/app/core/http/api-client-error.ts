export class ApiClientError extends Error {
  constructor(
    message: string,
    readonly status: number,
    readonly validationErrors: Record<string, string[]> = {},
  ) {
    super(message);
    this.name = 'ApiClientError';
  }
}