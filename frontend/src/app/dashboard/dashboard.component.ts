import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { DashboardStats } from '../models/models';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {
  stats: DashboardStats | null = null;
  loading = true;

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getDashboard().subscribe({
      next: s => { this.stats = s; this.loading = false; },
      error: () => this.loading = false
    });
  }
}
