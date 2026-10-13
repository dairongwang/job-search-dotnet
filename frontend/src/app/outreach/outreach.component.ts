import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { Outreach } from '../models/models';

@Component({
  selector: 'app-outreach',
  template: `
    <div class="page">
      <h2>Outreach Log</h2>
      <div class="form">
        <input [(ngModel)]="item.contactName" placeholder="Contact" />
        <input [(ngModel)]="item.organization" placeholder="Organization" />
        <select [(ngModel)]="item.channel">
          <option>Email</option><option>LinkedIn</option><option>Call</option><option>Other</option>
        </select>
        <input [(ngModel)]="item.subject" placeholder="Subject / note" />
        <input type="date" [(ngModel)]="item.activityDate" />
        <button (click)="add()">Log</button>
      </div>
      <table>
        <tr *ngFor="let o of items">
          <td>{{ o.activityDate }}</td>
          <td>{{ o.channel }}</td>
          <td>{{ o.contactName }}</td>
          <td>{{ o.organization }}</td>
          <td>{{ o.subject }}</td>
          <td><button (click)="remove(o)">x</button></td>
        </tr>
      </table>
    </div>
  `,
  styles: ['.page{padding:1rem} .form{display:flex;flex-wrap:wrap;gap:0.5rem;margin-bottom:1rem} input,select{padding:0.4rem} td{padding:0.3rem 0.6rem}']
})
export class OutreachComponent implements OnInit {
  items: Outreach[] = [];
  item: Outreach = { channel: 'Email', activityDate: new Date().toISOString().slice(0,10) };
  constructor(private api: ApiService) {}
  ngOnInit() { this.load(); }
  load() { this.api.getOutreach().subscribe(o => this.items = o); }
  add() {
    this.api.createOutreach(this.item).subscribe(() => {
      this.item = { channel: 'Email', activityDate: new Date().toISOString().slice(0,10) };
      this.load();
    });
  }
  remove(o: Outreach) { if (o.id) this.api.deleteOutreach(o.id).subscribe(() => this.load()); }
}
