import { Component, Inject, Input } from '@angular/core';
import { API_BASE_URL } from 'src/OpenApiClient';
import { ResourceCollectorGenerator } from './resourceCollectorGenerator';
import * as JSZip from 'jszip';
import * as saveAs from 'file-saver';
@Component({
  selector: 'app-resource-collector-dialog',
  templateUrl: './resource-collector-dialog.component.html',
  styleUrls: ['./resource-collector-dialog.component.css'],
})
export class ResourceCollectorDialogComponent {
  @Input() projectId? = '';
  dialogVisible = false;
  apiUrl: string;
  attachScript = `<script src="sw-attach.js"></script>`;
  constructor(@Inject(API_BASE_URL) baseUrl: string) {
    this.apiUrl = baseUrl;
  }

  generateCollector() {
    if (this.projectId) {
      const codeGenerator = new ResourceCollectorGenerator();
      const collector = codeGenerator.generate(this.apiUrl, this.projectId);
      const zip = new JSZip();
      zip.file('sw.js', collector);
      zip.file('sw-attach.js', codeGenerator.swAttach);
      zip.generateAsync({ type: 'blob' }).then((content) => {
        saveAs(content, 'sourceCollector.zip');
      });
    }
  }
}
