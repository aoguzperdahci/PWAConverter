import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { catchError } from 'rxjs';
import { AuthTokenService } from 'src/app/services/auth-token.service';
import { UIBlockService } from 'src/app/services/uiblock.service';
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
    private authTokenService: AuthTokenService,
    private router: Router,
    private UIBlockService: UIBlockService
  ) {}

  login() {
    if (this.rememberMe) {
      this.saveUserCredentials();
    }

    this.UIBlockService.blockUI();

    this.authClient
      .authenticate(this.credentials)
      .pipe(
        catchError((err) => {
          this.UIBlockService.unblockUI();
          console.log('failed'); //Todo: Replace this with toast
          throw err;
        })
      )
      .subscribe((token) => {
        this.authTokenService.saveToken(token);
        this.UIBlockService.unblockUI();
        this.router.navigateByUrl("/");
      });
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
