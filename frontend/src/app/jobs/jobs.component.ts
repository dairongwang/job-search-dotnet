import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { Job } from '../models/models';

@Component({
  selector: 'app-jobs',
  templateUrl: './jobs.component.html',
  styleUrls: ['./jobs.component.css']
})
export class JobsComponent implements OnInit {
  jobs: Job[] = [];
  filterStatus = '';
  statuses = ['Saved', 'Applied', 'Interview', 'Offer', 'Rejected', 'Closed'];

  constructor(private api: ApiService) {}

  ngOnInit() { this.load(); }

  load() {
    this.api.getJobs(this.filterStatus || undefined).subscribe(j => this.jobs = j);
  }

  updateStatus(job: Job, status: string) {
    job.status = status;
    if (job.id) this.api.updateJob(job.id, job).subscribe();
  }

  delete(job: Job) {
    if (job.id && confirm('Delete this job?')) {
      this.api.deleteJob(job.id).subscribe(() => this.load());
    }
  }
}
