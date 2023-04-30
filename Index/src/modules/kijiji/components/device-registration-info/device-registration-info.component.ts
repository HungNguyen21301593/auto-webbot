import { Component, OnDestroy, OnInit } from '@angular/core';
import { interval, Subscription } from 'rxjs';
import { UtilService } from 'src/core-services/util.service';
import { DeviceStatus } from '../../client.interface';
import { DeviceRegistrationInfoService } from './device-registration-info.service';

@Component({
  selector: 'app-device-registration-info',
  templateUrl: './device-registration-info.component.html',
  styleUrls: ['./device-registration-info.component.css']
})
export class DeviceRegistrationInfoComponent implements OnInit, OnDestroy {
  subscription?: Subscription;
  devicesubscription?: Subscription;
  deviceStatus?: DeviceStatus;

  constructor(public utilService: UtilService, 
    private deviceRegistrationInfoService: DeviceRegistrationInfoService,
    ) { }
  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
    this.devicesubscription?.unsubscribe();
  }

  ngOnInit() {
    this.deviceRegistrationInfoService.refetchWithFireBase(true);
    const source = interval(30000);
    this.subscription = source.subscribe(val => {
      this.deviceRegistrationInfoService.refetchWithFireBase(false);
    });
    this.devicesubscription = this.deviceRegistrationInfoService.DeviceStatusObsererable.subscribe((deviceStatus: DeviceStatus | null) => {
      if (!deviceStatus) {
        return;
      }
      this.deviceStatus = deviceStatus;
    })
  }

  public FormatLocalDate(date: Date | undefined) {
    var str = date?.toString();
    var formatedDate = new Date(str?.split("T")[0]!);
    var options: Intl.DateTimeFormatOptions = {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    };
    return formatedDate?.toLocaleString("en-US", options);
  }
}
