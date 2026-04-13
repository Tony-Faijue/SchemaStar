import { TestBed } from '@angular/core/testing';

import { EdgeRequest, EdgeResponse, EdgeService, EdgeType, UpdateEdge } from './edge-service';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { SecretData } from '../../../../environment';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from '../../http-interceptors/auth-interceptor';

describe('EdgeService', () => {
  let service: EdgeService;
  let httpTestingContoller: HttpTestingController;

  const mockBaseUrl = `${SecretData.baseuUrl}/api/edges`;

  const mockEdgeResponse: EdgeResponse = 
  {
    publicId: '123',
    uiMetadata: 'color:green',
    edgeType: EdgeType.Undirected,
    fromNodeId: '456',
    toNodeId: '789'
  }

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        EdgeService,
        provideHttpClient(
          withInterceptors([authInterceptor])
        ),
        provideHttpClientTesting()
      ],
    });
    service = TestBed.inject(EdgeService);
    httpTestingContoller = TestBed.inject(HttpTestingController);
  });

     afterEach(() => {
    //ensure no http request that were unplanned are made
    httpTestingContoller.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should get all edges for the given SchemaId when getEdges succeed', () => {
    //Arrange
    const schemaId = '389';
    const mockGetsEdgesUrl = `${SecretData.baseuUrl}/api/nodewebs/${schemaId}/edges`;
    const mockEdges: EdgeResponse[] = [mockEdgeResponse];

    //Act and Asset
    service.getEdges(schemaId).subscribe((edges) => {
      expect(edges.length).toBe(1);
      expect(edges).toEqual(mockEdges);
    });

      //Verify
      const req = httpTestingContoller.expectOne(mockGetsEdgesUrl);
      expect(req.request.method).toBe('GET');
      req.flush(mockEdges); 
  });

  it('shoudl get the specific edge with the edgeId when getEdge succeeds', () =>{
    //Arrange
    const edgeId = mockEdgeResponse.publicId;
    const url = `${mockBaseUrl}/${edgeId}`;

    //Act and Assert
    service.getEdge(edgeId).subscribe(edgeResponse => {
      expect(edgeResponse).toEqual(mockEdgeResponse);
    });

    //Verify
    const req = httpTestingContoller.expectOne(url);
    expect(req.request.method).toBe('GET');
    req.flush(mockEdgeResponse);
  });

  it('should update partially the edge with the given id when patchEdge succeeds', () => {
    //Arrange
    const updateEdge: UpdateEdge = {
      publicId: '123',
      edgeType: EdgeType.Directed,
      fromNodeId: '456',
      toNodeId: '789'
    }
    const url = `${mockBaseUrl}/${updateEdge.publicId}`;

    //Act and Asset
    service.patchEdge(updateEdge).subscribe();

    //Verify
    const req = httpTestingContoller.expectOne(url);
    expect(req.request.method).toEqual('PATCH');
    expect(req.request.body).toEqual({ edgeType: 0, fromNodeId: '456', toNodeId: '789'});
    req.flush(updateEdge);
  });

  it('should create new edge', () => {
    //Arrange
    const newEdge: EdgeRequest = {
      edgeType: EdgeType.Undirected,
      fromNodeId: '456',
      toNodeId: '789',
      uiMetadata: 'color:green',
      nodewebId: '111'
    }

    //Act and Asset
    service.createEdge(newEdge).subscribe();

    //Verify
    const req = httpTestingContoller.expectOne(mockBaseUrl);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(newEdge);
    req.flush(newEdge);
  });

  it('should delete an existing edge', () => {
    //Arrange
    const mockId = '123';
    const url = `${mockBaseUrl}/${mockId}`;

    //Act and Assert
    service.deleteEdge(mockId).subscribe();

    //Verify
    const req = httpTestingContoller.expectOne(url);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
  
});
