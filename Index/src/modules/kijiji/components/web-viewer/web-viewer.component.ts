import { Component, OnDestroy, OnInit } from '@angular/core';
import { Client } from '../../client.interface';
import { Subscription, interval } from 'rxjs';
import { environment } from 'src/environments/environment';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';

@Component({
  selector: 'app-web-viewer',
  templateUrl: './web-viewer.component.html',
  styleUrls: ['./web-viewer.component.css']
})
export class WebViewerComponent implements OnInit, OnDestroy {
  subscription?: Subscription;
  data!: string;
  srcUrl!: SafeResourceUrl; 
  constructor(private client: Client, public sanitizer:DomSanitizer) { }
  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  ngOnInit() {
    this.srcUrl = this.sanitizer.bypassSecurityTrustResourceUrl(`${environment.base}:7900/?autoconnect=1&resize=scale&password=secret`);
  }
}
