import { Injectable } from '@angular/core';
@Injectable({ providedIn: 'root' })
export class MathService {
  add(a: number, b: number) {
    return a + b;
  }
}
