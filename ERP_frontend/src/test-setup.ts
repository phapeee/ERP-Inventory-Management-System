import { provideZonelessChangeDetection } from '@angular/core';

// make it easy to import in every TestBed
export const withZoneless = () => [provideZonelessChangeDetection()];
