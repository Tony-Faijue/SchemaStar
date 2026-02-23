import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FooterMenu } from './footer-menu';

describe('FooterMenu', () => {
  let component: FooterMenu;
  let fixture: ComponentFixture<FooterMenu>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [FooterMenu]
    })
    .compileComponents();

    fixture = TestBed.createComponent(FooterMenu);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
