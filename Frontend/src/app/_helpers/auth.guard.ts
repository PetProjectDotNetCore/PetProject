import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';

import { LoginService } from '../_services/login.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
	constructor(
		private router: Router,
		private loginService: LoginService
	) { }

	canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
		const user = this.loginService.userValue;
		if (user) {
			return true;
		}

		this.router.navigate(['/login'], { queryParams: { returnUrl: state.url } });
		return false;
	}
}
