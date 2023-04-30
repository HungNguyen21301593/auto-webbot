import { Injectable } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { BehaviorSubject } from 'rxjs';
import { Client, DeviceStatus } from '../../client.interface';
import { DeviceRegistrationDialogComponent } from '../device-registration-dialog/device-registration-dialog.component';

@Injectable({
  providedIn: 'root'
})
export class DeviceRegistrationInfoService {

  DeviceStatusObsererable: BehaviorSubject<DeviceStatus | null>
    = new BehaviorSubject<DeviceStatus | null>(null);
  constructor(private client: Client, private dialog: MatDialog) {
    this.refetchWithFireBase(false);
  }

  refetchWithFireBase(doCheckFireBase: boolean) {
    this.client.verify(doCheckFireBase).subscribe((deviceStatus: DeviceStatus) => {
      this.DeviceStatusObsererable.next(deviceStatus);

      if (!deviceStatus.isVerified && doCheckFireBase) {
        this.dialog.open(DeviceRegistrationDialogComponent, {
          width: "50%",
          disableClose: true
        });
      }
    })
  }
}
