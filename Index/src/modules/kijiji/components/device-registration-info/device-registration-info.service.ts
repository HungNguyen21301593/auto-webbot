import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Client, DeviceStatus } from '../../client.interface';

@Injectable({
  providedIn: 'root'
})
export class DeviceRegistrationInfoService {

  DeviceStatusObsererable: BehaviorSubject<DeviceStatus | null>
    = new BehaviorSubject<DeviceStatus | null>(null);
  constructor(private client: Client) {
    this.refetch()
   }

  refetch() {
    this.client.verify().subscribe((deviceStatus: DeviceStatus) => {
      this.DeviceStatusObsererable.next(deviceStatus);
    })
  }
}
