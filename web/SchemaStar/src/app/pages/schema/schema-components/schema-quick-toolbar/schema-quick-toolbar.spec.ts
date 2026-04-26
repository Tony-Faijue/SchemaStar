import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SchemaQuickToolbar } from './schema-quick-toolbar';

describe('SchemaQuickToolbar', () => {
  let component: SchemaQuickToolbar;
  let fixture: ComponentFixture<SchemaQuickToolbar>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [SchemaQuickToolbar]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SchemaQuickToolbar);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
