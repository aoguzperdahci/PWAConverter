export class ResourceCollectorGenerator {
  generate(API_URL: string, projectId: string) {
    let template = this.templateSW.replace('%apiUrl%', API_URL + '/api/Source');
    template = template.replace('%projectId%', projectId);
    return template;
  }

  templateSW = `
const API_URL = "%apiUrl%";
const PROJECT_ID = "%projectId%";
const METHODS = ["GET", "POST", "PUT"];

self.addEventListener("install", (event) => {
    self.skipWaiting();
});

self.addEventListener("fetch", (event) => {
    event.respondWith(saveResource(event));
});

function saveResource(event) {
    const method = mapMethod(event.request.method);
    if (method >= 0) {
        const reqBody = {
            url: event.request.url,
            method: method,
            projectId: PROJECT_ID
        };

        fetch(API_URL, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify(reqBody)
        });
    }
    return fetch(event.request);
}

function mapMethod(method) {
    let result = -1;
    switch (method) {
        case "GET":
            result = 0;
            break;
        case "POST":
            result = 1;
            break;
        case "PUT":
            result = 2;
            break;
        default:
            break;
    }

    return result;
}
  `;

  swAttach = `
if ("serviceWorker" in navigator) {
  navigator.serviceWorker.register("sw.js");
}
  `;
}
