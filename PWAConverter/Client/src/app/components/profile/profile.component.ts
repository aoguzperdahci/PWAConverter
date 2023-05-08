import { Component } from '@angular/core';
import { UpdateEmailRequest } from 'src/OpenApiClient';
import { UpdatePasswordRequest, UserClient } from 'src/OpenApiClient';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent {
  passwordDialogVisible = false;
  emailDialogVisible = false;
  changePasswordModel = new UpdatePasswordRequest({password: "", newPassword: ""});
  changeEmailModel = new UpdateEmailRequest({password: "", newEmail:""});

  constructor(private userClient:UserClient){}

  user$ = this.userClient.getUser();

  showPasswordDialog(){
    this.passwordDialogVisible = true;
  }

  showEmailDialog(){
    this.emailDialogVisible = true;
  }

  deleteUserAccount(){
    this.userClient.deleteUser();
  }

}
