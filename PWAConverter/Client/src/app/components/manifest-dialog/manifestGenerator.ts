import { Manifest } from "src/OpenApiClient";

export class ManifestGenerator{

  generate(manifest: Manifest, displayMode: string, orientation: string): string{
    let template = this.templateManifest.replace("%shortName%", manifest.shortName ?? "App");
    template = template.replace("%shortName%", manifest.shortName ?? "App");
    template = template.replace("%startUrl%", manifest.startUrl ?? "/");
    template = template.replace("%backgroundColor%", manifest.backGroundColor ?? "#000000");
    template = template.replace("%displayMode%", displayMode);
    template = template.replace("%orientation%", orientation);
    template = template.replace("%scope%", manifest.scope ?? "/");
    template = template.replace("%themeColor%", manifest.themeColor ?? "#000000");
    template = template.replace("%description%", manifest.description ?? "An App");
    return template;
  }

  compressImage(src: string, newX: number, newY: number): Promise<string> {
    return new Promise((res, rej) => {
      const img = new Image();
      img.src = src;
      img.onload = () => {
        const elem = document.createElement('canvas');
        elem.width = newX;
        elem.height = newY;
        const ctx = elem.getContext('2d');
        if (ctx) {
          ctx.drawImage(img, 0, 0, newX, newY);
          const data = ctx.canvas.toDataURL();
          res(data);
        }
      }
      img.onerror = error => rej(error);
    })
  }

  dataURItoBlob(dataURI:string): Blob {
    // convert base64 to raw binary data held in a string
    // doesn't handle URLEncoded DataURIs - see SO answer #6850276 for code that does this
    const byteString = atob(dataURI.split(',')[1]);

    // separate out the mime component
    const mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0]

    // write the bytes of the string to an ArrayBuffer
    const ab = new ArrayBuffer(byteString.length);

    // create a view into the buffer
    const ia = new Uint8Array(ab);

    // set the bytes of the buffer to the correct values
    for (let i = 0; i < byteString.length; i++) {
        ia[i] = byteString.charCodeAt(i);
    }

    // write the ArrayBuffer to a blob, and you're done
    const blob = new Blob([ab], {type: mimeString});
    return blob;

  }


  templateManifest = `
{
  "short_name": "%shortName%",
  "name": "%shortName%",
  "start_url": "%startUrl%",
  "background_color": "%backgroundColor%",
  "display": "%displayMode%",
  "orientation": "%orientation%",
  "scope": "%scope%",
  "theme_color": "%themeColor%",
  "description": "%description%",
  "icons": [
    {
      "src": "/app-icon/icon-64x64.png",
      "sizes": "64x64",
      "type": "image/png"
    },
    {
      "src": "/app-icon/icon-128x128.png",
      "sizes": "128x128",
      "type": "image/png"
    },
    {
      "src": "/app-icon/icon-192x192.png",
      "sizes": "192x192",
      "type": "image/png"
    },
    {
      "src": "/app-icon/icon-256x256.png",
      "sizes": "256x256",
      "type": "image/png"
    },
    {
      "src": "/app-icon/icon-512x512.png",
      "sizes": "512x512",
      "type": "image/png"
    }
  ]
}
  `;
}
