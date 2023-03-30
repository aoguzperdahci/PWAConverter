import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthTokenService } from './services/auth-token.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  constructor(private authTokenService: AuthTokenService){}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    if (request.url.includes('api/Auth')) {
      return next.handle(request);
    }

    request = this.addAuthenticationToken(request);
    return next.handle(request);
  }

  private addAuthenticationToken(request: HttpRequest<unknown>): HttpRequest<unknown> {
    const token = this.authTokenService.getToken();
    if (token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        },
      });
    }
    return request;
  }

}
