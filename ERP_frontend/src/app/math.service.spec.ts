import { TestBed } from '@angular/core/testing';
import { describe, it, expect, beforeEach } from 'vitest';
import { MathService } from './math.service';
// import { withZoneless } from '../test-setup';

describe('MathService', () => {
  let svc: MathService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      // providers: withZoneless()
    });
    svc = TestBed.inject(MathService);
  });

  it('adds numbers', () => {
    expect(svc.add(2, 3)).toBe(5);
  });
});
