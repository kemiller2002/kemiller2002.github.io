const fs = require("fs");
const parser = require("xml2json");

const file = "/downloads/structuredsight.WordPress.2021-12-24.xml";

fs.readFile(file, function (err: Error, data: string) {
  var json = parser.toJson(data);
  console.log("to json ->", json);
});
