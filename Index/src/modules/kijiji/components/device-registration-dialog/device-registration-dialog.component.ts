import { Component, OnInit } from '@angular/core';
import { UtilService } from 'src/core-services/util.service';
import { DeviceStatus } from '../../client.interface';
import { DeviceRegistrationInfoService } from '../device-registration-info/device-registration-info.service';

@Component({
  selector: 'app-device-registration-dialog',
  templateUrl: './device-registration-dialog.component.html',
  styleUrls: ['./device-registration-dialog.component.css']
})
export class DeviceRegistrationDialogComponent implements OnInit {

  constructor(public deviceRegistrationInfoService: DeviceRegistrationInfoService,
    private utilService: UtilService) { }
  public deviceStatus?: DeviceStatus | null;
  public numberOfPostValue = 30; // 5*6*1.1
  public payment = 5; // 5*6*1.1
  public min = 30;
  public max = 1000;
  ratio = (1 / 6 * (90 / 100));
  paymentUrl = "https://paypal.me/hunghung1404?country.x=VN&locale.x=en_US";
  ngOnInit() {
    this.deviceStatus = this.deviceRegistrationInfoService
      .DeviceStatusObsererable.getValue();
  }
  refresh() {
    window.location.reload();
  }

  getExpiredDate(date: Date) {
    return this.utilService.formatLocalDisplayDate(date);
  }

  updatePayment() {
    this.payment = Math.round(this.numberOfPostValue * this.ratio);
  }

  updatePosts() {
    this.numberOfPostValue = Math.round(this.payment / this.ratio);
  }

  proceed()
  {
    window.open(this.paymentUrl,'_blank')?.focus();
  }
}
