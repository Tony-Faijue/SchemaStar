import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SchemaLayout } from './schema-layout';

describe('SchemaLayout', () => {
  let component: SchemaLayout;
  let fixture: ComponentFixture<SchemaLayout>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SchemaLayout]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SchemaLayout);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
