import { Inject, Injectable } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { Theme } from '../models/theme';
import { BehaviorSubject } from 'rxjs';

const THEME_KEY = 'active-theme';

@Injectable({
  providedIn: 'root',
})
export class ThemeService {
  activeTheme$: BehaviorSubject<Theme>;

  constructor(@Inject(DOCUMENT) private document: Document) {
    this.activeTheme$ = new BehaviorSubject(this.getThemeOrDefault());
    this.activeTheme$.subscribe((theme) => this.setTheme(theme));
  }

  switchTheme() {
    let theme = this.getThemeOrDefault();
    if (theme === Theme.Light) {
      theme = Theme.Dark;
    }else{
      theme = Theme.Light
    }
    this.activeTheme$.next(theme);
    localStorage.setItem(THEME_KEY, theme);
  }

  getThemeOrDefault(): Theme {
    let theme = localStorage.getItem(THEME_KEY);
    if (!theme) {
      theme = Theme.Light;
    }
    return theme as Theme;
  }

  setTheme(theme: Theme) {
    const themeLink = this.document.getElementById(
      'app-theme'
    ) as HTMLLinkElement;
    themeLink.href = theme;

  }
}
