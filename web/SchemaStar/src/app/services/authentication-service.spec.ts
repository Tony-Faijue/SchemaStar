import { TestBed } from '@angular/core/testing';

import { AuthenticationService, AuthResponse, LoginUser, RegisterUser, UserResponse } from './authentication-service';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { authInterceptor } from '../http-interceptors/auth-interceptor';
import { SecretData } from '../../../environment';

describe('AuthenticationService', () => {
  let service: AuthenticationService;
  let httpTestingController: HttpTestingController

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        AuthenticationService,
        provideHttpClient(
          withInterceptors([authInterceptor]) //Auth Interceptor for each request
        ),
        provideHttpClientTesting()
      ],
    });
    service = TestBed.inject(AuthenticationService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    //ensure no http requests that were unplanned are made
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should set currentUser signal with user data when checkAuthStatus succeeds', () => {
    //Arrange
    const mockUser: AuthResponse = {
      isAuthenticated: true,
      publicId: '123',
      username: 'TestUser',
      email: 'test@example.com',
      createdAt: '2026-02-18T20:52:02-06:00',
      updatedAt: '2026-02-18T20:52:02-06:00'      
    };

    //Act
    service.checkAuthStatus().subscribe();
    //Intercept and Expect that a request to checkAuthUrl has been made
    const req = httpTestingController.expectOne(`${service['checkAuthUrl']}`);
    req.flush(mockUser);
    //Assert currentUser signal eqauls the mockUser data
    expect(service.currentUser()).toEqual(mockUser);
    expect(service.isLoggedIn()).toBeTrue();
  });

  it('should set currentUser to null when checkAuthStatus fails with 401 error', () => {
    //Act
    service.checkAuthStatus().subscribe();
    //Intercept and simulate a 401 unauthorized error
    const req = httpTestingController.expectOne(`${service['checkAuthUrl']}`);
    req.flush('Unauthorized', {status: 401, statusText: 'Unauthorized'});
    //Assert currentUser signal should be null
    expect(service.currentUser()).toBeNull();
    expect(service.isLoggedIn()).toBeFalse();
  });

  it('isLoggedIn should return true only when currentUser has a value', () => {
    //Arrange, initially null
    expect(service.isLoggedIn()).toBeFalse();

     const mockUser: AuthResponse = {
      isAuthenticated: true,
      publicId: '123',
      username: 'TestUser',
      email: 'test@example.com',
      createdAt: '2026-02-18T20:52:02-06:00',
      updatedAt: '2026-02-18T20:52:02-06:00'      
    };

    //Act
    service.currentUser.set(mockUser);
    //Assert
    expect(service.isLoggedIn()).toBeTrue();
  });

  it('should set currentUser signal with user data when loginUser succeeds', () => {
    //Arrange
    const credentials : LoginUser = {
      email: 'test@example.com',
      password: 'Password1!'
    };

    const mockResponse: AuthResponse = {
      isAuthenticated: true,
      publicId: '123',
      username: 'TestUser',
      email: 'test@example.com',
      createdAt: '2026-02-18T20:52:02-06:00',
      updatedAt: '2026-02-18T20:52:02-06:00' 
    };
   
    //Act
    service.loginUser(credentials).subscribe();
    //Intercept, expect POST request
    const req = httpTestingController.expectOne(`${SecretData.baseuUrl}/api/users/token`);
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
    //Assert
    expect(service.currentUser()).toEqual(mockResponse);
  });

  it('should set currentUser signal to null when logoutUser succeeds', () => {
      //Arrange
      const activeUser: AuthResponse = {
      isAuthenticated: true,
      publicId: '123',
      username: 'TestUser',
      email: 'test@example.com',
      createdAt: '2026-02-18T20:52:02-06:00',
      updatedAt: '2026-02-18T20:52:02-06:00' 
    };

    //Logged in user already
    service.currentUser.set(activeUser);
    //Act
    service.logoutUser().subscribe();
    //Intercept, expect POST request
    const req = httpTestingController.expectOne(`${SecretData.baseuUrl}/api/users/logout`);
    expect(req.request.method).toBe('POST');
    req.flush({ message: 'Logged out successfully'});
    //Assert
    expect(service.currentUser()).toBeNull();
    expect(service.isLoggedIn()).toBeFalse();
  });

  it('should return a UserResponse filled with user data when the registerUser succeeds', () => {
    //Arrange
    const credentials: RegisterUser = {
      username: 'TestUser',
      email: 'test@example.com',
      password: 'Password1!'    
    };

    const mockResponse: UserResponse = {
      publicId: '123',
      username: 'TestUser',
      email: 'test@example.com',
      createdAt: '2026-02-18T20:52:02-06:00',
      updatedAt: '2026-02-18T20:52:02-06:00' 
    };

    let actualResponse: UserResponse|undefined;

    //Act
    service.registerUser(credentials).subscribe(
      response => actualResponse = response //get the UserResponse 
    );
    //Intercept, expect POST request
    const req = httpTestingController.expectOne(`${SecretData.baseuUrl}/api/users`);
    expect(req.request.method).toBe('POST');
    req.flush(mockResponse);
    //Assert
    expect(actualResponse).toEqual(mockResponse);
    
    expect(service.currentUser()).toBeNull();
    expect(service.isLoggedIn()).toBeFalse();  
  });

  it('should show error when registerUser fails ', () => {
    //Arrange
     const credentials: RegisterUser = {
      username: 'TestUser',
      email: 'test@example.com',
      password: 'Password1!'    
    };

    let actualError: any;

    //Act
    service.registerUser(credentials).subscribe({
      next: () => {
        fail('An unexpected 400 error occurred');
      },
      error: (error) => {
        actualError = error;
      }
    });
    //Intercept
    const req = httpTestingController.expectOne(`${SecretData.baseuUrl}/api/users`);
    req.flush('At an error occured', {status: 400, statusText: 'Bad Request'});
    
    //Assert
    expect(actualError.status).toBe(400);
    expect(actualError.error).toBe('At an error occured');

    expect(service.currentUser()).toBeNull();
    expect(service.isLoggedIn()).toBeFalse();   
  });

});
