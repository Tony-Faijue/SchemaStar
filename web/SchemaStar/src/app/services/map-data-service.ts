import { Injectable, signal } from '@angular/core';
import { NodeResponse } from './schema/node-service';
import { EdgeResponse } from './schema/edge-service';
import { SchemaResponse } from './schema/schema-service';
import { NodeAssetResponse } from './schema/node-asset-service';


@Injectable({
  providedIn: 'root',
})
export class MapDataService {
  //Data Layer
  //All resource data for the user
  schemas = signal<SchemaResponse[]>([]);
  nodes = signal<NodeResponse[]>([]);
  edges = signal<EdgeResponse[]>([]);
  nodeAssets = signal<NodeAssetResponse[]>([]);
  
}
