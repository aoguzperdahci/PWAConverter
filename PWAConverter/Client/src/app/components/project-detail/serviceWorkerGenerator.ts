import { AdditionalFeatures } from 'src/app/models/additionalFeatures';
import { CacheStrategy } from 'src/app/models/cacheStrategy';
import { ProjectDetail } from 'src/app/models/projectDetail';

interface CacheObject {
  name: string;
  rules: string[];
  size?: number;
}

export class ServiceWorkerGenerator {
  generateSW(projectDetail: ProjectDetail, title?: string): string {
    const hash = this.generateHash(4);
    const staticCaches = [] as CacheObject[];
    const dynamicCaches = [] as CacheObject[];
    let preCache = [] as string[];
    let template = this.templateSW;
    for (const sourceContainer of projectDetail.sourceContainers) {
      if (sourceContainer.cacheStrategy === CacheStrategy.preCache) {
        staticCaches.push({
          name: sourceContainer.name + '-' + hash,
          rules: sourceContainer.rules.map((r) => '%^' + r + '%'),
        });
        preCache = preCache.concat(sourceContainer.sourceList);
      } else if (sourceContainer.cacheStrategy === CacheStrategy.cacheFirst) {
        staticCaches.push({
          name: sourceContainer.name + '-' + hash,
          rules: sourceContainer.rules.map((r) => '%^' + r + '%'),
        });
      } else if (sourceContainer.cacheStrategy === CacheStrategy.networkFirst) {
        dynamicCaches.push({
          name: sourceContainer.name + '-' + hash,
          rules: sourceContainer.rules.map((r) => '%^' + r + '%'),
          size: sourceContainer.maxSize,
        });
      }
    }

    if (
      projectDetail.additionalFeatures.some(
        (feature) => feature === AdditionalFeatures.OfflineFallbackPage
      )
    ) {
      preCache.unshift('/fallback.html');
      template = template.replace(
        '%fetchNetwork%',
        this.templateFetchNetworkWithFallback
      );
    } else {
      template = template.replace('%fetchNetwork%', this.templateFetchNetwork);
    }

    template = template.replace('%staticCaches%', JSON.stringify(staticCaches));
    template = template.replace(
      '%dynamicCaches%',
      JSON.stringify(dynamicCaches)
    );
    template = template.replace(/\//g, '\\/');
    template = template.replace(/"%/g, '/');
    template = template.replace(/%"/g, '/');
    template = template.replace('%preCache%', JSON.stringify(preCache));

    if (
      projectDetail.additionalFeatures.some(
        (feature) => feature === AdditionalFeatures.PushNotification
      )
    ) {
      template = template.replace(
        '%pushEventHandler%',
        this.templatePushEventHandler
      );
      template = template.replace('%title%', title ?? 'App');
    } else {
      template = template.replace('%pushEventHandler%', '');
    }

    return template;
  }

  generateHash(length: number): string {
    let result = '';
    const characters =
      'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    const charactersLength = characters.length;
    let counter = 0;
    while (counter < length) {
      result += characters.charAt(Math.floor(Math.random() * charactersLength));
      counter += 1;
    }
    return result;
  }

  generateSWAttach(projectDetail: ProjectDetail): string {
    let template = this.templateSWAttach;
    if (
      projectDetail.additionalFeatures.some(
        (feature) => feature === AdditionalFeatures.PushNotification
      )
    ) {
      template = template.replace(
        '%pushNotificationCall%',
        'subscribePushNotifications();'
      );
      template = template.replace(
        '%pushNotificationFunction%',
        this.templatePushNotificationFunction
      );
      template = template.replace(
        '%pushNotificationServerKey%',
        projectDetail.options.pushNotificationServerKey ?? ''
      );
      template = template.replace(
        '%pushNotificationSubscribeEndpoint%',
        projectDetail.options.pushNotificationSubscribeEndpoint ?? ''
      );
    } else {
      template = template.replace('%pushNotificationCall%', '');
      template = template.replace('%pushNotificationFunction%', '');
    }

    return template;
  }

  templateSWAttach = `if ("serviceWorker" in navigator) {
  navigator.serviceWorker.register("sw.js");
  %pushNotificationCall%
}

%pushNotificationFunction%`;

  templatePushNotificationFunction = `function subscribePushNotifications() {
navigator.serviceWorker.ready.then(sw => {
  sw.pushManager.getSubscription().then(sub => {
    if (!sub) {
      sw.pushManager.subscribe({
        userVisibleOnly: true,
        applicationServerKey: "%pushNotificationServerKey%"
      }).then(res => fetch("%pushNotificationSubscribeEndpoint%", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify(res)
      }));
    }
  })
});
}`;

  templateSW = `const staticCaches = %staticCaches%;
const dynamicCaches = %dynamicCaches%;
const preCache = %preCache%;

self.addEventListener("install", (event) => {
  event.waitUntil(
    caches.open(staticCaches[0].name).then((staticCache) => {
      staticCache.addAll(preCache);
    })
  );
});

self.addEventListener("activate", (event) => {
  event.waitUntil(
    caches.keys().then((keys) => {
      return Promise.all(
        keys
          .filter(
            (key) => !staticCaches.some(c => c.name === key) && !dynamicCaches.some(c => c.name === key)
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
    if (cacheObject.rules.some(r => r.test(event.request.url))) {
      return caches.open(cacheObject.name).then(staticCache => {
        return staticCache.match(event.request.url)
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
    if (cacheObject.rules.some(r => r.test(event.request.url))) {
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

  %fetchNetwork%
}
%pushEventHandler%
function limitCacheSize (cacheObject) {
  caches.open(cacheObject.name).then(cache => {
    cache.keys().then(keys => {
      if(keys.length > cacheObject.size){
        cache.delete(keys[0]).then(limitCacheSize(cacheObject));
      }
    });
  });
};`;

  templateFetchNetwork = `return fetch(event.request);`;

  templateFetchNetworkWithFallback = `return fetch(event.request).catch(err => {
    if(event.request.url.indexOf(".html") > -1 || event.request.mode === "navigate"){
      return caches.open(staticCaches[0].name).then(staticCache => {
        return staticCache.match(preCache[0]);
      });
    }
  });`;

  templatePushEventHandler = `
self.addEventListener("push", (event) => {
  const options = {
    body: event.data.text(),
    icon: "app-icon/icon-256x256.png",
    vibrate: [100, 50, 100],
    data: {
      dateOfArrival: Date.now(),
      primaryKey: "1"
    }
  }
  event.waitUntil(self.registration.showNotification("%title%", options));
});
`;

  fallbackPage = `
<!DOCTYPE html>
<html>

<head>
    <title>Offline</title>
    <meta charset="UTF-8">
    <style>
        html,
        body {
            height: 100%;
            margin: 0;
            font-size: 24px;
        }

        body {
            background: black;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        canvas {
            border: 1px solid white;
        }

        p {
            color: white;
            font-family: Helvetica, Arial, sans-serif;
        }

        #start-button {
            background-color: #fbeee0;
            border: 2px solid #422800;
            border-radius: 30px;
            box-shadow: #422800 4px 4px 0 0;
            color: #422800;
            cursor: pointer;
            display: inline-block;
            font-weight: 600;
            font-size: 18px;
            padding: 0 18px;
            line-height: 50px;
            text-align: center;
            text-decoration: none;
            user-select: none;
            -webkit-user-select: none;
            touch-action: manipulation;
            margin-left: 140px;
        }

        #start-button:hover {
            background-color: #fff;
        }

        #start-button:active {
            box-shadow: #422800 2px 2px 0 0;
            transform: translate(2px, 2px);
        }

        @media (min-width: 768px) {
            #start-button {
                min-width: 120px;
                padding: 0 25px;
            }
        }
    </style>
</head>

<body>
    <div id="message">
        <p style="font-size: 32px; margin-left: 64px;">No internet connection</p>
        <p>Try reloading this page when you are online</p>
        <p>You can play Tetris while you wait</p>
        <button onclick="startGame()" id="start-button">Start Game</button>
    </div>
    <canvas width="320" height="640" id="game" style="display: none;"></canvas>
    <script>
        // https://tetris.fandom.com/wiki/Tetris_Guideline

        // get a random integer between the range of [min,max]
        // @see https://stackoverflow.com/a/1527820/2124254
        function getRandomInt(min, max) {
            min = Math.ceil(min);
            max = Math.floor(max);

            return Math.floor(Math.random() * (max - min + 1)) + min;
        }

        // generate a new tetromino sequence
        // @see https://tetris.fandom.com/wiki/Random_Generator
        function generateSequence() {
            const sequence = ['I', 'J', 'L', 'O', 'S', 'T', 'Z'];

            while (sequence.length) {
                const rand = getRandomInt(0, sequence.length - 1);
                const name = sequence.splice(rand, 1)[0];
                tetrominoSequence.push(name);
            }
        }

        // get the next tetromino in the sequence
        function getNextTetromino() {
            if (tetrominoSequence.length === 0) {
                generateSequence();
            }

            const name = tetrominoSequence.pop();
            const matrix = tetrominos[name];

            // I and O start centered, all others start in left-middle
            const col = playfield[0].length / 2 - Math.ceil(matrix[0].length / 2);

            // I starts on row 21 (-1), all others start on row 22 (-2)
            const row = name === 'I' ? -1 : -2;

            return {
                name: name,      // name of the piece (L, O, etc.)
                matrix: matrix,  // the current rotation matrix
                row: row,        // current row (starts offscreen)
                col: col         // current col
            };
        }

        // rotate an NxN matrix 90deg
        // @see https://codereview.stackexchange.com/a/186834
        function rotate(matrix) {
            const N = matrix.length - 1;
            const result = matrix.map((row, i) =>
                row.map((val, j) => matrix[N - j][i])
            );

            return result;
        }

        // check to see if the new matrix/row/col is valid
        function isValidMove(matrix, cellRow, cellCol) {
            for (let row = 0; row < matrix.length; row++) {
                for (let col = 0; col < matrix[row].length; col++) {
                    if (matrix[row][col] && (
                        // outside the game bounds
                        cellCol + col < 0 ||
                        cellCol + col >= playfield[0].length ||
                        cellRow + row >= playfield.length ||
                        // collides with another piece
                        playfield[cellRow + row][cellCol + col])
                    ) {
                        return false;
                    }
                }
            }

            return true;
        }

        // place the tetromino on the playfield
        function placeTetromino() {
            for (let row = 0; row < tetromino.matrix.length; row++) {
                for (let col = 0; col < tetromino.matrix[row].length; col++) {
                    if (tetromino.matrix[row][col]) {

                        // game over if piece has any part offscreen
                        if (tetromino.row + row < 0) {
                            return showGameOver();
                        }

                        playfield[tetromino.row + row][tetromino.col + col] = tetromino.name;
                    }
                }
            }

            // check for line clears starting from the bottom and working our way up
            for (let row = playfield.length - 1; row >= 0;) {
                if (playfield[row].every(cell => !!cell)) {

                    // drop every row above this one
                    for (let r = row; r >= 0; r--) {
                        for (let c = 0; c < playfield[r].length; c++) {
                            playfield[r][c] = playfield[r - 1][c];
                        }
                    }
                }
                else {
                    row--;
                }
            }

            tetromino = getNextTetromino();
        }

        // show the game over screen
        function showGameOver() {
            cancelAnimationFrame(rAF);
            gameOver = true;

            context.fillStyle = 'black';
            context.globalAlpha = 0.75;
            context.fillRect(0, canvas.height / 2 - 30, canvas.width, 60);

            context.globalAlpha = 1;
            context.fillStyle = 'white';
            context.font = '36px monospace';
            context.textAlign = 'center';
            context.textBaseline = 'middle';
            context.fillText('GAME OVER!', canvas.width / 2, canvas.height / 2);
        }

        const canvas = document.getElementById('game');
        const context = canvas.getContext('2d');
        const grid = 32;
        const tetrominoSequence = [];

        // keep track of what is in every cell of the game using a 2d array
        // tetris playfield is 10x20, with a few rows offscreen
        const playfield = [];

        // populate the empty state
        for (let row = -2; row < 20; row++) {
            playfield[row] = [];

            for (let col = 0; col < 10; col++) {
                playfield[row][col] = 0;
            }
        }

        // how to draw each tetromino
        // @see https://tetris.fandom.com/wiki/SRS
        const tetrominos = {
            'I': [
                [0, 0, 0, 0],
                [1, 1, 1, 1],
                [0, 0, 0, 0],
                [0, 0, 0, 0]
            ],
            'J': [
                [1, 0, 0],
                [1, 1, 1],
                [0, 0, 0],
            ],
            'L': [
                [0, 0, 1],
                [1, 1, 1],
                [0, 0, 0],
            ],
            'O': [
                [1, 1],
                [1, 1],
            ],
            'S': [
                [0, 1, 1],
                [1, 1, 0],
                [0, 0, 0],
            ],
            'Z': [
                [1, 1, 0],
                [0, 1, 1],
                [0, 0, 0],
            ],
            'T': [
                [0, 1, 0],
                [1, 1, 1],
                [0, 0, 0],
            ]
        };

        // color of each tetromino
        const colors = {
            'I': 'cyan',
            'O': 'yellow',
            'T': 'purple',
            'S': 'green',
            'Z': 'red',
            'J': 'blue',
            'L': 'orange'
        };

        let count = 0;
        let tetromino = getNextTetromino();
        let rAF = null;  // keep track of the animation frame so we can cancel it
        let gameOver = false;

        // game loop
        function loop() {
            rAF = requestAnimationFrame(loop);
            context.clearRect(0, 0, canvas.width, canvas.height);

            // draw the playfield
            for (let row = 0; row < 20; row++) {
                for (let col = 0; col < 10; col++) {
                    if (playfield[row][col]) {
                        const name = playfield[row][col];
                        context.fillStyle = colors[name];

                        // drawing 1 px smaller than the grid creates a grid effect
                        context.fillRect(col * grid, row * grid, grid - 1, grid - 1);
                    }
                }
            }

            // draw the active tetromino
            if (tetromino) {

                // tetromino falls every 35 frames
                if (++count > 35) {
                    tetromino.row++;
                    count = 0;

                    // place piece if it runs into anything
                    if (!isValidMove(tetromino.matrix, tetromino.row, tetromino.col)) {
                        tetromino.row--;
                        placeTetromino();
                    }
                }

                context.fillStyle = colors[tetromino.name];

                for (let row = 0; row < tetromino.matrix.length; row++) {
                    for (let col = 0; col < tetromino.matrix[row].length; col++) {
                        if (tetromino.matrix[row][col]) {

                            // drawing 1 px smaller than the grid creates a grid effect
                            context.fillRect((tetromino.col + col) * grid, (tetromino.row + row) * grid, grid - 1, grid - 1);
                        }
                    }
                }
            }
        }

        // listen to keyboard events to move the active tetromino
        document.addEventListener('keydown', function (e) {
            if (gameOver) return;

            // left and right arrow keys (move)
            if (e.which === 37 || e.which === 39) {
                const col = e.which === 37
                    ? tetromino.col - 1
                    : tetromino.col + 1;

                if (isValidMove(tetromino.matrix, tetromino.row, col)) {
                    tetromino.col = col;
                }
            }

            // up arrow key (rotate)
            if (e.which === 38) {
                const matrix = rotate(tetromino.matrix);
                if (isValidMove(matrix, tetromino.row, tetromino.col)) {
                    tetromino.matrix = matrix;
                }
            }

            // down arrow key (drop)
            if (e.which === 40) {
                const row = tetromino.row + 1;

                if (!isValidMove(tetromino.matrix, row, tetromino.col)) {
                    tetromino.row = row - 1;

                    placeTetromino();
                    return;
                }

                tetromino.row = row;
            }
        });

        // start the game
        function startGame() {
            requestAnimationFrame(loop);
            let element = document.getElementById("message");
            element.style.display = "none";
            let game = document.getElementById("game");
            game.style.display = "block";
        }
    </script>
</body>

</html>`;
}
