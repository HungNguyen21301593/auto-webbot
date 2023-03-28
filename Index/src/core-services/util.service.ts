import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UtilService {

  constructor() { }
  public formatLocalDisplayDate(date: Date | undefined) {
    var str = date?.toString();
    var formatedDate = new Date(str?.split("T")[0]!);
    var options: Intl.DateTimeFormatOptions = {
      weekday: 'long',
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    };
    return date!.toLocaleString("en-US", options);
  }
}
