import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SchemaToolbar } from './schema-toolbar';

describe('SchemaToolbar', () => {
  let component: SchemaToolbar;
  let fixture: ComponentFixture<SchemaToolbar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SchemaToolbar]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SchemaToolbar);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
