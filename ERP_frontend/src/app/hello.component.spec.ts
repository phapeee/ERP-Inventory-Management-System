import { ComponentFixture, TestBed } from '@angular/core/testing';
import { describe, it, expect, beforeEach } from 'vitest';
import { HelloComponent } from './hello.component';
// import { withZoneless } from '../test-setup';

describe('HelloComponent', () => {
  let fixture: ComponentFixture<HelloComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HelloComponent],
      // providers: withZoneless()
    }).compileComponents();

    fixture = TestBed.createComponent(HelloComponent);
    fixture.detectChanges();
  });

  it('renders a greeting', () => {
    const h1: HTMLElement = fixture.nativeElement.querySelector('h1');
    expect(h1.textContent).toContain('Hello Vitest');
  });
});
