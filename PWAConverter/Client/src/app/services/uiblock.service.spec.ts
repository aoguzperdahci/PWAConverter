import { TestBed } from '@angular/core/testing';

import { UIBlockService } from './uiblock.service';

describe('UIBlockService', () => {
  let service: UIBlockService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(UIBlockService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
