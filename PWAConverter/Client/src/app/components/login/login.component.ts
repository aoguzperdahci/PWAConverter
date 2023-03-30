import { Component } from '@angular/core';
import { catchError } from 'rxjs';
import { AuthTokenService } from 'src/app/services/auth-token.service';
import { AuthClient, AuthenticateRequest } from 'src/OpenApiClient';

const REMEMBER_USER = 'remember-credentials';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent {
  credentials = this.getUserCredentials();
  rememberMe = false;

  constructor(
    private authClient: AuthClient,
    private authTokenService: AuthTokenService
  ) {}

  login() {
    if (this.rememberMe) {
      this.saveUserCredentials();
    }

    this.authClient
      .authenticate(this.credentials)
      .pipe(
        catchError((err) => {
          console.log('failed'); //Todo: Replace this with toast
          throw err;
        })
      )
      .subscribe((token) => this.authTokenService.saveToken(token));
  }

  saveUserCredentials() {
    localStorage.setItem(REMEMBER_USER, JSON.stringify(this.credentials.toJSON()));
  }

  getUserCredentials(): AuthenticateRequest {
    const credentials = localStorage.getItem(REMEMBER_USER);
    if (credentials) {
      const authenticateRequest = new AuthenticateRequest(
        JSON.parse(credentials)
      );
      this.rememberMe = true;
      return authenticateRequest;
    } else {
      return new AuthenticateRequest({ email: '', password: '' });
    }
  }
}
