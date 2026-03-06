import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardMain } from './dashboard-main';

describe('DashboardMain', () => {
  let component: DashboardMain;
  let fixture: ComponentFixture<DashboardMain>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardMain]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DashboardMain);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
