import { Injectable } from '@angular/core';

const AUTH_TOKEN_NAME = "authToken";

@Injectable({
  providedIn: 'root'
})
export class AuthTokenService {
  private token = "";

  saveToken(token: string){
    this.token = token;
    localStorage.setItem(AUTH_TOKEN_NAME, token);
  }

  getToken(): string{
    if (this.token) {
      return this.token;
    }else{
      const token = localStorage.getItem(AUTH_TOKEN_NAME);
      return token ?? "";
    }
  }

  clearToken(){
    this.token = "";
    localStorage.removeItem(AUTH_TOKEN_NAME);
  }
}
