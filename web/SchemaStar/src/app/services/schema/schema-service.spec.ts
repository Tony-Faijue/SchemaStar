import { TestBed } from '@angular/core/testing';

import { RegisterSchema, SchemaResponse, SchemaService, UpdateSchema } from './schema-service';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from '../../http-interceptors/auth-interceptor';
import { SecretData } from '../../../../environment';

describe('SchemaService', () => {
  let service: SchemaService;
  let httpTestingController: HttpTestingController

  const mockBaseUrl = `${SecretData.baseuUrl}/api/Nodewebs`;

  const mockSchemaResponse: SchemaResponse = 
    {
      publicId:'123',
      nodeWebName: 'Test_Schema',
      createdAt: '2026-02-18T20:52:02-06:00',
      updatedAt: '2026-02-18T20:52:02-06:00',
      lastLayoutAt: '2026-02-18T20:52:02-06:00'
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
    req.flush(newSchema);
  });

  it('should delete an exisitng schema', () => {
    //Arrange
    const mockId = '123';
    const url = `${mockBaseUrl}/${mockId}`

    //Act and Assert
    service.deleteSchema(mockId).subscribe();

    //Verify
    const req = httpTestingController.expectOne(url);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

});
