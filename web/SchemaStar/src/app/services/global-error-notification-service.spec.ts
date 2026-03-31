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
});
