import { Component, OnInit } from '@angular/core';
import { LoginService } from '../_services/login.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

	constructor(private loginService: LoginService) { }

  ngOnInit(): void {
	}
	
	refreshToken() {
		this.loginService.refreshToken()
			.subscribe(
				success => {
					if (success) {
						alert('OK');
					}
				},
				error => {
					console.log(error)
				});
}

	logout() {
		this.loginService.logout();
	}
}
