import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

const AUTH_TOKEN_NAME = 'authToken';

@Injectable({
  providedIn: 'root',
})
export class AuthTokenService {
  token$ = new BehaviorSubject<string>(this.getToken());

  saveToken(token: string) {
    this.token$.next(token);
    localStorage.setItem(AUTH_TOKEN_NAME, token);
  }

  getToken(): string {
    const token = localStorage.getItem(AUTH_TOKEN_NAME);
    return token ?? '';
  }

  clearToken() {
    this.token$.next('');
    localStorage.removeItem(AUTH_TOKEN_NAME);
  }
}
