import { Routes } from '@angular/router';
import { CataloguePageComponent } from './features/catalogue/catalogue-page.component';
import { VideoGameEditorPageComponent } from './features/video-game-editor/video-game-editor-page.component';

export const routes: Routes = [
	{
		path: '',
		title: 'Browse Catalogue',
		component: CataloguePageComponent,
	},
	{
		path: 'games/new',
		title: 'Add Game',
		component: VideoGameEditorPageComponent,
	},
	{
		path: 'games/:id/edit',
		title: 'Edit Game',
		component: VideoGameEditorPageComponent,
	},
	{
		path: '**',
		redirectTo: '',
	},
];
