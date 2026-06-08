import { ComponentFixture, TestBed } from '@angular/core/testing';

import { NodePaletteItem } from './node-palette-item';

describe('NodePaletteItem', () => {
  let component: NodePaletteItem;
  let fixture: ComponentFixture<NodePaletteItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NodePaletteItem]
    })
    .compileComponents();

    fixture = TestBed.createComponent(NodePaletteItem);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
