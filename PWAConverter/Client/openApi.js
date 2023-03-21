const https = require("https");
const fs = require('fs');
process.env["NODE_TLS_REJECT_UNAUTHORIZED"] = 0;

https
  .get(`https://localhost:7194/swagger/v1/swagger.yaml`, resp => {
    let data = "";

    // A chunk of data has been recieved.
    resp.on("data", chunk => {
      data += chunk;
    });

    // The whole response has been received. Print out the result.
    resp.on("end", () => {
      fs.writeFile("./openApi.yaml", data, function(err) {
        if(err) {
            return console.log(err);
        }
        console.log("The file was saved!");
    });
        });
  })
  .on("error", err => {
    console.log("Error: " + err.message);
  });


