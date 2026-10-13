import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { JobsComponent } from './jobs/jobs.component';
import { SearchComponent } from './search/search.component';
import { ConfigComponent } from './config/config.component';
import { ContactsComponent } from './contacts/contacts.component';
import { OutreachComponent } from './outreach/outreach.component';

@NgModule({
  declarations: [
    AppComponent,
    DashboardComponent,
    JobsComponent,
    SearchComponent,
    ConfigComponent,
    ContactsComponent,
    OutreachComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
