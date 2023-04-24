import { Component } from '@angular/core';
import { TreeDragDropService, TreeNode } from 'primeng/api';
import { SourceData } from 'src/app/models/sourceData';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
  providers: [TreeDragDropService],
})
export class HomeComponent {
  urls = [
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
    "http://localhost:4200/",
    "http://localhost:4200/10000"
  ];

  rootNodes: TreeNode<SourceData>[] = [];

  constructor() {
    const t = this.urls.map((url) => {
      const index = url.indexOf('/', 8);
      const host = url.substring(0, index);
      const parts = url.split('/');
      parts.shift();
      parts.shift();
      // parts.unshift(host);
      return parts;
    });
    // console.log(t);
    // this.urls.sort();

    const y = this.urls.map((url) => {
      const index = url.indexOf('/', 8);
      const host = url.substring(0, index);
      return host;
    });

    for (let url of this.urls) {
      const container = 1;
      url = url[url.length - 1] === '/' ? url.substring(0, url.length - 1) : url;
      let treeNodeList = this.rootNodes;
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
            data: {containerId: container, url: data, method: method},
            children: [],
          } as TreeNode<SourceData>;
          treeNodeList.push(node);
          treeNodeList = node.children ?? [];
        }
      }
    }
    console.log(this.rootNodes);
  }
  files: TreeNode[] = [
    {
      key: '0',
      label: 'Documents',
      data: 'Documents Folder',
      icon: 'pi pi-fw pi-inbox',
      children: [
        {
          key: '0',
          label: 'Work',
          data: 'Work Folder',
          icon: 'pi pi-fw pi-cog',
          children: [
            {
              key: '0',
              label: 'Expenses.doc',
              icon: 'pi pi-fw pi-file',
              data: 'Expenses Document',
            },
            {
              key: '0',
              label: 'Resume.doc',
              icon: 'pi pi-fw pi-file',
              data: 'Resume Document',
            },
          ],
        },
        {
          key: '0',
          label: 'Home',
          data: 'Home Folder',
          icon: 'pi pi-fw pi-home',
          children: [
            {
              key: '0',
              label: 'Invoices.txt',
              icon: 'pi pi-fw pi-file',
              data: 'Invoices for this month',
            },
          ],
        },
      ],
    },
    {
      key: '0',
      label: 'Events',
      data: 'Events Folder',
      icon: 'pi pi-fw pi-calendar',
      children: [
        {
          key: '0',
          label: 'Meeting',
          icon: 'pi pi-fw pi-calendar-plus',
          data: 'Meeting',
        },
        {
          key: '0',
          label: 'Product Launch',
          icon: 'pi pi-fw pi-calendar-plus',
          data: 'Product Launch',
        },
        {
          key: '0',
          label: 'Report Review',
          icon: 'pi pi-fw pi-calendar-plus',
          data: 'Report Review',
        },
      ],
    },
    {
      key: '0',
      label: 'Movies',
      data: 'Movies Folder',
      icon: 'pi pi-fw pi-star-fill',
      children: [
        {
          key: '0',
          icon: 'pi pi-fw pi-star-fill',
          label: 'Al Pacino',
          data: 'Pacino Movies',
          children: [
            {
              key: '0',
              label: 'Scarface',
              icon: 'pi pi-fw pi-video',
              data: 'Scarface Movie',
            },
            {
              key: '0',
              label: 'Serpico',
              icon: 'pi pi-fw pi-video',
              data: 'Serpico Movie',
            },
          ],
        },
        {
          key: '0',
          label: 'Robert De Niro',
          icon: 'pi pi-fw pi-star-fill',
          data: 'De Niro Movies',
          children: [
            {
              key: '0',
              label: 'Goodfellas',
              icon: 'pi pi-fw pi-video',
              data: 'Goodfellas Movie',
            },
            {
              key: '0',
              label: 'Untouchables',
              icon: 'pi pi-fw pi-video',
              data: 'Untouchables Movie',
            },
          ],
        },
      ],
    },
  ];

  files2: TreeNode[] = [];

  onDrop(event: any, container: number) {
    console.log(container);
    console.log('drag:', event.dragNode);
    console.log('drop:', event.dropNode);
  }
}
