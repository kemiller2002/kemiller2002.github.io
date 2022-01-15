import * as parser from "xml-js";
import { entry } from "../webpack.config";

const fs = require("fs");

var home = require("os").homedir();

const file = home + "/Downloads/structuredsight.WordPress.2021-12-24.xml";

class Entry {
  type: string;
  elements: Entry[];
  name?: string;
  cdata?: string;
}

function getObjectPropertyOrDefault(elements: unknown[], valueName: string) {
  const items = elements || [{ valueName: "" }];

  return (<any>items[0])[valueName];
}

function getValue(
  valueName: string,
  objectPropertyName: string,
  pGetValue: (name: string) => Entry
): string {
  const selector = pGetValue(valueName);

  return getObjectPropertyOrDefault(selector.elements, objectPropertyName);
}

const extractValue = {
  title: "cdata",
  "content:encoded": "cdata",
  pubDate: "text",
};

const transformers = [{ title: "cdata" }].map(
  (x) => (fn: (name: string) => Entry) => {
    const key = Object.keys(x)[0];
    return getValue(key, (<any>x)[key].toString(), fn);
  }
);

function extractValues(key: object, data: Entry[]): object {
  const keys = Object.keys(key);
  const aggregator = (p: object, k: string, i: number) => {
    console.log(k, p);
    const obj = <any>{};

    obj[k] = getValue(
      k,
      (<any>key)[k],
      (v) => data.filter((d) => d.name === v)[0]
    );

    return Object.assign(obj, p);
  };

  return keys.reduce(aggregator, {});
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

function run(file: string, dataProperties: object) {
  fs.readFile(file, { encoding: "utf-8" }, function (err: Error, data: string) {
    if (data) {
      const json = parser.xml2json(data);
      const objects = JSON.parse(json);

      const items = bruteForceSearch(objects);

      const dataObjects = items.map((i) =>
        extractValues(dataProperties, i.elements)
      );

      fs.writeFile(
        file.replace(".xml", ".data.json"),
        JSON.stringify(dataObjects),
        {},
        () => {}
      );
    }
  });
}

run(file, extractValue);
