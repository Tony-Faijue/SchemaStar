import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuestNavigation } from './guest-navigation';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('GuestNavigation', () => {
  let component: GuestNavigation;
  let fixture: ComponentFixture<GuestNavigation>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GuestNavigation],
      //provide needed dependencies
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting()
      ],
    })
    .compileComponents();

    fixture = TestBed.createComponent(GuestNavigation);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
