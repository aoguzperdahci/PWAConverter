import { HttpEvent } from '@angular/common/http';
import { Component, Input, ViewChild } from '@angular/core';
import { MenuItem, MessageService, ConfirmationService } from 'primeng/api';
import { EditDialogComponent } from '../edit-dialog/edit-dialog.component';
import { ManifestDialogComponent } from '../manifest-dialog/manifest-dialog.component';
import { ResourceCollectorDialogComponent } from '../resource-collector-dialog/resource-collector-dialog.component';
import { GetProjectResponse, ProjectClient } from 'src/OpenApiClient';

@Component({
  selector: 'app-project-item',
  templateUrl: './project-item.component.html',
  styleUrls: ['./project-item.component.css'],
  providers: [MessageService, ConfirmationService],
})
export class ProjectItemComponent {
  @Input() project!: GetProjectResponse;
  @ViewChild(EditDialogComponent) editDialog!: EditDialogComponent;
  @ViewChild(ManifestDialogComponent) manifestDialog!: ManifestDialogComponent;
  @ViewChild(ResourceCollectorDialogComponent)
  resourceCollectorDialog!: ResourceCollectorDialogComponent;
  projectIcon = '';

  constructor(
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private projectService: ProjectClient
  ) {}

  confirmDelete(event: Event) {
    if (event.target) {
      this.confirmationService.confirm({
        target: event.target,
        message: 'Are you sure that you want to delete?',
        icon: 'pi pi-exclamation-triangle',
        accept: () => {
          this.projectService.delete(this.project.id).subscribe(res => {
            this.messageService.add({
              severity: 'success',
              summary: 'Confirmed',
              detail: 'Project deleted',
            });
          })
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
    this.resourceCollectorDialog.dialogVisible = true;
  }

  showEditDialog() {
    this.editDialog.dialogVisible = true;
  }

  showManifestDialog() {
    this.manifestDialog.dialogVisible = true;
  }
}
