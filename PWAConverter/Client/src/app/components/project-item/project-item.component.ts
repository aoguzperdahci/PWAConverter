import { HttpEvent } from '@angular/common/http';
import { Component, ViewChild } from '@angular/core';
import { MenuItem, MessageService, ConfirmationService } from 'primeng/api';
import { EditDialogComponent } from '../edit-dialog/edit-dialog.component';

@Component({
  selector: 'app-project-item',
  templateUrl: './project-item.component.html',
  styleUrls: ['./project-item.component.css'],
  providers: [MessageService, ConfirmationService],
})
export class ProjectItemComponent {
  manifestDialogVisible = false;
  resourceCollectorDialogVisible = false;
  value = "";
  options = ["1111111", "2222222", "33333333"]

  @ViewChild(EditDialogComponent) editDialog!: EditDialogComponent;

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

  showResourceCollectorDialog() {
    this.resourceCollectorDialogVisible = true;
  }


  showEditDialog(){
    this.editDialog.dialogVisible = true;
  }

  showManifestDialog(){
    this.manifestDialogVisible = true;
  }

}
