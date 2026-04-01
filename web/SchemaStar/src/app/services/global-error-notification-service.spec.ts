import { TestBed } from '@angular/core/testing';

import { GlobalErrorNotificationService } from './global-error-notification-service';

describe('GlobalErrorNotification', () => {
  let service: GlobalErrorNotificationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GlobalErrorNotificationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should set errorMessage for showError', () =>{
    //Arrange
    const message: string = 'A global error has occured';

    //Act and Assert
    service.showError(message);
    expect(service.errorMessage()).toEqual(message);
  });

  it('should set errorMessage to null for clearError', () =>{
    //Arrange
    const message: string = 'A global error has occured';
    service.showError(message);
    expect(service.errorMessage()).toEqual(message);

    //Act and Assert
    service.clearError();
    expect(service.errorMessage()).toBe(null);
  });
});
