import { Component } from '@angular/core';
import { TreeDragDropService, TreeNode } from 'primeng/api';
import { SourceContainer } from 'src/app/models/sourceContainer';
import { SourceData } from 'src/app/models/sourceData';
import { CacheStrategy } from "src/app/models/cacheStrategy";

@Component({
  selector: 'app-project-detail',
  templateUrl: './project-detail.component.html',
  styleUrls: ['./project-detail.component.css'],
  providers: [TreeDragDropService],
})
export class ProjectDetailComponent {
  urls1 = [
    'https://www.youtube.com/10000/20000/',
    'https://www.youtube.com/10000/20001',
    'https://www.youtube.com/10000/20002',
    'https://www.youtube.com/10000/20003',
    'https://www.youtube.com/10000/20004',
    'https://www.youtube.com/10000/20006',
    'https://www.youtube.com/10000/20007',
    'https://www.youtube.com/10000/20008',
    'https://www.youtube.com/10001/20000',
    'https://www.youtube.com/10000/20005',
    'https://www.youtube.com/10000/20000/30001',
    'https://www.youtube.com/10001/20001',
    'https://www.youtube.com/10001/20002',
    'https://www.youtube.com/10001/20003',
    'https://www.youtube.com/10001/20004',
    'https://www.youtube.com/10001/20005',
    'https://www.youtube.com/10001/20006',
    'https://www.youtube.com/10000/20000/30002',
    'https://www.youtube.com/10001/20007',
    'https://www.youtube.com/10001/20008',
    'https://www.youtube.com/10000/20000/30000',
    'https://www.youtube.com/10000/20000/30003',
    'https://www.youtube.com/10000/20000/30004',
    'https://www.youtube.com/10000/20001/30000',
    'https://www.youtube.com/10000/20001/30001',
    'https://www.youtube.com/10000/20001/30002',
    'https://www.youtube.com/10000/20001/30003',
    'https://www.youtube.com/10000/20001/30004',
    // "http://localhost:4200/",
    "http://localhost:4200/10000"
  ];

  urls2 = [
    'https://www.youtube.com/11000/20000/',
    'https://www.youtube.com/11000/20001',
    'https://www.youtube.com/11000/20002',
    'https://www.youtube.com/11000/20003',
    'https://www.youtube.com/11000/20004',
    'https://www.youtube.com/11000/20006',
    'https://www.youtube.com/11000/20007',
    'https://www.youtube.com/11000/20008',
    'https://www.youtube.com/11001/20000',
    'https://www.youtube.com/11000/20005',
    'https://www.youtube.com/11000/20000/30001',
    'https://www.youtube.com/11001/20001',
    'https://www.youtube.com/11001/20002',
    'https://www.youtube.com/11001/20003',
    'https://www.youtube.com/11001/20004',
    'https://www.youtube.com/11001/20005',
    'https://www.youtube.com/11001/20006',
    'https://www.youtube.com/11000/20000/30002',
    'https://www.youtube.com/11001/20007',
    'https://www.youtube.com/11001/20008',
    'https://www.youtube.com/11000/20000/30000',
    'https://www.youtube.com/11000/20000/30003',
    'https://www.youtube.com/11000/20000/30004',
    'https://www.youtube.com/11000/20001/30000',
    'https://www.youtube.com/11000/20001/30001',
    'https://www.youtube.com/11000/20001/30002',
    'https://www.youtube.com/11000/20001/30003',
    'https://www.youtube.com/11000/20001/30004'
  ];


  containers: SourceContainer[] = [];

  constructor(){
    const container1 = {name: "v1", containerId: 0, cacheStrategy: CacheStrategy.cacheFirst, sourceList: this.urls1, sourceTree: []} as SourceContainer;
    const container2 = {name: "v2", containerId: 1, cacheStrategy: CacheStrategy.cacheFirst, sourceList: this.urls2, sourceTree: []} as SourceContainer;
    this.mapSourceToTree(container1);
    this.mapSourceToTree(container2);
    this.containers.push(container1);
    this.containers.push(container2);

  }

  mapSourceToTree(sourceContainer: SourceContainer){
    sourceContainer.sourceTree = [];
    for (let url of sourceContainer.sourceList) {
      url = url[url.length - 1] === '/' ? url.substring(0, url.length - 1) : url;
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
            data: {containerId: sourceContainer.containerId, url: data, method: method},
            children: [],
          } as TreeNode<SourceData>;
          treeNodeList.push(node);
          treeNodeList = node.children ?? [];
        }
      }
    }
  }

  onDrop(event: any, containerToId: number) {
    const containerFrom = this.containers[event.dragNode.data.containerId];
    const containerTo = this.containers[containerToId];
    const startUrl = event.dragNode.data.url;

    containerFrom.sourceList = containerFrom.sourceList.filter(element => {
      if (element.startsWith(startUrl)) {
        containerTo.sourceList.push(element);
        return false;
      } else {
        return true;
      }
    });

    this.mapSourceToTree(containerFrom);
    this.mapSourceToTree(containerTo);
  }
}
