import { Component, Input, OnInit } from '@angular/core';
import { MessageService } from 'primeng/api';
import { Manifest, ManifestClient, UpdateManifestModel } from 'src/OpenApiClient';
import { ManifestGenerator } from './manifestGenerator';
import * as JSZip from 'jszip';
import * as saveAs from 'file-saver';

@Component({
  selector: 'app-manifest-dialog',
  templateUrl: './manifest-dialog.component.html',
  styleUrls: ['./manifest-dialog.component.css'],
  providers: [MessageService]
})
export class ManifestDialogComponent implements OnInit {
  dialogVisible = false;
  displayModeOptions = ['fullscreen', 'standalone', 'minimal-ui', 'browser'];
  orientationOptions = ['any', 'landscape', 'portrait'];

  manifest = {} as Manifest;
  displayMode = 'fullscreen';
  orientation = 'any';
  @Input() projectId?: string;
  @Input() icon?: string;

  constructor(private manifestService: ManifestClient, private messageService: MessageService) {}
  ngOnInit(): void {
    this.manifestService.getManifestFile(this.projectId).subscribe((res) => {
      this.manifest = res;
      this.displayMode = this.displayModeOptions[res.displayMode ?? 0];
      this.orientation = this.orientationOptions[res.orientation ?? 0];
    });
  }

  async generateManifest() {
    const requestData = {
      projectId: this.projectId,
      shortName: this.manifest.shortName,
      description: this.manifest.description,
      displayMode: this.displayModeOptions.findIndex(o => o === this.displayMode),
      orientation: this.orientationOptions.findIndex(o => o === this.orientation),
      backGroundColor: this.manifest.backGroundColor,
      themeColor: this.manifest.themeColor,
      startUrl: this.manifest.startUrl,
      scope: this.manifest.scope,
    } as UpdateManifestModel;

    this.manifestService.updateManifest(requestData).subscribe(res =>
      this.messageService.add({
        severity: 'success',
        summary: 'Saved',
        detail: 'Changes successfully saved',
      })
    );

    if (this.icon) {
      const codeGenerator = new ManifestGenerator();
      const manifest = codeGenerator.generate(this.manifest, this.displayMode, this.orientation);
      const icon512 = await codeGenerator.compressImage(this.icon, 512, 512);
      const icon256 = await codeGenerator.compressImage(this.icon, 256, 256);
      const icon192 = await codeGenerator.compressImage(this.icon, 192, 192);
      const icon128 = await codeGenerator.compressImage(this.icon, 128, 128);
      const icon64 = await codeGenerator.compressImage(this.icon, 64, 64);

      const zip = new JSZip();
      zip.file("manifest.json", manifest);
      zip.file("app-icon/icon-512x512.png", codeGenerator.dataURItoBlob(icon512));
      zip.file("app-icon/icon-256x256.png", codeGenerator.dataURItoBlob(icon256));
      zip.file("app-icon/icon-192x192.png", codeGenerator.dataURItoBlob(icon192));
      zip.file("app-icon/icon-128x128.png", codeGenerator.dataURItoBlob(icon128));
      zip.file("app-icon/icon-64x64.png", codeGenerator.dataURItoBlob(icon64));
      zip.generateAsync({type:"blob"})
      .then((content) => {
        saveAs(content, "manifest.zip");
      });
    }
  }

  manifestScript(): string[]{
    const script = [`<meta name="theme-color" content="${this.manifest.themeColor}" />`,
    `<meta name="description" content="${this.manifest.description}" />`,
    `<meta name="viewport" content="width=device-width, initial-scale=1" />`,
    `<link rel="icon" href="app-icon/icon-64x64.png" />`,
    `<link rel="apple-touch-icon" href="app-icon/icon-192x192.png" />`,
    `<link rel="manifest" href="manifest.json" />`];
    return script;
  }

}
