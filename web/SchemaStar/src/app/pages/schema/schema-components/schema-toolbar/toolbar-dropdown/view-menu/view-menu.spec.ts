import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewMenu } from './view-menu';

describe('ViewMenu', () => {
  let component: ViewMenu;
  let fixture: ComponentFixture<ViewMenu>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ViewMenu]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ViewMenu);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
