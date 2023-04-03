import { Component } from '@angular/core';
import { AuthClient, RegisterRequest } from 'src/OpenApiClient';
import { UIBlockService } from 'src/app/services/uiblock.service';
import { catchError } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css'],
})
export class SignupComponent {
  user = new RegisterRequest({ name: '', email: '', password: '' });

  constructor(
    private authClient: AuthClient,
    private router: Router,
    private UIBlockService: UIBlockService
  ) {}

  signup() {
    this.UIBlockService.blockUI();
    this.authClient
      .register(this.user)
      .pipe(
        catchError((err) => {
          this.UIBlockService.unblockUI();
          console.log('failed'); //Todo: Replace this with toast
          throw err;
        })
      )
      .subscribe(() => {
        this.UIBlockService.unblockUI();
        console.log('success'); //Todo: Replace this with toast
        this.router.navigateByUrl('/login');
      });
  }
}
