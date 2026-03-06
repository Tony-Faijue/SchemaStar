import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateSchema } from './create-schema';

describe('CreateSchema', () => {
  let component: CreateSchema;
  let fixture: ComponentFixture<CreateSchema>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateSchema]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CreateSchema);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
