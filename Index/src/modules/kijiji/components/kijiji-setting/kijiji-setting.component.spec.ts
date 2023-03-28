/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';

import { KijijiSettingComponent } from './kijiji-setting.component';

describe('KijijiSettingComponent', () => {
  let component: KijijiSettingComponent;
  let fixture: ComponentFixture<KijijiSettingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ KijijiSettingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(KijijiSettingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
