import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardSchemaCard } from './dashboard-schema-card';

describe('DashboardSchemaCard', () => {
  let component: DashboardSchemaCard;
  let fixture: ComponentFixture<DashboardSchemaCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardSchemaCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DashboardSchemaCard);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
