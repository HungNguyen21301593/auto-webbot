import { Component, OnDestroy, OnInit } from '@angular/core';
import { interval, Subscription } from 'rxjs';
import { ChartItemViewModel, Client } from '../../client.interface';
import { EChartsOption } from 'echarts';
import { UtilService } from 'src/core-services/util.service';
import { DeviceRegistrationInfoService } from '../device-registration-info/device-registration-info.service';

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
  chartdata: ChartItemViewModel[] = [];
  constructor(
    private client: Client,
    private utilService: UtilService,
    private deviceRegistrationInfoService: DeviceRegistrationInfoService
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
        this.chartdata = chartItemViewModels.sort(function (a, b) { return a.date!.getTime() - b.date!.getTime() });
        var xAxisData = chartItemViewModels.map(model => this.utilService.formatLocalDisplayDate(model.date));
        var yAxisData = chartItemViewModels.map(model => model.value!.toString());
        this.chartOption = {
          tooltip: {
            trigger: 'axis',
            axisPointer: {
              type: 'cross'
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
                data: [
                  [
                    {
                      name: 'Expired',
                      xAxis: xAxisData[xAxisData.findIndex(x => x == this.utilService.formatLocalDisplayDate(expiredDate))]
                    },
                    {
                      xAxis: xAxisData[xAxisData.length]
                    }
                  ]
                ]
              }
            }
          ],
        };
      });
  }

  private addEmptyFutureData(expiredDate: Date): ChartItemViewModel[] {
    var data: ChartItemViewModel[] = [];
    data.push(new ChartItemViewModel({
      date: expiredDate,
      value: 0
    }));
    for (let index = 0; index < 30; index++) {
      var date = new Date(expiredDate);
      date.setDate(expiredDate.getDate() + index)
      data.push(new ChartItemViewModel({ date: date, value: 0 }))
    }
    return data;
  }
}
