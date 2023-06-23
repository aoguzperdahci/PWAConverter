import { Component } from '@angular/core';
import { MenuItem, MessageService, ConfirmationService } from 'primeng/api';
import { Observable, merge, tap } from 'rxjs';
import { CreateProjectModel, GetProjectResponse, ProjectClient } from 'src/OpenApiClient';
import { CacheStrategy } from 'src/app/models/cacheStrategy';
import { ProjectDetail } from 'src/app/models/projectDetail';
import { SourceMap } from 'src/app/models/sourceMap';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.css'],
  providers: [MessageService, ConfirmationService],
})
export class ProjectsComponent {
  addNewDialogVisible = false;
  newProjectName = '';
  newProjectIcon =
    'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkqAcAAIUAgUW0RjgAAAAASUVORK5CYII=';
  projects$ = this.projectService.getAll();

  constructor(
    private confirmationService: ConfirmationService,
    private messageService: MessageService,
    private projectService: ProjectClient
  ) {}

  showAddNewDialog() {
    this.addNewDialogVisible = true;
  }

  createProject() {
    const projectDetail = {
      sourceContainers: [
        {
          containerId: 0,
          name: "Ignore",
          cacheStrategy: CacheStrategy.ignore,
          rules: [],
          sourceList: [],
          sourceTree: []
        },
        {
          containerId: 1,
          name: "Pre-Cache",
          cacheStrategy: CacheStrategy.preCache,
          rules: [],
          sourceList: [],
          sourceTree: []
        }
      ],
      sourceMapList: [],
      additionalFeatures: [],
      options: {}
    } as ProjectDetail;

    this.projectService
      .create({
        name: this.newProjectName,
        file: this.newProjectIcon,
        projectDetail: JSON.stringify(projectDetail)
      } as CreateProjectModel)
      .subscribe((res) => {
        this.addNewDialogVisible = false;
        this.projects$ = this.projectService.getAll();
        this.messageService.add({
          severity: 'success',
          summary: 'Project Created',
          detail: 'Project successfully created',
        });
      });
  }

  onFileChange(event: any) {
    const reader = new FileReader();

    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsDataURL(file);

      reader.onload = () => {
        this.newProjectIcon = reader.result as string;
      };
    }
  }
}
