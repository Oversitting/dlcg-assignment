export const videoGameValidation = {
  titleMaxLength: 120,
  genreMaxLength: 60,
  platformMaxLength: 60,
  developerMaxLength: 120,
  publisherMaxLength: 120,
  summaryMaxLength: 500,
  minReleaseYear: 1970,
  maxReleaseYear: 2100,
  minCriticScore: 0,
  maxCriticScore: 100,
} as const;

export const catalogueDefaults = {
  pageSize: 6,
} as const;