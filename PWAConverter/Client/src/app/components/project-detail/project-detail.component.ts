import { Component, OnInit, ViewChild } from '@angular/core';
import { MessageService, TreeDragDropService, TreeNode } from 'primeng/api';
import { SourceContainer } from 'src/app/models/sourceContainer';
import { SourceData } from 'src/app/models/sourceData';
import { CacheStrategy } from 'src/app/models/cacheStrategy';
import * as JSZip from 'jszip';
import { saveAs } from 'file-saver';
import { ServiceWorkerGenerator } from './serviceWorkerGenerator';
import { ActivatedRoute } from '@angular/router';
import {
  GetProjectResponse,
  GetSourceResponse,
  ProjectClient,
  ProjectDetailClient,
  SourceClient,
  UpdateProjectDetailModel,
} from 'src/OpenApiClient';
import { ProjectDetail } from 'src/app/models/projectDetail';
import { ResourceCollectorDialogComponent } from '../resource-collector-dialog/resource-collector-dialog.component';
import { ManifestDialogComponent } from '../manifest-dialog/manifest-dialog.component';
import { AdditionalFeatures } from 'src/app/models/additionalFeatures';

@Component({
  selector: 'app-project-detail',
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.css'],
  providers: [TreeDragDropService, MessageService],
})
export class ProjectDetailComponent implements OnInit {
  project: GetProjectResponse = {} as GetProjectResponse;
  projectDetail: ProjectDetail = {} as ProjectDetail;
  sources: GetSourceResponse[] = [];
  projectId = '';
  settingsDialogVisible = false;
  additionalFeaturesDialogVisible = false;
  selectedContainer: SourceContainer | null = null;
  cacheOptions = [
    CacheStrategy.cacheFirst,
    CacheStrategy.networkFirst,
    CacheStrategy.preCache,
    CacheStrategy.ignore,
  ];
  fallbackPageOptions = ["fallback.html", "index.html"]

  @ViewChild(ResourceCollectorDialogComponent)
  resourceCollectorDialog!: ResourceCollectorDialogComponent;
  @ViewChild(ManifestDialogComponent) manifestDialog!: ManifestDialogComponent;

  constructor(
    private route: ActivatedRoute,
    private messageService: MessageService,
    private projectService: ProjectClient,
    private projectDetailService: ProjectDetailClient,
    private sourceService: SourceClient
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.projectId = id ?? '';
    if (id) {
      this.projectService.getById(id).subscribe(res => {
        this.project = res;
      })
      this.projectDetailService.getProjectDetail(id).subscribe((res) => {
        this.projectDetail = JSON.parse(res);
        this.mapSourcesToIgnore();
      });
      this.sourceService.getSources(id).subscribe((res) => {
        this.sources = res;
        this.mapSourcesToIgnore();
      });
    }
  }

  mapSourcesToIgnore() {
    if (this.sources.length === 0 || !this.projectDetail) {
      return;
    }

    for (const source of this.sources) {
      let found = false;
      for (const container of this.projectDetail.sourceContainers) {
        if (source.url && container.sourceList.includes(source.url)) {
          found = true;
        }
      }
      if (!found && source.url) {
        this.projectDetail.sourceContainers[0].sourceList.push(source.url);
      }
    }

    for (const container of this.projectDetail.sourceContainers) {
      this.mapSourceToTree(container);
    }
  }

  mapSourceToTree(sourceContainer: SourceContainer) {
    sourceContainer.sourceTree = [];
    for (let url of sourceContainer.sourceList) {
      url =
        url[url.length - 1] === '/' ? url.substring(0, url.length - 1) : url;
      let treeNodeList = sourceContainer.sourceTree;
      let prevIndex = -1;
      let index = 7;
      while (index !== -1) {
        index = url.indexOf('/', index + 1);
        let data, label, method;

        if (index > 0) {
          data = url.substring(0, index);
          label = url.substring(prevIndex + 1, index);
        } else {
          data = url;
          label = url.substring(prevIndex + 1);
          //method
        }

        prevIndex = index;

        let foundFlag = false;
        for (const treeNode of treeNodeList) {
          if (treeNode.label === label) {
            treeNodeList = treeNode.children ?? [];
            foundFlag = true;
            break;
          }
        }
        if (!foundFlag) {
          const node = {
            label: label,
            data: {
              containerId: sourceContainer.containerId,
              url: data,
              method: method,
            },
            children: [],
          } as TreeNode<SourceData>;
          treeNodeList.push(node);
          treeNodeList = node.children ?? [];
        }
      }
    }
  }

  onDrop(event: any, containerToId: number) {
    const containerFrom =
      this.projectDetail.sourceContainers[event.dragNode.data.containerId];
    const containerTo = this.projectDetail.sourceContainers[containerToId];
    const startUrl = event.dragNode.data.url;

    if (containerFrom !== containerTo) {
      containerFrom.sourceList = containerFrom.sourceList.filter((element) => {
        if (element.startsWith(startUrl)) {
          containerTo.sourceList.push(element);
          return false;
        } else {
          return true;
        }
      });
    }

    this.mapSourceToTree(containerFrom);
    this.mapSourceToTree(containerTo);
  }

  generateServiceWorker() {
    this.mapSourceToRules();
    const codeGenerator = new ServiceWorkerGenerator();
    const sw = codeGenerator.generateSW(this.projectDetail, this.project.name);

    const zip = new JSZip();
    zip.file("sw.js", sw);
    zip.file("sw-attach.js", codeGenerator.generateSWAttach(this.projectDetail));
    if (this.projectDetail.additionalFeatures.some(feature => feature === AdditionalFeatures.OfflineFallbackPage) && this.projectDetail.options.fallbackPage !== "index.html") {
      zip.file("fallback.html", codeGenerator.fallbackPage);
    }
    zip.generateAsync({type:"blob"})
    .then((content) => {
      saveAs(content, "serviceWorker.zip");
    });
  }

  mapSourceToRules() {
    for (let i = 1; i < this.projectDetail.sourceContainers.length; i++) {
      const element = this.projectDetail.sourceContainers[i];

      for (const url of element.sourceList) {
        if (element.rules.some((r) => url.startsWith(r))) {
          continue;
        }

        let currentContainer = 0;
        let index = url.indexOf('/', 8);
        let currentRule = url.substring(0, index);
        while (currentContainer < this.projectDetail.sourceContainers.length) {
          if (currentContainer === i) {
            currentContainer++;
            continue;
          }

          const conflictFound = this.projectDetail.sourceContainers[
            currentContainer
          ].sourceList.some((u) => u.startsWith(currentRule));

          if (conflictFound) {
            index = url.indexOf('/', index + 1);
            if (index === -1) {
              currentRule = url + "$";
              break;
            } else {
              currentRule = url.substring(0, index);
              currentContainer = 0;
            }
          } else {
            if (currentContainer < this.projectDetail.sourceContainers.length) {
              currentContainer++;
            } else {
              break;
            }
          }
        }

        element.rules.push(currentRule);
      }
    }
  }

  showSettingsDialog(container: SourceContainer) {
    this.selectedContainer = container;
    this.settingsDialogVisible = true;
  }

  showAdditionalFeaturesDialog() {
    this.additionalFeaturesDialogVisible = true;
  }

  showResourceCollectorDialog() {
    this.resourceCollectorDialog.dialogVisible = true;
  }

  showManifestDialog() {
    this.manifestDialog.dialogVisible = true;
  }

  addNewContainer() {
    const newContainer = {
      name: 'New Container',
      cacheStrategy: CacheStrategy.cacheFirst,
      containerId: this.projectDetail.sourceContainers.length,
      rules: [],
      sourceList: [],
      sourceTree: [],
    } as SourceContainer;
    this.projectDetail.sourceContainers.push(newContainer);
  }

  additionalFeaturesIncludesPush(){
    if (this.projectDetail?.additionalFeatures) {
      return this.projectDetail.additionalFeatures.some(feature => feature === AdditionalFeatures.PushNotification);
    }else{
      return false;
    }
  }

  additionalFeaturesIncludesFallback(){
    if (this.projectDetail?.additionalFeatures) {
      return this.projectDetail.additionalFeatures.some(feature => feature === AdditionalFeatures.OfflineFallbackPage);
    }else{
      return false;
    }
  }


  saveProjectDetail() {
    const sourceContainer = [];
    for (const container of this.projectDetail.sourceContainers) {
      sourceContainer.push({containerId: container.containerId, name: container.name, cacheStrategy: container.cacheStrategy, sourceList: container.sourceList, rules: [], sourceTree: [], maxSize: container.maxSize })
    }

    const updateModel = {
      sourceMapList: this.projectDetail.sourceMapList,
      sourceContainers: sourceContainer,
      additionalFeatures: this.projectDetail.additionalFeatures,
      options: this.projectDetail.options
    } as ProjectDetail;

    this.projectDetailService.updateProjectDetail({
      projectId: this.projectId,
      projectDetail: JSON.stringify(updateModel)
    } as UpdateProjectDetailModel).subscribe(res =>
      this.messageService.add({
        severity: 'success',
        summary: 'Saved',
        detail: 'Changes successfully saved',
      })
    );
  }
}
