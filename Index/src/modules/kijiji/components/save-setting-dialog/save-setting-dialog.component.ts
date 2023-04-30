import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarHorizontalPosition, MatSnackBarVerticalPosition } from '@angular/material/snack-bar';
import { environment } from 'src/environments/environment';
import { Setting } from '../../client.interface';
import { Client } from '../../client.interface';
import { KijijiToastComponent } from '../Kijiji-toast/Kijiji-toast.component';
import { TriggerMode } from '../kijiji-stepper/trigger-mode.enum';

@Component({
  selector: 'app-save-setting-dialog',
  templateUrl: './save-setting-dialog.component.html',
  styleUrls: ['./save-setting-dialog.component.css']
})
export class SaveSettingDialogComponent implements OnInit {
  setting?: Setting;
  horizontalPosition: MatSnackBarHorizontalPosition = 'start';
  verticalPosition: MatSnackBarVerticalPosition = 'bottom';
  selectedMode?: TriggerMode;
  TriggerModeType: typeof TriggerMode = TriggerMode;
  
  constructor(@Inject(MAT_DIALOG_DATA) public data: any,
    public dialogRef: MatDialogRef<SaveSettingDialogComponent>,
    private client: Client,
    private snackBar: MatSnackBar) {
    this.setting = data.setting;
    this.selectedMode = data.selectedMode;
  }

  ngOnInit() {
  }

  onNoClick() {
    this.dialogRef.close();
  }

  async save() {
    
    this.dialogRef.close();
    await this.client.settingPUT(0, this.setting).subscribe(() => {
      this.snackBar.open('The new setting has been saved successfully and will take effect during the next application flow, this process will usually take 5 to 10 mininutes. Thank you for your patience.',"Got it!", {
        horizontalPosition: this.horizontalPosition,
        verticalPosition: this.verticalPosition,
        duration: 10000
      });
      this.dialogRef.close();
    });
  }
}
