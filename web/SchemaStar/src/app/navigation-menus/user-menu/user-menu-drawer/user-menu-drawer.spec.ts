import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UserMenuDrawer } from './user-menu-drawer';

describe('UserMenuDrawer', () => {
  let component: UserMenuDrawer;
  let fixture: ComponentFixture<UserMenuDrawer>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [UserMenuDrawer]
    })
    .compileComponents();

    fixture = TestBed.createComponent(UserMenuDrawer);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
