import { Component, OnInit } from '@angular/core';
import { BodyContentService } from '../service/body-content.service';
import { ContentType } from '../service/ContentType.enum';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {

  constructor(private bodyContentService: BodyContentService) { }

  ngOnInit() {
  }

  showSetting()
  {
    this.bodyContentService.contentBodyObserable.next(ContentType.KijijiSetting);
  }
  
  showChart()
  {
    this.bodyContentService.contentBodyObserable.next(ContentType.KijijiChart);
  }
}
