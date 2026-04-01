import { TestBed } from '@angular/core/testing';
import { HttpClient, HttpInterceptorFn, provideHttpClient, withInterceptors } from '@angular/common/http';

import { globalErrorInterceptor } from './global-error-interceptor';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { Router } from '@angular/router';
import { GlobalErrorNotificationService } from '../services/global-error-notification-service';

describe('globalErrorInterceptor', () => {
  let httpMock: HttpTestingController;
  let httpClient: HttpClient;
  let routerSpy: jasmine.SpyObj<Router>; //Spy is a fake version of a class; used to check methods called and arguments used
  let notificationServiceSpy: jasmine.SpyObj<GlobalErrorNotificationService>;

  beforeEach(() => {
    //Create spies for dependencies
    routerSpy = jasmine.createSpyObj('Router', ['navigate'], { url: '/home'});
    notificationServiceSpy = jasmine.createSpyObj('GlobalErrorNotificationService', ['showError']);

    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([globalErrorInterceptor])),
        provideHttpClientTesting(),
        //Inject the mocks instead of the real classes
        { provide: Router, useValue: routerSpy },
        { provide: GlobalErrorNotificationService, useValue: notificationServiceSpy }
      ],
    });

    httpClient = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should redirect to /login route on 401 error if not on login page', () => {
    //Arrange make dummy request
    httpClient.get('/test').subscribe({
      error: (err) => expect(err.status).toBe(401)
    });

    //Assert simulate a 401 response
    const req = httpMock.expectOne('/test');
    req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized' });

    //Verify the rotuer navigation is called
    expect(routerSpy.navigate).toHaveBeenCalledWith(['/login']);
    expect(notificationServiceSpy.showError).not.toHaveBeenCalled();
  });

  it('should not redirect but throw error if 401 occurs while on /login route', () => {
    //Arrange; change the mock url to login
    Object.defineProperty(routerSpy, 'url', { value: '/login'});

    httpClient.get('/test').subscribe({
      error: (err) => expect(err.status).toBe(401)
    });

    const req = httpMock.expectOne('/test');
    req.flush('Unauthorized', { status: 401, statusText: 'Unauthorized'});

    //Verify no navigation occurred
    expect(routerSpy.navigate).not.toHaveBeenCalled();
    expect(notificationServiceSpy.showError).not.toHaveBeenCalled();
  });

  it('should call notification service for 500 server errors', () => {
    //Arrange
    const errorDetail = 'Server error occured';

    httpClient.get('/test').subscribe({
      error: (err) => expect(err.status).toBe(500)
    });
    //Act and Assert
    const req = httpMock.expectOne('/test');
    req.flush({detail: errorDetail}, { status: 500, statusText: 'Internal Server Error'});

    //Verify showError was called
    expect(notificationServiceSpy.showError).toHaveBeenCalledWith(errorDetail);
    expect(routerSpy.navigate).not.toHaveBeenCalled();
  });

  it('should show default error message if error detail is missing', () => {
    //Arrange
    httpClient.get('/test').subscribe({
      error: (err) => expect(err.status).toBe(400)
    });

    //Act and Assert
    const req = httpMock.expectOne('/test');
    req.flush({}, { status: 400, statusText: 'Bad Request' });

    //Verify
    expect(notificationServiceSpy.showError).toHaveBeenCalledWith('An unexpected error occured. Please try again.');
  });
});

