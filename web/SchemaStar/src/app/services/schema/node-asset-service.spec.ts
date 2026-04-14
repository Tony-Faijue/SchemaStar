import { TestBed } from '@angular/core/testing';

import { NodeAssetRequest, NodeAssetResponse, NodeAssetService, NodeAssetSource, NodeAssetType, UpdateNodeAsset } from './node-asset-service';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { SecretData } from '../../../../environment';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from '../../http-interceptors/auth-interceptor';

describe('NodeAssetService', () => {
  let service: NodeAssetService;
  let httpTestingController: HttpTestingController;

  const mockBaseUrl = `${SecretData.baseuUrl}/api/nodeassets`;

  const mockNodeAssetResponse: NodeAssetResponse = {
    publicId: '123',
    nodeAssetName: 'Test_NodeAsset',
    nodeAssetType: NodeAssetType.Audio,
    nodeAssetSource: NodeAssetSource.External,
    url: 'example.com',
    nodeId: '9898'
  }

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        NodeAssetService,
        provideHttpClient(
          withInterceptors([authInterceptor])
        ),
        provideHttpClientTesting()
      ],
    });
    service = TestBed.inject(NodeAssetService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

     afterEach(() => {
    //ensure no http request that were unplanned are made
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all node assets for the given node when getNodeAssets succeeds', () => {
    const nodeId = '6565';
    const mockGetsNodeAssetsURL = `${SecretData.baseuUrl}/api/nodes/${nodeId}/nodeassets`;
    const mockNodeAssets: NodeAssetResponse[] = [mockNodeAssetResponse];

    //Act and Assert
    service.getNodeAssets(nodeId).subscribe((nodeAssets) => {
      expect(nodeAssets.length).toBe(1);
      expect(nodeAssets).toEqual(mockNodeAssets);
    });

    //Verify
    const req = httpTestingController.expectOne(mockGetsNodeAssetsURL);
    expect(req.request.method).toBe('GET');
    req.flush(mockNodeAssets);
  });

  it('it should get the specific node asset with the nodeAssetId getNodeAsset succeeds', () => {
    //Arrange
    const nodeAssetId = mockNodeAssetResponse.publicId;
    const url = `${mockBaseUrl}/${nodeAssetId}`;

    //Act and Assert
    service.getNodeAsset(nodeAssetId).subscribe(nodeAssetResponse => {
      expect(nodeAssetResponse).toEqual(mockNodeAssetResponse);
    });

    //Verify
    const req = httpTestingController.expectOne(url);
    expect(req.request.method).toBe('GET');
    req.flush(mockNodeAssetResponse);
  });

  it('should update partially the node asset with the diven id patchNodeAsset succeeds', () => {
    //Arrange
    const updateNodeAsset: UpdateNodeAsset = {
      publicId: '123',
      nodeAssetName: 'Updated_NodeAsset',
      nodeAssetType: NodeAssetType.Image,
      url: 'example.com'
    }
    const url = `${mockBaseUrl}/${updateNodeAsset.publicId}`;

    //Act and Assert
    service.patchNodeAsset(updateNodeAsset).subscribe();

    //Verify
    const req = httpTestingController.expectOne(url);
    expect(req.request.method).toBe('PATCH');
    expect(req.request.body).toEqual({ nodeAssetName: 'Updated_NodeAsset', nodeAssetType: 0, url: 'example.com'});
    req.flush(updateNodeAsset);
  });

  it('should create a new node asset', () => {
    //Arrange
    const newNodeAsset: NodeAssetRequest = {
      nodeAssetName: 'New NodeAsset',
      nodeAssetType: NodeAssetType.Link,
      nodeAssetSource: NodeAssetSource.External,
      url: 'example.com',
      nodeId : '353535'
    }

    //Act and Assert
    service.createNodeAsset(newNodeAsset).subscribe();

    //Verify
    const req = httpTestingController.expectOne(mockBaseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newNodeAsset);
    req.flush(mockNodeAssetResponse);
  });

  it('should delete an existing node asset', () =>{
    //Arrange
    const mockId = '123';
    const url = `${mockBaseUrl}/${mockId}`;

    //Act and Assert
    service.deleteNodeAsset(mockId).subscribe();

    //Verify
    const req = httpTestingController.expectOne(url);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

});
