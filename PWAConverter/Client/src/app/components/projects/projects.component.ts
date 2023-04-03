import { Component } from '@angular/core';
import { MenuItem, MessageService, ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css'],
  providers: [MessageService, ConfirmationService],
})
export class ProjectsComponent {
  addNewDialogVisible = false;

  constructor(
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) {}

  showAddNewDialog(){
    this.addNewDialogVisible = true;
  }

}
