/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { KijijiToastComponent } from './Kijiji-toast.component';

describe('KijijiToastComponent', () => {
  let component: KijijiToastComponent;
  let fixture: ComponentFixture<KijijiToastComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ KijijiToastComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(KijijiToastComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
