import { Component, EventEmitter, Input, Output } from '@angular/core';
import { map } from 'rxjs';
import { Theme } from 'src/app/models/theme';
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

  constructor(private themeService: ThemeService) {
  }

  toggleSidebar(){
    this.sidebarToggle = !this.sidebarToggle;
    this.sidebarToggleChange.emit(this.sidebarToggle);
  }

  switchTheme(){
    this.themeService.switchTheme();
  }
}
