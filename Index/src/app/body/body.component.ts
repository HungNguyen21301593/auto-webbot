import { Component, OnInit } from '@angular/core';
import { BodyContentService } from '../service/body-content.service';
import { ContentType } from '../service/ContentType.enum';

@Component({
  selector: 'app-body',
  templateUrl: './body.component.html',
  styleUrls: ['./body.component.css']
})
export class BodyComponent implements OnInit {

  public Type: typeof ContentType = ContentType;
  constructor(public bodyContentService: BodyContentService) {
    this.bodyContentService.contentBodyObserable.value
   }

  ngOnInit() {
  }

}
