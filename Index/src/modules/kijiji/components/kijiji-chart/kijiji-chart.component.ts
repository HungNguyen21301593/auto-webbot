import { ChangeDetectorRef, Component, OnDestroy, OnInit } from '@angular/core';
import { interval, Subscription } from 'rxjs';
import { ChartItemViewModel, Client } from '../../client.interface';
import { EChartsOption } from 'echarts';
import { UtilService } from 'src/core-services/util.service';
import { DeviceRegistrationInfoService } from '../device-registration-info/device-registration-info.service';
import * as _ from "lodash";


@Component({
  selector: 'app-kijiji-chart',
  templateUrl: './kijiji-chart.component.html',
  styleUrls: ['./kijiji-chart.component.css']
})
export class KijijiChartComponent implements OnInit, OnDestroy {
  subscription?: Subscription;
  chartOption: EChartsOption = {
    xAxis: {
      type: 'category',
      data: ['Mon'],
    },
    yAxis: {
      type: 'value',
    },
    series: [
      {
        data: [0],
        type: 'line',
      },
    ],
  };
  constructor(
    private client: Client,
    private utilService: UtilService,
    private deviceRegistrationInfoService: DeviceRegistrationInfoService,
    private changeref: ChangeDetectorRef
  ) { }
  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  ngOnInit() {
    this.loadChartdata();
    const source = interval(5000);
    this.subscription = source.subscribe(val => {
      this.loadChartdata();
    });
  }



  loadChartdata() {
    var today = new Date();
    var fromDate = new Date();
    fromDate.setDate(today.getDate() - 5);
    var toDate = new Date();
    toDate.setDate(today.getDate() + 1);
    var expiredDate = this.deviceRegistrationInfoService.DeviceStatusObsererable.getValue()?.deviceInfo?.expiredDate;

    this.client.chart(fromDate, toDate)
      .subscribe((chartItemViewModels: ChartItemViewModel[]) => {
        chartItemViewModels.push(...this.addEmptyFutureData(expiredDate!));
        chartItemViewModels = this.mapDateOnly(chartItemViewModels);
        chartItemViewModels = _.uniqWith(chartItemViewModels, _.isEqual);
        chartItemViewModels = chartItemViewModels.sort(function (a, b) { return a.date!.getTime() - b.date!.getTime() });
        var xAxisData = chartItemViewModels.map(model => this.utilService.formatLocalDisplayDate(model.date));
        var yAxisData = chartItemViewModels.map(model => model.value!.toString());
        var expiredFrom = xAxisData[xAxisData.lastIndexOf(this.utilService.formatLocalDisplayDate(expiredDate))];
        var expiredTo = xAxisData[xAxisData.length]
        this.chartOption = {
          tooltip: {
            trigger: 'axis',
            axisPointer: {
              type: 'line'
            },
          },
          xAxis: {
            type: 'category',
            data: xAxisData,
          },
          yAxis: {
            type: 'value',
          },
          series: [
            {
              data: yAxisData,
              type: 'bar',
              markArea:
              {
                itemStyle: {
                  color: 'rgba(255, 173, 177, 0.4)'
                },
                animationDelay:2,
                data: [
                  [
                    {
                      name: 'Expired',
                      xAxis: expiredFrom,
                    },
                    {
                      xAxis: expiredTo
                    }
                  ]
                ]
              },
            }
          ],
        };
        this.changeref.markForCheck();
      });
  }

  private addEmptyFutureData(expiredDate: Date): ChartItemViewModel[] {
    var data: ChartItemViewModel[] = [];
    var today = new Date();
    data.push(new ChartItemViewModel({
      date: expiredDate,
      value: 0
    }));
    for (let index = 0; index < 10; index++) {
      var nearDate = new Date();
      nearDate.setDate(today.getDate() + index)
      data.push(new ChartItemViewModel({ date: nearDate, value: 0 }))
    }
    for (let index = 0; index < 10; index++) {
      var afterExpired = new Date(expiredDate);
      afterExpired.setDate(expiredDate.getDate() + index)
      data.push(new ChartItemViewModel({ date: afterExpired, value: 0 }))
    }
    return data;
  }

  private mapDateOnly(data: ChartItemViewModel[]): ChartItemViewModel[] {
    var newData: ChartItemViewModel[] = [];
    data.forEach(chartItem => {
      if (!chartItem.date) {
        throw new Error("");
      }
      var date = chartItem.date.getDate();
      var Mon = chartItem.date.getMonth();
      var year = chartItem.date.getFullYear();
      newData.push(
        new ChartItemViewModel({
          date: new Date(year, Mon, date, 0, 0, 0),
          value:chartItem.value
        }))
    });
    return newData;
  }
}
