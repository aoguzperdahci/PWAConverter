import { Component, EventEmitter, Input, Output } from '@angular/core';
import { map } from 'rxjs';
import { Theme } from 'src/app/models/theme';
import { AuthTokenService } from 'src/app/services/auth-token.service';
import { ThemeService } from 'src/app/services/theme.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css']
})
export class SidebarComponent {

  @Input() sidebarToggle!: boolean;
  @Output() sidebarToggleChange = new EventEmitter<boolean>();
  isActiveThemeLight$ = this.themeService.activeTheme$.pipe(map(theme => theme === Theme.Light));
  isLoggedIn$ = this.authTokenService.token$.pipe(map(token => token !== ""));

  constructor(private themeService: ThemeService, private authTokenService: AuthTokenService) {
  }

  toggleSidebar(){
    this.sidebarToggle = !this.sidebarToggle;
    this.sidebarToggleChange.emit(this.sidebarToggle);
  }

  switchTheme(){
    this.themeService.switchTheme();
  }

  logOut(){
    this.authTokenService.clearToken();
  }
}
