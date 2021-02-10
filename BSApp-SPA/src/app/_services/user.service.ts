import { NgIf } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../_modules/user';


@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.baseUrl;

  constructor(
    private http: HttpClient
  ) { }

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl + 'users');
  }

  getUser(id: number): Observable<User> {
    return this.http.get<User>(this.baseUrl + `users/${id}`);
  }

  updateUser(id: number, user: User): Observable<User> {
    return this.http.put<User>(this.baseUrl + `users/${id}`, user);
  }

  setMainPhoto(userId: number, id: number) {
    return this.http.post(this.baseUrl + `users/${userId}/photos/${id}/setMain`, {});
  }
}
