import * as parser from "xml-js";

const fs = require("fs");

var home = require("os").homedir();

const file = home + "/Downloads/structuredsight.WordPress.2021-12-24.xml";

class Entry {
  type: string;
  elements: Entry[];
  name?: string;
  cdata?: string;
}

function getPosts(data: Entry) {
  data.elements.map((x) => getPosts(x));
}

function getTitle(entry: Entry, previousData: any) {
  const title = entry.elements.filter((x) => x.name === "title");
  const data = Object.assign({}, previousData);
  if (title) {
    data.title = title[0].elements[0].cdata;
  }

  return data;
}

const transformers = [getPosts];

const path = "rss.channel".split(".");

function extract(path: string[], item: Entry): Entry[] {
  if (path.length == 0) {
    return item.elements;
  }

  const subset = item.elements.filter((x) => x.name === path[0]);
  const remainingPath = path.slice(1, path.length);

  extract(remainingPath, subset[0]);
}

function extractPostData(elements: Entry[]) {
  const data = elements.filter((x) => x.elements)[0];

  const items = extract(path, data);

  items.forEach((x) => console.log(x));
}

function bruteForceSearch(entry: Entry): Entry[] {
  if (!entry.elements) {
    return [];
  } else if (entry.elements.some((x) => x.name === "content:encoded")) {
    return [entry];
  }

  const elements = entry.elements.map((x) => bruteForceSearch(x)).flat();

  return elements;
}

function run(file: string) {
  fs.readFile(file, { encoding: "utf-8" }, function (err: Error, data: string) {
    if (data) {
      const json = parser.xml2json(data);
      const objects = JSON.parse(json);

      const items = bruteForceSearch(objects);
      console.log(items);

      //extractPostData((<Entry>objects).elements);
    }
  });
}

run(file);
