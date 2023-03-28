/* tslint:disable:no-unused-variable */

import { TestBed, async, inject } from '@angular/core/testing';
import { DeviceRegistrationInfoService } from './device-registration-info.service';

describe('Service: DeviceRegistrationInfo', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [DeviceRegistrationInfoService]
    });
  });

  it('should ...', inject([DeviceRegistrationInfoService], (service: DeviceRegistrationInfoService) => {
    expect(service).toBeTruthy();
  }));
});
