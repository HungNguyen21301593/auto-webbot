import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Setting } from '../../client.interface';

@Component({
  selector: 'app-kijiji-setting',
  templateUrl: './kijiji-setting.component.html',
  styleUrls: ['./kijiji-setting.component.css']
})
export class KijijiSettingComponent implements OnInit {

  
  constructor(private http: HttpClient) { }
  ngOnInit(): void {
   
  }
}
