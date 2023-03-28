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
    MatProgressSpinnerModule
  ],
  declarations:
    [
      KijijiComponent,
      KijijiStepperComponent,
      WebViewerComponent,
      PostHistoryComponent,
      KijijiSettingComponent,
      DeviceRegistrationInfoComponent,
      KijijiChartComponent
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
