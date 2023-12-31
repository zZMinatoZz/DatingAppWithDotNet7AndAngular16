import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  registerMode: boolean = false;
  users: any;

  constructor() {}

  ngOnInit(): void {
    // this.getUsers();
  }

  // getUsers() {
  //   this.http.get('https://localhost:5001/api/users').subscribe({
  //     // if no error happen
  //     next: (response) => (this.users = response),
  //     // handle error if happen
  //     error: (error) => console.log(error),
  //     // when request complete
  //     complete: () => console.log('Request has completed'),
  //   });
  // }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  cancelRegisterMode(event: boolean) {
    this.registerMode = event;
  }
}
