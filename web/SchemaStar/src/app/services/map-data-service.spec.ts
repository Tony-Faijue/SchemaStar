import { TestBed } from '@angular/core/testing';

import { MapDataService } from './map-data-service';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { SchemaResponse, SchemaService } from './schema/schema-service';
import { NodeResponse, NodeResponseFull, NodeService, NodeState } from './schema/node-service';
import { EdgeResponse, EdgeService } from './schema/edge-service';
import { NodeAssetResponse, NodeAssetSource, NodeAssetType } from './schema/node-asset-service';
import { of, throwError } from 'rxjs';
import { resource } from '@angular/core';

describe('MapDataService', () => {
  let service: MapDataService;

  let schemaSpy: jasmine.SpyObj<SchemaService>;// Schema Spy
  let nodeSpy: jasmine.SpyObj<NodeService>; //Node Spy
  let edgeSpy: jasmine.SpyObj<EdgeService>; //Edge Spy

  //--Initial States--
  const initialSchema: SchemaResponse = {
      publicId:'123',
      nodeWebName: 'initialSchema',
      createdAt: '2026-02-18T20:52:02-06:00',
      updatedAt: '2026-02-18T20:52:02-06:00',
      lastLayoutAt: '2026-02-18T20:52:02-06:00'
  }

  const mockSchema: SchemaResponse[] = [initialSchema];

  const initialNode: NodeResponse = {
      publicId:'123',
      nodeName: 'initialNode',
      createdAt: '2026-02-18T20:52:02-06:00',
      updatedAt: '2026-02-18T20:52:02-06:00',
      positionX: 10,
      positionY: 20,
      width: 200,
      height: 200,
      state: NodeState.Unlocked,
      nodeWebId: '999'
  }

  const mockNode: NodeResponse[] = [initialNode];

  const initialNodeAsset: NodeAssetResponse = {
      publicId:'123',
      nodeAssetName: 'initialNodeAsset',
      nodeAssetSource: NodeAssetSource.External,
      nodeAssetType: NodeAssetType.Link,
      nodeId: '999'
  }

  const mockNodeAsset: NodeAssetResponse[] = [initialNodeAsset];

  const initialEdge: EdgeResponse = {
      publicId:'123',
      fromNodeId: '555',
      toNodeId: '777',
      nodeWebId: '999'
  }

  const mockEdge: EdgeResponse[] = [initialEdge];

  beforeEach(() => {

    schemaSpy = jasmine.createSpyObj('SchemaService', ['getSchemas']);
    nodeSpy = jasmine.createSpyObj('NodeService', ['getNodes', 'getNodeFull']);
    edgeSpy = jasmine.createSpyObj('EdgeService', ['getEdges']);

    TestBed.configureTestingModule({
      providers: [
        MapDataService,
        { provide: SchemaService, useValue: schemaSpy },
        { provide: NodeService, useValue: nodeSpy },
        { provide: EdgeService, useValue: edgeSpy },
      ],
    });
    service = TestBed.inject(MapDataService);
  });


  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should add a new schema and update and existing one when upsertSchemas succeeds', () => {
    //---Arrange---
    service.schemas.set(mockSchema); //initial state of schemas

    //add schema and update existing one
    const newSchema: SchemaResponse = {
      publicId:'456',
      nodeWebName: 'newSchema',
      createdAt: '2026-02-18T20:52:02-06:00',
      updatedAt: '2026-02-18T20:52:02-06:00',
      lastLayoutAt: '2026-02-18T20:52:02-06:00'
    }

    const updateSchema: SchemaResponse = {
      publicId:'123',
      nodeWebName: 'updateSchema',
      createdAt: '2026-02-18T20:52:02-06:00',
      updatedAt: '2026-02-18T20:52:02-06:00',
      lastLayoutAt: '2026-02-18T20:52:02-06:00'
    }

    const mockSchemas: SchemaResponse[] = [updateSchema, newSchema];

    //---Act---
    service.upsertSchemas(mockSchemas);

    //---Assert---
    const result = service.schemas();
    expect(result.length).toBe(2);
    expect(result.find(s => s.publicId === '123')?.nodeWebName).toEqual('updateSchema');
    expect(result.find(s => s.publicId === '456')?.nodeWebName).toEqual('newSchema');
  });

  it('should remove an existing schema if it exists when deleteSchema succeeds', () => {
     //---Arrange---
    service.schemas.set(mockSchema); //initial state of schemas

    //---Act---
    service.deleteSchema('123');

    //---Assert---
    const result = service.schemas();
    expect(result.length).toBe(0);
  });


 it('should add a new node and update and existing one when upsertNodes succeeds', () => {
    //---Arrange---
    service.nodes.set(mockNode); //initial state of nodes

    //add node and update existing one
    const newNode: NodeResponse = {
      publicId:'456',
      nodeName: 'newNode',
      createdAt: '2026-02-18T20:52:02-06:00',
      updatedAt: '2026-02-18T20:52:02-06:00',
      positionX: 567,
      positionY: 100,
      width: 200,
      height: 200,
      state: NodeState.Unlocked,
      nodeWebId: '998'

    }

    const updateNode: NodeResponse = {
      publicId:'123',
      nodeName: 'updateNode',
      createdAt: '2026-02-18T20:52:02-06:00',
      updatedAt: '2026-02-18T20:52:02-06:00',
      positionX: 0,
      positionY: 0,
      width: 200,
      height: 200,
      state: NodeState.Locked,
      nodeWebId: '999'
    }

    const mockNodes: NodeResponse[] = [updateNode, newNode];

    //---Act---
    service.upsertNodes(mockNodes);

    //---Assert---
    const result = service.nodes();
    expect(result.length).toBe(2);
    expect(result.find(n => n.publicId === '123')?.nodeName).toEqual('updateNode');
    expect(result.find(n => n.publicId === '123')?.state).toBe(0);
    expect(result.find(n => n.publicId === '456')?.nodeName).toEqual('newNode');
  });

  it('should remove an existing node if it exists when deleteNode succeeds', () => {
     //---Arrange---
    service.nodes.set(mockNode); //initial state of nodes 

    //---Act---
    service.deleteNode('123');

    //---Assert---
    const result = service.nodes();
    expect(result.length).toBe(0);
  });  

 it('should add a new node asset and update and existing one when upsertAssets succeeds', () => {
    //---Arrange---
    service.assets.set(mockNodeAsset); //initial state of nodes assets

    //add node asset and update existing one
    const newNodeAsset: NodeAssetResponse = {
      publicId:'456',
      nodeAssetName: 'newNodeAsset',
      nodeAssetSource: NodeAssetSource.External,
      nodeAssetType: NodeAssetType.Link,
      nodeId: '998'
    }

    const updateNodeAsset: NodeAssetResponse = {
      publicId:'123',
      nodeAssetName: 'updateNodeAsset',
      nodeAssetSource: NodeAssetSource.Upload,
      nodeAssetType: NodeAssetType.Video,
      nodeId: '999'
    }

    const mockNodeAssets: NodeAssetResponse[] = [updateNodeAsset, newNodeAsset];

    //---Act---
    service.upsertAssets(mockNodeAssets);

    //---Assert---
    const result = service.assets();
    expect(result.length).toBe(2);
    expect(result.find(n => n.publicId === '123')?.nodeAssetName).toEqual('updateNodeAsset');
    expect(result.find(n => n.publicId === '123')?.nodeAssetSource).toBe(0);
    expect(result.find(n => n.publicId === '123')?.nodeAssetType).toBe(2);
    expect(result.find(n => n.publicId === '456')?.nodeAssetName).toEqual('newNodeAsset');
  });  

  it('should remove an existing node asset if it exists when deleteAsset succeeds', () => {
     //---Arrange---
    service.assets.set(mockNodeAsset); //initial state of node assets

    //---Act---
    service.deleteAsset('123');

    //---Assert---
    const result = service.assets();
    expect(result.length).toBe(0);
  }); 

 it('should add a new edge and update and existing one when upsertEdges succeeds', () => {
    //---Arrange---
    service.edges.set(mockEdge); //initial state of edges

    //add edge and update existing one
    const newEdge: EdgeResponse = {
      publicId:'456',
      fromNodeId: '1313',
      toNodeId: '1212',
      nodeWebId: '998'
    }

    const updateEdge: EdgeResponse = {
      publicId:'123',
      fromNodeId: '625',
      toNodeId: '888',
      nodeWebId: '999'
    }

    const mockEdges: EdgeResponse[] = [updateEdge, newEdge];

    //---Act---
    service.upsertEdges(mockEdges);

    //---Assert---
    const result = service.edges();
    expect(result.length).toBe(2);
    expect(result.find(n => n.publicId === '123')?.fromNodeId).toEqual('625');
    expect(result.find(n => n.publicId === '123')?.toNodeId).toEqual('888');
    expect(result.find(n => n.publicId === '456')?.nodeWebId).toEqual('998');
  });  

  it('should remove an existing edge if it exists when deleteEdge succeeds', () => {
     //---Arrange---
    service.edges.set(mockEdge); //initial state of edge

    //---Act---
    service.deleteEdge('123');

    //---Assert---
    const result = service.edges();
    expect(result.length).toBe(0);
  }); 

  it('should stop loading and update empty edges for schema when loadSchemaResources has no nodes', () => {
    //---Arrange--
    const schemaId = '123';

    //Mock the first forkJoin results empty arrays
    nodeSpy.getNodes.and.returnValue(of([]));
    edgeSpy.getEdges.and.returnValue(of([]));

    //---Act--
    service.loadSchemaResources(schemaId);

    //---Assert--
    expect(service.selectedSchemaId()).toBe(schemaId);
    expect(service.isLoading()).toBe(false);
    expect(nodeSpy.getNodeFull).not.toHaveBeenCalled();
    expect(service.error()).toBe(null);
  });

  it('should stop loading and update edges, nodes, and assets for schema when loadSchemaResources succeeds', () => {
    //---Arrange--
    const id = '996'
    const baseNode: NodeResponse = {
      ...initialNode,
      publicId: id,
    }

    //Mock the first forkJoin with results of data in the arrays
    nodeSpy.getNodes.and.returnValue(of([baseNode]));
    edgeSpy.getEdges.and.returnValue(of(mockEdge));

    //Mock the nested second forkJoin
    const fullNode:NodeResponseFull = { ...baseNode, NodeAssets: mockNodeAsset };
    nodeSpy.getNodeFull.and.returnValue(of(fullNode));

    //---Act--
    service.loadSchemaResources(id);

    //---Assert--
    expect(service.selectedSchemaId()).toBe(id);

    expect(service.edges().length).toBe(1);
    expect(service.nodes().length).toBe(1);

    expect(service.assets().length).toBe(1);
    expect(service.assets()[0].publicId).toEqual('123');

    expect(service.isLoading()).toBe(false);
    expect(nodeSpy.getNodeFull).toHaveBeenCalledWith('996');
    expect(service.error()).toBe(null);
  });

  it('should set error signal when initial resource fetch fails when loadSchemaResources fails', () => {
    //---Arrange---
    const errorMessage = 'Failed loading schema resources';
    nodeSpy.getNodes.and.returnValue(throwError(() => new Error('An API Error occured')));
    edgeSpy.getEdges.and.returnValue(of(mockEdge));
    //---Act---
    service.loadSchemaResources('123');
    //---Assert---
    expect(service.error()).toBe(errorMessage);
    expect(service.isLoading()).toBe(false);
    expect(nodeSpy.getNodeFull).not.toHaveBeenCalled();
  });

  it('should set error signal when full node detail fetch fails when loadSchemaResources fails', () => {
    //---Arrange---
    const errorMessage = 'Failed to load node details';
    nodeSpy.getNodes.and.returnValue(of(mockNode));
    edgeSpy.getEdges.and.returnValue(of(mockEdge));
    nodeSpy.getNodeFull.and.returnValue(throwError(() => new Error('API Error')));
    //---Act---
    service.loadSchemaResources('123');
    //---Assert---
    expect(service.error()).toBe(errorMessage);
    expect(service.isLoading()).toBe(false);

    expect(service.edges().length).toBe(1);
  });

  it('should load schemas and trigger loading for the first schema when loadUserWorkSpace succeeds', () => {
    //---Arrange---
    const schema2: SchemaResponse = {
      ...initialSchema,
      publicId: 'n2'
    };
    
    const mockSchemas = [initialSchema, schema2];
    schemaSpy.getSchemas.and.returnValue(of(mockSchemas));

    //Spy on helper method
    const resourceSpy = spyOn(service, 'loadSchemaResources');
    //---Act---
    service.loadUserWorkspace();
    //---Assert---
    expect(service.schemas().length).toBe(2);
    expect(resourceSpy).toHaveBeenCalledWith('123'); //first schema
    expect(service.isLoading()).toBe(true); //true since loadSchemaResources sets it to true
  });

it('should stop loading and update empty schemas when loadUserWorkSpace has no schemas', () => {
    //---Arrange---

    schemaSpy.getSchemas.and.returnValue(of([]));

    //Spy on helper method
    const resourceSpy = spyOn(service, 'loadSchemaResources');
    //---Act---
    service.loadUserWorkspace();
    //---Assert---
    expect(service.schemas().length).toBe(0);
    expect(resourceSpy).not.toHaveBeenCalled(); 
    expect(service.isLoading()).toBe(false);
  });

it('should set error signal when schema fetch fails when loadUserWorkSpace fails', () => {
    //---Arrange--- 
    schemaSpy.getSchemas.and.returnValue(throwError(() => new Error('API Error')));

    //Spy on helper method
    const resourceSpy = spyOn(service, 'loadSchemaResources');
    //---Act---
    service.loadUserWorkspace();
    //---Assert---
    expect(service.error()).toBe('Failed to load user workspace');
    expect(service.isLoading()).toBe(false);
    expect(resourceSpy).not.toHaveBeenCalled();
    expect(service.schemas().length).toBe(0);
  });  
});
