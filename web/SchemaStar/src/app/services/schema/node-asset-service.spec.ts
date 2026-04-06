import { TestBed } from '@angular/core/testing';

import { NodeAssetService } from './node-asset-service';

describe('NodeAssetService', () => {
  let service: NodeAssetService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(NodeAssetService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
