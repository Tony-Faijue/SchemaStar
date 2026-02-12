import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GuestNavigation } from './guest-navigation';

describe('GuestNavigation', () => {
  let component: GuestNavigation;
  let fixture: ComponentFixture<GuestNavigation>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GuestNavigation]
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
