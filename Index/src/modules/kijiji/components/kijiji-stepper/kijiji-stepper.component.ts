import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { environment } from 'src/environments/environment';
import { Client, Setting } from '../../client.interface';

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
    repostTriggerCtrl: ['', Validators.required],
  });
  isLinear = false;
  private setting?: Setting;
  constructor(
    private _formBuilder: FormBuilder,
    private http: HttpClient) { }

  ngOnInit() {
    this.http.get<Setting>(`${environment.base_url}/api/setting/0`, { headers: {'Access-Control-Allow-Origin': '*'} })
    .subscribe((settting: Setting) => {
      this.setting = settting;
      this.load();
    });
  }

  load() {
    this.registrationFormGroup.get("registrationCtrl")?.setValue(this.setting?.registrationId);
    this.accountFormGroup.get("emailCtrl")?.setValue(this.setting?.kijijiEmail);
    this.accountFormGroup.get("passwordCtrl")?.setValue(this.setting?.kijijiPassword);
    this.repostTriggerFormGroup.get("repostTriggerCtrl")?.setValue(this.setting?.pageToTrigger);
  }

  async submit() {
    if (!this.registrationFormGroup.valid
      || !this.accountFormGroup.valid
      || !this.repostTriggerFormGroup.valid) {
    }
    this.setting = new Setting({
      id: this.setting?.id,
      registrationId: this.registrationFormGroup.get("registrationCtrl")?.value,
      kijijiEmail: this.accountFormGroup.get("emailCtrl")?.value,
      kijijiPassword: this.accountFormGroup.get("passwordCtrl")?.value,
      pageToTrigger: this.repostTriggerFormGroup.get("repostTriggerCtrl")?.value,
    });
    await this.http.put<any>(`${environment.base_url}/api/setting/0`, this.setting).toPromise();
  }
}
