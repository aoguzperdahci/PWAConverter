import { Component } from '@angular/core';
import { RegisterRequest } from 'src/OpenApiClient';

@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent {
  user = new RegisterRequest ({name: "", email: "", password: ""});

  signup(){
    console.log(this.user);
  }
}
