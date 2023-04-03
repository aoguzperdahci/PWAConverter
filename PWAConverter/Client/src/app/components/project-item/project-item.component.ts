import { HttpEvent } from '@angular/common/http';
import { Component } from '@angular/core';
import { MenuItem, MessageService, ConfirmationService } from 'primeng/api';

@Component({
  selector: 'app-project-item',
  templateUrl: './project-item.component.html',
  styleUrls: ['./project-item.component.css'],
  providers: [MessageService, ConfirmationService],
})
export class ProjectItemComponent {
  editDialogVisible = false;
  manifestDialogVisible = false;
  value = "";
  options = ["1111111", "2222222", "33333333"]

  constructor(
    private confirmationService: ConfirmationService,
    private messageService: MessageService
  ) {}

  confirmDelete(event: Event) {
    if (event.target) {
      this.confirmationService.confirm({
        target: event.target,
        message: 'Are you sure that you want to delete?',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Confirmed',
            detail: 'Project deleted',
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

  confirmResourceCollector(event: Event) {
    if (event.target) {
      this.confirmationService.confirm({
        target: event.target,
        message: 'Are you sure that you want to generate resource collector?',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Confirmed',
            detail: 'Resource collector generated',
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


  showEditDialog(){
    this.editDialogVisible = true;
  }

  showManifestDialog(){
    this.manifestDialogVisible = true;
  }

}
