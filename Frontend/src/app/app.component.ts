import { Component } from '@angular/core';
import { LoginService } from './_services/login.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
	title = 'app';
	
	constructor(private loginService: LoginService) { }
}
