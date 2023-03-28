/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { KijijiChartComponent } from './kijiji-chart.component';

describe('KijijiChartComponent', () => {
  let component: KijijiChartComponent;
  let fixture: ComponentFixture<KijijiChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ KijijiChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(KijijiChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
