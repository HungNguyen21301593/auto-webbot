import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { MatDialog } from '@angular/material/dialog';
import { environment } from 'src/environments/environment';
import { Client, Setting } from '../../client.interface';
import { SaveSettingDialogComponent } from '../save-setting-dialog/save-setting-dialog.component';
import { TriggerMode } from './trigger-mode.enum';
import {TooltipPosition} from '@angular/material/tooltip';

@Component({
  selector: 'app-kijiji-stepper',
  templateUrl: './kijiji-stepper.component.html',
  styleUrls: ['./kijiji-stepper.component.css']
})
export class KijijiStepperComponent implements OnInit {
  registrationFormGroup = this._formBuilder.group({
    registrationCtrl: ['', Validators.required],
  });
  accountFormGroup = this._formBuilder.group({
    emailCtrl: ['', Validators.required],
    passwordCtrl: ['', Validators.required],
  });
  repostTriggerFormGroup = this._formBuilder.group({
    triggerModeControl: ['', Validators.required],
    pageToTriggerCtrl: ['', Validators.required],
    repostScheduleCtrl: ['', Validators.required],
  });
  isLinear = true;
  private setting?: Setting;
  minRepostvalue = 120;
  TriggerModeType: typeof TriggerMode = TriggerMode;
  triggerModes: TriggerMode[] = [TriggerMode.byPage, TriggerMode.bySchedule]
  selectedMode: TriggerMode = TriggerMode.byPage;

  constructor(
    private _formBuilder: FormBuilder,
    private http: HttpClient, private dialog: MatDialog) { }

  ngOnInit() {
    this.http.get<Setting>(`${environment.base_url}/api/setting/0`, { headers: { 'Access-Control-Allow-Origin': '*' } })
      .subscribe((settting: Setting) => {
        this.setting = settting;
        this.load(settting);
      });
  }

 

  load(settting: Setting) {
    this.registrationFormGroup.get("registrationCtrl")?.setValue(settting.registrationId);
    this.accountFormGroup.get("emailCtrl")?.setValue(settting.kijijiEmail);
    this.accountFormGroup.get("passwordCtrl")?.setValue(settting.kijijiPassword);
    if (settting.pageToTrigger == 0) {
      this.selectedMode = TriggerMode.bySchedule;
      this.repostTriggerFormGroup.get("repostScheduleCtrl")?.setValue(settting.readInterval! < this.minRepostvalue ? this.minRepostvalue : settting.readInterval);
    } 
    else
    {
      this.selectedMode = TriggerMode.byPage;
      this.repostTriggerFormGroup.get("pageToTriggerCtrl")?.setValue(settting.pageToTrigger);
    } 
    this.changeSelectMode();
  }

  changeSelectMode() {
    switch (this.selectedMode) {
      case TriggerMode.byPage:
        this.repostTriggerFormGroup.get("repostScheduleCtrl")?.disable();
        this.repostTriggerFormGroup.get("pageToTriggerCtrl")?.enable();
        break;
      case TriggerMode.bySchedule:
        this.repostTriggerFormGroup.get("repostScheduleCtrl")?.enable();
        this.repostTriggerFormGroup.get("pageToTriggerCtrl")?.disable();
        break;
      default:
        break;
    }
  }

  getDescriptionText(selectedMode: TriggerMode): string {
    switch (selectedMode) {
      case TriggerMode.byPage:
        return "If an ad surpasses the page, it will trigger the app to repost it."
      case TriggerMode.bySchedule:
        return "The app will repost ads based on a predetermined schedule."
      default:
        throw new Error("not supported trigger type");
    }
  }

  async submit() {
    if (!this.registrationFormGroup.valid
      || !this.accountFormGroup.valid
      || !this.repostTriggerFormGroup.valid) {
    }
    var setting = this.extractSetting();
    this.dialog.open(SaveSettingDialogComponent, {
      width: "50%",
      data: {
        setting: setting,
        selectedMode: this.selectedMode
      }
    });
  }

  private extractSetting(): Setting {
    switch (this.selectedMode) {
      case TriggerMode.byPage:
        return new Setting({
          id: this.setting?.id,
          registrationId: this.registrationFormGroup.get("registrationCtrl")?.value,
          kijijiEmail: this.accountFormGroup.get("emailCtrl")?.value,
          kijijiPassword: this.accountFormGroup.get("passwordCtrl")?.value,
          pageToTrigger: this.repostTriggerFormGroup.get("pageToTriggerCtrl")?.value,
          readInterval: 5
        });

      case TriggerMode.bySchedule:
        return new Setting({
          id: this.setting?.id,
          registrationId: this.registrationFormGroup.get("registrationCtrl")?.value,
          kijijiEmail: this.accountFormGroup.get("emailCtrl")?.value,
          kijijiPassword: this.accountFormGroup.get("passwordCtrl")?.value,
          pageToTrigger: 0,
          readInterval: this.repostTriggerFormGroup.get("repostScheduleCtrl")?.value,
        });
    }
  }
}
