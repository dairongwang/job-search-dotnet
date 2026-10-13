import { Component } from '@angular/core';
import { ApiService } from '../services/api.service';
import { JobListing } from '../models/models';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.css']
})
export class SearchComponent {
  keyword = '';
  results: JobListing[] = [];
  loading = false;
  message = '';

  constructor(private api: ApiService) {}

  search() {
    this.loading = true;
    this.message = '';
    this.api.search(this.keyword || undefined).subscribe({
      next: r => { this.results = r; this.loading = false; if (r.length === 0) this.message = 'No results.'; },
      error: e => { this.loading = false; this.message = 'Search failed. Is the API running?'; }
    });
  }

  save(listing: JobListing) {
    this.api.saveFromListing(listing).subscribe({
      next: () => this.message = `Saved: ${listing.title}`,
      error: () => this.message = 'Failed to save.'
    });
  }
}
