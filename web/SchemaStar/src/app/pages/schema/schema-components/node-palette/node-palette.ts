import { Component } from '@angular/core';
import { NodePaletteItem } from "./node-palette-item/node-palette-item";

@Component({
  selector: 'app-node-palette',
  imports: [NodePaletteItem],
  templateUrl: './node-palette.html',
  styleUrl: './node-palette.css',
})
export class NodePalette {

}
