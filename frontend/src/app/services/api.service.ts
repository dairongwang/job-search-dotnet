import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Job, JobListing, Contact, Outreach, Organization, AppConfig, DashboardStats } from '../models/models';

@Injectable({ providedIn: 'root' })
export class ApiService {
  private base = '/api';

  constructor(private http: HttpClient) {}

  // Dashboard
  getDashboard(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.base}/dashboard`);
  }

  // Jobs
  getJobs(status?: string): Observable<Job[]> {
    let params = new HttpParams();
    if (status) params = params.set('status', status);
    return this.http.get<Job[]>(`${this.base}/jobs`, { params });
  }
  createJob(job: Job): Observable<Job> {
    return this.http.post<Job>(`${this.base}/jobs`, job);
  }
  updateJob(id: number, job: Job): Observable<void> {
    return this.http.put<void>(`${this.base}/jobs/${id}`, job);
  }
  deleteJob(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/jobs/${id}`);
  }
  saveFromListing(listing: JobListing): Observable<Job> {
    return this.http.post<Job>(`${this.base}/jobs/from-listing`, listing);
  }

  // Search
  search(keyword?: string): Observable<JobListing[]> {
    let params = new HttpParams();
    if (keyword) params = params.set('keyword', keyword);
    return this.http.get<JobListing[]>(`${this.base}/search`, { params });
  }

  // Config
  getConfig(): Observable<AppConfig> {
    return this.http.get<AppConfig>(`${this.base}/config`);
  }
  saveConfig(config: AppConfig): Observable<void> {
    return this.http.put<void>(`${this.base}/config`, config);
  }

  // Contacts
  getContacts(): Observable<Contact[]> {
    return this.http.get<Contact[]>(`${this.base}/contacts`);
  }
  createContact(c: Contact): Observable<Contact> {
    return this.http.post<Contact>(`${this.base}/contacts`, c);
  }
  deleteContact(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/contacts/${id}`);
  }

  // Outreach
  getOutreach(): Observable<Outreach[]> {
    return this.http.get<Outreach[]>(`${this.base}/outreach`);
  }
  createOutreach(o: Outreach): Observable<Outreach> {
    return this.http.post<Outreach>(`${this.base}/outreach`, o);
  }
  deleteOutreach(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/outreach/${id}`);
  }

  // Organizations
  getOrganizations(): Observable<Organization[]> {
    return this.http.get<Organization[]>(`${this.base}/organizations`);
  }
  createOrganization(o: Organization): Observable<Organization> {
    return this.http.post<Organization>(`${this.base}/organizations`, o);
  }
  deleteOrganization(id: number): Observable<void> {
    return this.http.delete<void>(`${this.base}/organizations/${id}`);
  }
}
