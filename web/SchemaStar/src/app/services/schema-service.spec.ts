import { TestBed } from '@angular/core/testing';

import { SchemaService } from './schema-service';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { authInterceptor } from '../http-interceptors/auth-interceptor';

describe('SchemaService', () => {
  let service: SchemaService;
  let httpTestingController: HttpTestingController

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

  
});
