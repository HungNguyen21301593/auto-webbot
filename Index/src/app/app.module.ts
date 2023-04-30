import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { KijijiModule } from 'src/modules/kijiji/kijiji.module';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { HeaderComponent } from './header/header.component';
import { FooterComponent } from './footer/footer.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { BodyComponent } from './body/body.component';
import { environment } from 'src/environments/environment';
import { API_BASE_URL } from 'src/modules/kijiji/client.interface';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    BodyComponent,
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    KijijiModule,
    BrowserAnimationsModule,
    MatToolbarModule,
    MatIconModule,
    FormsModule
  ],
  providers: [{ provide: API_BASE_URL, useValue: environment.base_url },],
  bootstrap: [AppComponent]
})
export class AppModule { }
