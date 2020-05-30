import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { User}  from '../_models/user.model'
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class LoginService {

	private userSubject: BehaviorSubject<User>;
	public user: Observable<User>;

	constructor(
		private router: Router,
		private http: HttpClient
	) {
		this.userSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('user')));
		this.user = this.userSubject.asObservable();
	}

	public get userValue(): User {
		return this.userSubject.value;
	}

	login(email, password) {
		return this.http.post<User>(`${environment.apiUrl}/login/authenticate`, { email, password })
			.pipe(map(user => {
				localStorage.setItem('user', JSON.stringify(user));
				this.userSubject.next(user);
				return user;
			}));
	}

	logout() {
		localStorage.removeItem('user');
		this.userSubject.next(null);
		this.router.navigate(['/login'])
	}
}
