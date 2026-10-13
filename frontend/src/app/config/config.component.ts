import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { AppConfig, SiteConfig } from '../models/models';

@Component({
  selector: 'app-config',
  templateUrl: './config.component.html',
  styleUrls: ['./config.component.css']
})
export class ConfigComponent implements OnInit {
  config: AppConfig | null = null;
  titlesText = '';
  skillsText = '';
  message = '';

  constructor(private api: ApiService) {}

  ngOnInit() {
    this.api.getConfig().subscribe(c => {
      this.config = c;
      this.titlesText = (c.preferences.titles || []).join(', ');
      this.skillsText = (c.preferences.skills || []).join(', ');
    });
  }

  save() {
    if (!this.config) return;
    this.config.preferences.titles = this.titlesText.split(',').map(s => s.trim()).filter(Boolean);
    this.config.preferences.skills = this.skillsText.split(',').map(s => s.trim()).filter(Boolean);
    this.api.saveConfig(this.config).subscribe({
      next: () => this.message = 'Configuration saved.',
      error: () => this.message = 'Save failed.'
    });
  }

  toggleSite(site: SiteConfig) {
    site.enabled = !site.enabled;
  }
}
