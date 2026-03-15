import { ComponentFixture, TestBed } from '@angular/core/testing';

import { Schema } from './schema';

describe('Schema', () => {
  let component: Schema;
  let fixture: ComponentFixture<Schema>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Schema]
    })
    .compileComponents();

    fixture = TestBed.createComponent(Schema);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
