import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateSchemaForm } from './create-schema-form';

describe('CreateSchemaForm', () => {
  let component: CreateSchemaForm;
  let fixture: ComponentFixture<CreateSchemaForm>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateSchemaForm]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateSchemaForm);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
