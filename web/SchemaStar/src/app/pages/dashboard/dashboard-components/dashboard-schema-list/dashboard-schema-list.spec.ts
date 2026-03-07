import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardSchemaList } from './dashboard-schema-list';

describe('DashboardSchemaList', () => {
  let component: DashboardSchemaList;
  let fixture: ComponentFixture<DashboardSchemaList>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardSchemaList]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DashboardSchemaList);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
