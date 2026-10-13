import { Component, OnInit } from '@angular/core';
import { ApiService } from '../services/api.service';
import { Contact } from '../models/models';

@Component({
  selector: 'app-contacts',
  template: `
    <div class="page">
      <h2>Contacts</h2>
      <div class="form">
        <input [(ngModel)]="newContact.name" placeholder="Name" />
        <input [(ngModel)]="newContact.organization" placeholder="Organization" />
        <input [(ngModel)]="newContact.email" placeholder="Email" />
        <button (click)="add()">Add</button>
      </div>
      <ul>
        <li *ngFor="let c of contacts">
          <strong>{{ c.name }}</strong> — {{ c.organization }} {{ c.email }}
          <button (click)="remove(c)">x</button>
        </li>
      </ul>
    </div>
  `,
  styles: ['.page{padding:1rem} .form{display:flex;gap:0.5rem;margin-bottom:1rem} input{padding:0.4rem}']
})
export class ContactsComponent implements OnInit {
  contacts: Contact[] = [];
  newContact: Contact = { name: '' };
  constructor(private api: ApiService) {}
  ngOnInit() { this.load(); }
  load() { this.api.getContacts().subscribe(c => this.contacts = c); }
  add() {
    if (!this.newContact.name) return;
    this.api.createContact(this.newContact).subscribe(() => { this.newContact = { name: '' }; this.load(); });
  }
  remove(c: Contact) { if (c.id) this.api.deleteContact(c.id).subscribe(() => this.load()); }
}
