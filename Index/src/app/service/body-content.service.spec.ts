/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { BodyContentService } from './body-content.service';

describe('Service: BodyContent', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [BodyContentService]
    });
  });

  it('should ...', inject([BodyContentService], (service: BodyContentService) => {
    expect(service).toBeTruthy();
  }));
});
