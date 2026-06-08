import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NodePalette } from './node-palette';

describe('NodePalette', () => {
  let component: NodePalette;
  let fixture: ComponentFixture<NodePalette>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NodePalette]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NodePalette);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
