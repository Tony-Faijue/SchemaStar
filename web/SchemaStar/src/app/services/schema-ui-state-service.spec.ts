import { TestBed } from '@angular/core/testing';

import { SchemaUiStateService } from './schema-ui-state-service';

describe('SchemaUiStateService', () => {
  let service: SchemaUiStateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SchemaUiStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
