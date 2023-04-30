import { MatStepperModule } from '@angular/material/stepper';
import { MatInputModule } from '@angular/material/input';
import { MatTreeModule } from '@angular/material/tree';
import { MatFormFieldModule } from '@angular/material/form-field';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { KijijiComponent } from './kijiji.component';
import { KijijiStepperComponent } from './components/kijiji-stepper/kijiji-stepper.component';
import { WebViewerComponent } from './components/web-viewer/web-viewer.component';
import { ReactiveFormsModule } from '@angular/forms';
import { PostHistoryComponent } from './components/post-history/post-history.component';
import { MatIconModule } from '@angular/material/icon';
import { KijijiSettingComponent } from './components/kijiji-setting/kijiji-setting.component';
import { DeviceRegistrationInfoComponent } from './components/device-registration-info/device-registration-info.component';
import {MatBadgeModule} from '@angular/material/badge';
import {MatSlideToggleModule} from '@angular/material/slide-toggle';
import { KijijiChartComponent } from './components/kijiji-chart/kijiji-chart.component';
import { NgxEchartsModule } from 'ngx-echarts';
import * as echarts from 'echarts';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {MatDialogModule} from '@angular/material/dialog';
import { DeviceRegistrationDialogComponent } from './components/device-registration-dialog/device-registration-dialog.component';
import {MatListModule} from '@angular/material/list';
import {MatSliderModule} from '@angular/material/slider';
import { FormsModule } from '@angular/forms';
import {MatCardModule} from '@angular/material/card';
import { SaveSettingDialogComponent } from './components/save-setting-dialog/save-setting-dialog.component';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import {MatButtonModule} from '@angular/material/button';
import {MatRadioModule} from '@angular/material/radio';
import {MatTooltipModule} from '@angular/material/tooltip';

@NgModule({
  imports: [
    CommonModule,
    MatStepperModule,
    MatFormFieldModule,
    ReactiveFormsModule,
    MatInputModule,
    MatTreeModule,
    MatIconModule,
    MatBadgeModule,
    MatSlideToggleModule,
    NgxEchartsModule.forRoot({ echarts }),
    MatProgressSpinnerModule,
    MatDialogModule,
    MatListModule,
    MatSliderModule,
    FormsModule,
    MatCardModule,
    MatSnackBarModule,
    MatButtonModule,
    MatRadioModule,
    MatTooltipModule
  ],
  declarations:
    [
      KijijiComponent,
      KijijiStepperComponent,
      WebViewerComponent,
      PostHistoryComponent,
      KijijiSettingComponent,
      DeviceRegistrationInfoComponent,
      KijijiChartComponent,
      DeviceRegistrationDialogComponent,
      SaveSettingDialogComponent
    ],
  exports: [
    KijijiStepperComponent,
    WebViewerComponent,
    PostHistoryComponent,
    KijijiSettingComponent,
    DeviceRegistrationInfoComponent,
    KijijiChartComponent
  ],
})
export class KijijiModule { }
