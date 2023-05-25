import { CacheStrategy } from "src/app/models/cacheStrategy";
import { SourceContainer } from "src/app/models/sourceContainer";

interface CacheObject {
  name: string;
  rules: string[];
  size?: number;
}

export class ServiceWorkerGenerator{

  generate(sourceContainers: SourceContainer[]){
    const staticCaches = [] as CacheObject[];
    const dynamicCaches = [] as CacheObject[];
    for (const sourceContainer of sourceContainers) {
      if (sourceContainer.cacheStrategy === CacheStrategy.cacheFirst) {
        staticCaches.push({name: sourceContainer.name, rules: sourceContainer.rules});
      } else {
        dynamicCaches.push({name: sourceContainer.name, rules: sourceContainer.rules, size: sourceContainer.maxSize})
      }
    }

    let template = this.templateSW.replace("%staticCaches%", JSON.stringify(staticCaches));
    template = template.replace("%dynamicCaches%", JSON.stringify(dynamicCaches));
    console.log(template);
  }

  templateSW = `const staticCaches = %staticCaches%;
  const dynamicCaches = %dynamicCaches%;

  self.addEventListener("install", (event) => {
    event.waitUntil(
      caches.open(staticCaches[0].name).then((staticCache) => {
        staticCache.addAll(staticCache[0].rules);
      })
    );
  });

  self.addEventListener("activate", (event) => {
    event.waitUntil(
      caches.keys().then((keys) => {
        return Promise.all(
          keys
            .filter(
              (key) => staticCaches.some(c => c.name !== key) && dynamicCaches.some(c => c.name !== key)
            )
            .map((key) => caches.delete(key))
        );
      })
    );
  });

  self.addEventListener("fetch", (event) => {
    event.respondWith(provideCacheRespond(event));
  });

  function provideCacheRespond(event) {
    for (const cacheObject of staticCaches) {
      if (cacheObject.rules.some(r => event.request.url.startsWith(r))) {
        return caches.open(cacheObject.name).then(staticCache => {
          staticCache.match(event.request.url)
            .then(cachedRespond => {
              if (cachedRespond) {
                return cachedRespond;
              } else {
                return fetch(event.request).then(response => {
                  const clone = response.clone();
                  staticCache.put(event.request.url, clone);
                  return response;
                });
              }
            })
        })
      }
    }

    for (const cacheObject of dynamicCaches) {
      if (cacheObject.rules.some(r => event.request.url.startsWith(r))) {
        return caches.open(cacheObject.name).then(dynamicCache => {
          return fetch(event.request).then(response => {
            const clone = response.clone();
            dynamicCache.put(event.request.url, clone);
            limitCacheSize(cacheObject);
            return response;
          }).catch(err => {
            return dynamicCache.match(event.request.url);
          })
        })
      }
    }

    return fetch(event.request).catch(err => {
      if(evt.request.url.indexOf('.html') > -1){
        return caches.open(staticCaches[0].name).match('/fallback.html');
      }
    });
  }

  function limitCacheSize (cacheObject) {
    caches.open(cacheObject.name).then(cache => {
      cache.keys().then(keys => {
        if(keys.length > cacheObject.size){
          cache.delete(keys[0]).then(limitCacheSize(cacheObject));
        }
      });
    });
  };
  `;

}
