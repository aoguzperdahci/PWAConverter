import { Component } from '@angular/core';
import { UIBlockService } from './services/uiblock.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'Client';
  isSidebarToggled = true;
  isBlocked$ = this.UIBlockService.block$;

  constructor(private UIBlockService: UIBlockService) {

  }
}
