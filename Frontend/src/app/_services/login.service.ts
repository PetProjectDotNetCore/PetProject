import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { User}  from '../_models/user.model'
import { BehaviorSubject, Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';

import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class LoginService {

	private userSubject: BehaviorSubject<User>;
	private token: String;

	constructor(
		private router: Router,
		private http: HttpClient
	) {
		this.userSubject = new BehaviorSubject<User>(JSON.parse(localStorage.getItem('user')));
	}

	public get userValue(): User {
		return this.userSubject.value;
	}

	login(email, password): Observable<Boolean> {
		return this.http.post<User>(`${environment.apiUrl}/login/authenticate`, { email, password }, { withCredentials: true })
			.pipe(map(user => {
				this.token = user.token;
				user.token = '';
				localStorage.setItem('user', JSON.stringify(user));
				this.userSubject.next(user);
				return true;
			}));
	}

	refreshToken(): Observable<Boolean> {
		var refreshTokenExpires = new Date(this.userSubject.value.expires);
		if (!this.token || refreshTokenExpires < new Date(Date.now())) {
			this.logout();
			return of(false);
		}

		return this.http.post<User>(`${environment.apiUrl}/login/refresh`, { accessToken: this.token }, { withCredentials: true })
			.pipe(map(dto => {
				this.token = dto.token;
				var user = this.userSubject.value;
				user.expires = dto.expires;
				localStorage.setItem('user', JSON.stringify(user));
				this.userSubject.next(user);
				return true;
			}));
	}

	logout() {
		localStorage.removeItem('user');
		this.userSubject.next(null);
		this.router.navigate(['/login'])
	}
}
