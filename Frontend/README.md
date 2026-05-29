# Frontend

Angular frontend for the Video Game Catalogue assignment. The app has two routed pages:

- `/` for browsing, filtering, ordering, and paging catalogue entries
- `/games/new` and `/games/:id/edit` for creating and editing entries

## Development server

From this directory, start the app with:

```bash
npm start
```

This runs `ng serve` with `proxy.conf.json`, so API calls to `/api` are forwarded to `http://localhost:5201`.

## Build

Create a production build with:

```bash
npm run build
```

The output is written to `dist/video-game-catalogue/`.

## Unit tests

Run the Vitest-backed Angular unit tests with:

```bash
npm test -- --watch=false
```

## Notes

- The shared validation limits live in `src/app/core/config/video-game.constants.ts`.
- Bootstrap and ng-bootstrap are used for layout, styling, and alerts.
- No end-to-end test target is configured in `angular.json`.
