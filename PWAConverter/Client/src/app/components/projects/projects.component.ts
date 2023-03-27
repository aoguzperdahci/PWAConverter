import { Component } from '@angular/core';
import { MenuItem, MessageService, ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css'],
  providers: [MessageService, ConfirmationService],
})
export class ProjectsComponent {
  items: MenuItem[] = [];

  constructor(
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.items = [
      {
        icon: 'pi pi-pencil',
        command: () => {
          this.messageService.add({
            severity: 'info',
            summary: 'Add',
            detail: 'Data Added',
          });
        },
      },
      {
        icon: 'pi pi-refresh',
        command: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Update',
            detail: 'Data Updated',
          });
        },
      },
      {
        icon: 'pi pi-trash',
        command: () => {
          this.messageService.add({
            severity: 'error',
            summary: 'Delete',
            detail: 'Data Deleted',
          });
        },
      },
    ];
  }

  confirm(event: Event) {
    if (event.target) {
      this.confirmationService.confirm({
        target: event.target,
        message: 'Are you sure that you want to proceed?',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
          this.messageService.add({
            severity: 'info',
            summary: 'Confirmed',
            detail: 'You have accepted',
          });
        },
        reject: () => {
          this.messageService.add({
            severity: 'error',
            summary: 'Rejected',
            detail: 'You have rejected',
          });
        },
      });
    }
  }
}
