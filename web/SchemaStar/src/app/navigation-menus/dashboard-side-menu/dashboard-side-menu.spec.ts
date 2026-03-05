import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardSideMenu } from './dashboard-side-menu';

describe('DashboardSideMenu', () => {
  let component: DashboardSideMenu;
  let fixture: ComponentFixture<DashboardSideMenu>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardSideMenu]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DashboardSideMenu);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
