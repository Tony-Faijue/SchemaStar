import { TestBed } from '@angular/core/testing';

import { RegisterSchema, SchemaResponse, SchemaResponseFull, SchemaService, UpdateSchema } from './schema-service';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from '../../http-interceptors/auth-interceptor';
import { SecretData } from '../../../../environment';
import { NodeResponse, NodeResponseFull, NodeState } from './node-service';
import { EdgeResponse, EdgeType } from './edge-service';
import { NodeAssetResponse, NodeAssetSource, NodeAssetType } from './node-asset-service';

describe('SchemaService', () => {
  let service: SchemaService;
  let httpTestingController: HttpTestingController

  const mockBaseUrl = `${SecretData.baseuUrl}/api/nodewebs`;

  const mockSchemaResponse: SchemaResponse = {
    publicId:'123',
    nodeWebName: 'Test_Schema',
    createdAt: '2026-02-18T20:52:02-06:00',
    updatedAt: '2026-02-18T20:52:02-06:00',
    lastLayoutAt: '2026-02-18T20:52:02-06:00'
  };

  const node: NodeResponse = {
    publicId: '123',
    nodeName: 'Test_Node',
    positionX: 0,
    positionY: 0,
    width: 150,
    height: 200,
    state: NodeState.Locked,
    createdAt: '2026-02-18T20:52:02-06:00',
    nodeWebId: mockSchemaResponse.publicId
  };

  const node2: NodeResponse = {
    publicId: '456',
    nodeName: 'Test_Node2',
    positionX: 0,
    positionY: 0,
    width: 150,
    height: 200,
    state: NodeState.Locked,
    createdAt: '2026-02-18T20:52:02-06:00',
    nodeWebId: mockSchemaResponse.publicId
  };

  const edge: EdgeResponse = {
    publicId: '789',
    edgeType: EdgeType.Directed,
    fromNodeId: node.publicId,
    toNodeId: node2.publicId,
    nodeWebId: mockSchemaResponse.publicId
  };

  const nodes:NodeResponse[] = [node, node2];
  const edges:EdgeResponse[] = [edge];

  //----SchemaResponse Full-------
 const myAsset1: NodeAssetResponse = {
    publicId: '909',
    nodeAssetName: 'asset_1',
    nodeAssetType: NodeAssetType.Link,
    nodeAssetSource: NodeAssetSource.External,
    nodeId: node.publicId
  }

  const myAsset2: NodeAssetResponse = {
    publicId: '908',
    nodeAssetName: 'asset_2',
    nodeAssetType: NodeAssetType.Audio,
    nodeAssetSource: NodeAssetSource.Upload,
    nodeId: node.publicId
  }
    
  const nodeAssetsForNode1Full: NodeAssetResponse[] = [myAsset1, myAsset2];

  const node1Full: NodeResponseFull = {
    ...node,
    NodeAssets: nodeAssetsForNode1Full
  };

  const node2Full: NodeResponseFull = {
    ...node,
    NodeAssets: []
  };

  const nodesFull: NodeResponseFull[] = [node1Full, node2Full];

  const mockSchemaResponseFull: SchemaResponseFull = {
     ...mockSchemaResponse,
      nodes: nodesFull,
      edges: edges
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        SchemaService,
        provideHttpClient(
          withInterceptors([authInterceptor])
        ),
        provideHttpClientTesting()
      ],
    });
    service = TestBed.inject(SchemaService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    //ensure no http request that were unplanned are made
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
  
  it('should get all schemas', () => {
    //Arrange
    const mockSchemas: SchemaResponse[] = [mockSchemaResponse];

    //Act and assert
    service.getSchemas().subscribe((schemas) => {
      expect(schemas.length).toBe(1);
      expect(schemas).toEqual(mockSchemas);
    });
    
    //Verify
    const req = httpTestingController.expectOne(mockBaseUrl);
    expect(req.request.method).toBe('GET');
    req.flush(mockSchemas);
  });

  it('should get a single schema and set the currentSchema signal with the id when getSchema succeeds', () => {
    //Arrange
    const mockId = '123';
    const url = `${mockBaseUrl}/${mockId}`;

    //Act and Assert
    expect(service.currentSchema()).toBeNull(); //assert currentSchema is null

    service.getSchema(mockId).subscribe(schemaResponse => {
      expect(schemaResponse).toEqual(mockSchemaResponse);
      expect(service.currentSchema()).toEqual(mockSchemaResponse); //assert currentSchema is set
    });

    //Verify
    const req = httpTestingController.expectOne(url);
    expect(req.request.method).toBe('GET');
    req.flush(mockSchemaResponse);
  });

  it('should send only the nodeWebName in the body for patchSchema', () => {
    //Arrange
    const updateSchema: UpdateSchema = {
      nodeWebName: 'Updated_Schema',
      publicId: '123'
    }
    const url = `${mockBaseUrl}/${updateSchema.publicId}`;

    //Act and Assert
    service.patchSchema(updateSchema).subscribe(schemaResponse =>{
      expect(service.currentSchema()).toEqual(schemaResponse); //assert currentSchema is set
    });
    
    //Verfiy
    const req = httpTestingController.expectOne(url);
    expect(req.request.method).toEqual('PATCH');
    expect(req.request.body).toEqual({ nodeWebName: 'Updated_Schema' });
    req.flush(updateSchema);
  });

  it('should set the currentSchema signal to null', () =>{
    //Arrage
    service.currentSchema.set(mockSchemaResponse);

    //Act and Assert
    expect(service.currentSchema()).toEqual(mockSchemaResponse);
    service.clearCurrentSchema();
    expect(service.currentSchema()).toBeNull();
  });

  it('should create new schema', () => {
    //Arrange
    const newSchema: RegisterSchema = {
      nodeWebName: 'Test_Schema'
    };

    //Act and Assert
    service.createSchema(newSchema).subscribe();
    
    //Verify
    const req = httpTestingController.expectOne(mockBaseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newSchema);
    req.flush(mockSchemaResponse);
  });

  it('should delete an exisitng schema', () => {
    //Arrange
    const mockId = '123';
    const url = `${mockBaseUrl}/${mockId}`;

    //Act and Assert
    service.deleteSchema(mockId).subscribe();

    //Verify
    const req = httpTestingController.expectOne(url);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

   it('should get the SchemaFullResponse for a schema and set the currentSchema signal with the id when getSchemaFull succeeds', () => {
    //Arrange
    const mockId = mockSchemaResponseFull.publicId;
    const url = `${mockBaseUrl}/${mockId}/full`;

    //Act and Assert
    expect(service.currentSchema()).toBeNull(); //assert currentSchema is null

    service.getSchemaFull(mockId).subscribe(schemaResponseFull => {
      expect(schemaResponseFull).toEqual(mockSchemaResponseFull);
      expect(service.currentSchema()).toEqual(mockSchemaResponseFull); //assert currentSchema is set
      expect(schemaResponseFull.nodes.length).toBe(2);
      expect(schemaResponseFull.edges.length).toBe(1);
    });

    //Verify
    const req = httpTestingController.expectOne(url);
    expect(req.request.method).toBe('GET');
    req.flush(mockSchemaResponseFull);
  });

});
