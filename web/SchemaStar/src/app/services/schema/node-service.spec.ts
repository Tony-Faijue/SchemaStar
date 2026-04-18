import { TestBed } from '@angular/core/testing';

import { NodeRequest, NodeResponse, NodeResponseFull, NodeService, NodeState, UpdateNode } from './node-service';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { SecretData } from '../../../../environment';
import { NodeAssetResponse, NodeAssetSource, NodeAssetType } from './node-asset-service';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from '../../http-interceptors/auth-interceptor';

describe('NodeService', () => {
  let service: NodeService;
  let httpTestingController: HttpTestingController;

  const mockBaseUrl = `${SecretData.baseuUrl}/api/nodes`;

  const mockNodeResponse: NodeResponse = 
  {
    publicId: '123',
    nodeName: 'Test_Node',
    positionX: 0,
    positionY: 0,
    width: 150,
    height: 200,
    state: NodeState.Locked,
    createdAt: '2026-02-18T20:52:02-06:00',
    nodeWebId: '9999'
  }

  const myAsset1: NodeAssetResponse = {
    publicId: '909',
    nodeAssetName: 'asset_1',
    nodeAssetType: NodeAssetType.Link,
    nodeAssetSource: NodeAssetSource.External,
    nodeId: mockNodeResponse.publicId
  }

  const myAsset2: NodeAssetResponse = {
    publicId: '908',
    nodeAssetName: 'asset_2',
    nodeAssetType: NodeAssetType.Audio,
    nodeAssetSource: NodeAssetSource.Upload,
    nodeId: mockNodeResponse.publicId
  }

  const myAssets:NodeAssetResponse[] = [myAsset1, myAsset2];

 
  const mockNodeResponseFull: NodeResponseFull = 
  {
    publicId: '123',
    nodeName: 'Test_Node',
    positionX: 0,
    positionY: 0,
    width: 150,
    height: 200,
    state: NodeState.Locked,
    createdAt: '2026-02-18T20:52:02-06:00',
    NodeAssets: myAssets,
    nodeWebId: '9999'
  }

  const nodeUpdate1: UpdateNode = {
    publicId: '1010',
    nodeName: 'nodeUpdate1' 
  }

  const nodeUpdate2: UpdateNode = {
    publicId: '1020',
    nodeName: 'nodeUpdate2' 
  }

  const nodesToUpdate: UpdateNode[] = [nodeUpdate1, nodeUpdate2];

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        NodeService,
        provideHttpClient(
          withInterceptors([authInterceptor])
        ),
        provideHttpClientTesting()
      ],
    });
    service = TestBed.inject(NodeService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

   afterEach(() => {
    //ensure no http request that were unplanned are made
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all nodes for the given SchemaId when getNodes succeds', () => {
    //Arrange
      const schemaId = '389';
      const mockGetsNodesURL = `${SecretData.baseuUrl}/api/nodewebs/${schemaId}/nodes`;
      const mockNodes: NodeResponse[] = [mockNodeResponse];

      //Act and Assert
      service.getNodes(schemaId).subscribe((nodes) => {
        expect(nodes.length).toBe(1);
        expect(nodes).toEqual(mockNodes);
      });

      //Verify
      const req = httpTestingController.expectOne(mockGetsNodesURL);
      expect(req.request.method).toBe('GET');
      req.flush(mockNodes);
  });

  it('should get the specific node with the nodeId when getNode succeeds', () => {
    //Arrange
    const nodeId = mockNodeResponse.publicId;
    const url = `${mockBaseUrl}/${nodeId}`;

    //Act and Assert
    service.getNode(nodeId).subscribe(nodeResponse =>{
      expect(nodeResponse).toEqual(mockNodeResponse);
    });

    //Verify
    const req = httpTestingController.expectOne(url);
    expect(req.request.method).toBe('GET');
    req.flush(mockNodeResponse);
  });

  it('should update partially the node with the given id when patchNode succeeds', () => {
    //Arrange 
    const updateNode: UpdateNode = {
      publicId: '123',
      nodeName: 'Updated_Node',
      positionX: 10,
      state: NodeState.Unlocked
    }
    const url = `${mockBaseUrl}/${updateNode.publicId}`;

    //Act and Assert
    service.patchNode(updateNode).subscribe();

    //Verify
    const req = httpTestingController.expectOne(url);
    expect(req.request.method).toEqual('PATCH');
    expect(req.request.body).toEqual({ nodeName: 'Updated_Node', positionX: 10, state: 1});
    req.flush(updateNode);
  });

  it('should create new node', () => {
    //Arrange
    const newNode: NodeRequest = {
      nodeName: 'New Node',
      positionX: 10,
      positionY: 20,
      width: 300,
      height: 500,
      state: NodeState.Unlocked,
      nodewebId: '133535'
    }
    
    //Act and Assert
    service.createNode(newNode).subscribe();

    //Verify 
    const req = httpTestingController.expectOne(mockBaseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newNode);
    req.flush(mockNodeResponse);
  });

  it('should delete an existing node', () =>{
    //Arrange
    const mockId = '123';
    const url = `${mockBaseUrl}/${mockId}`;

    //Act and Assert
    service.deleteNode(mockId).subscribe();

    //Verify 
    const req = httpTestingController.expectOne(url);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  it('should get NodeResponseFull for node given the id when getNodeFull succeeds', () => {
    //Arrange
    const nodeId = mockNodeResponseFull.publicId;
    const url = `${mockBaseUrl}/${nodeId}/full`;

    //Act and Assert
    service.getNodeFull(nodeId).subscribe(nodeResponseFull =>{
      expect(nodeResponseFull).toEqual(mockNodeResponseFull);
    });

    //Verify
    const req = httpTestingController.expectOne(url);
    expect(req.request.method).toBe('GET');
    req.flush(mockNodeResponseFull);
  });

  it('should update partially the nodes when given the node ids and the Schema id when bulkUpdateNodes succeeds', () => {
    //Arrange
    const schemaId = '555';
    const mockNodesBulkURL = `${SecretData.baseuUrl}/api/nodewebs/${schemaId}/nodes/bulk`;

    //Act and Assert
    service.bulkUpdateNodes(schemaId, nodesToUpdate).subscribe();

    //Verify
    const req = httpTestingController.expectOne(mockNodesBulkURL);

    expect(req.request.method).toBe('PATCH');
    expect(req.request.body).toEqual(nodesToUpdate);
    req.flush(null);
  });

  it('should delete the nodes when given the node ids and the Schema id when bulkDeleteNodes succeeds', () => {
    //Arrange
    const schemaId = '555';
    const nodeIds: string[] = ['1', '12', '16', '78'];
    const mockNodesBulkURL = `${SecretData.baseuUrl}/api/nodewebs/${schemaId}/nodes/bulk`;

    //Act and Assert
    service.bulkDeleteNodes(schemaId, nodeIds).subscribe();

    //Verify
    const req = httpTestingController.expectOne(mockNodesBulkURL);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
  
});
