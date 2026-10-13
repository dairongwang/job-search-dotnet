import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';
import { JobsComponent } from './jobs/jobs.component';
import { SearchComponent } from './search/search.component';
import { ConfigComponent } from './config/config.component';
import { ContactsComponent } from './contacts/contacts.component';
import { OutreachComponent } from './outreach/outreach.component';

const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'search', component: SearchComponent },
  { path: 'jobs', component: JobsComponent },
  { path: 'contacts', component: ContactsComponent },
  { path: 'outreach', component: OutreachComponent },
  { path: 'config', component: ConfigComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
