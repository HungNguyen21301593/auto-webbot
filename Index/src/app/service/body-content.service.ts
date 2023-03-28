import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { ContentType } from './ContentType.enum';

@Injectable({
  providedIn: 'root'
})
export class BodyContentService {
  contentBodyObserable: BehaviorSubject<ContentType> = new BehaviorSubject<ContentType>(ContentType.KijijiSetting);
  constructor() { this.contentBodyObserable.value}

}
