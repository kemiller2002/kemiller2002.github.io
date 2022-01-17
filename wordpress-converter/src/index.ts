import * as parser from "xml-js";
import { entry } from "../webpack.config";

const fs = require("fs");

var home = require("os").homedir();

const file = home + "/Downloads/structuredsight.WordPress.2021-12-24.xml";

const destination =
  "/Users/kevinmiller/Documents/GitHub/kemiller2002.github.io/docs/_posts/";

class Entry {
  type: string;
  elements: Entry[];
  name?: string;
  cdata?: string;
}

interface Post {
  title: string;
  pubDate: string;
  "content:encoded": string;
  categories: string;
}

class MarkdownPost {
  name: string;
  content: string;
}

function getObjectPropertyOrDefault(elements: unknown[], valueName: string) {
  const items =
    elements && elements.length > 0 ? elements : [{ valueName: "" }];

  return (<any>items[0])[valueName];
}

function getValue(
  valueName: string,
  objectPropertyName: string,
  pGetValue: (name: string) => Entry
): string {
  const selector = pGetValue(valueName);

  return getObjectPropertyOrDefault(
    (selector || { elements: [] }).elements,
    objectPropertyName
  );
}

const extractValue = {
  category: "cdata",
  title: "cdata",
  "content:encoded": "cdata",
  pubDate: "text",
};

function tryCatch<X, T>(fn: (x: X) => T) {
  return (f: X) => {
    try {
      return fn(f);
    } catch (e) {
      console.log(e, fn);
    }
  };
}

function extractValues(key: object, data: Entry[]): object {
  const keys = Object.keys(key);
  const aggregator = (p: object, k: string, i: number) => {
    const obj = <any>{};

    if (k === "category") {
      const t = data.filter((d) => d.name === k) || [<Entry>{}];

      console.log(t);
    }

    obj[k] = getValue(
      k,
      (<any>key)[k],
      tryCatch(
        (v) =>
          (data.filter((d) => d.name === v) || [<Entry>{ elements: [] }])[0]
      )
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

function hrefParser(tag: string) {
  return tag;
}

function imageParser(tag: string) {
  return tag;
}

const transformCodeBlock = (content: string): string =>
  content.replace(/<\/?pre[a-zA-Z"'0-9 ]*>/gi, "\r\n```\r\n");

const transformList = (content: string): string =>
  content.replace(/<\/?(li|ol|ul)>/gi, ""); //

const transformBold = (content: string): string =>
  content.replace(/<\/?(em|b)>/gi, "**");

const transformHref = (content: string): string =>
  content.replace(/(<a .*>.*<\/a>)/gi, hrefParser);

const transformImage = (content: string): string =>
  content.replace(/(<a .*>.*<\/a>)/gi, imageParser);

const transformTabs = (content: string): string => content.replace(/[\t]/g, "");

function updateContentEntry(fn: (c: string) => string) {
  return (item: Post) => {
    const copy = Object.assign({}, item) as any;

    copy["content:encoded"] = fn(copy["content:encoded"]);

    return copy;
  };
}

function apply<pInput, sInput, output>(
  fFirst: (i: pInput) => sInput,
  fSecond: (i: sInput) => output
) {
  return (i: pInput) => fSecond(fFirst(i));
}

const contentTransformers = [
  transformTabs,
  transformCodeBlock,
  //transformList,
  transformBold,
  transformHref,
  transformImage,
]
  .map((fn) => updateContentEntry(fn))
  .map((fn) => tryCatch(fn))
  .reduce((s, i) => apply(s, i));

const exclude =
  "twentytwelve~twentyfifteen~Kevin Miller~Numbers, Numbers Everywhere".split(
    "~"
  );

function run(
  file: string,
  dataProperties: object,
  destination: string,
  exclude: string[],
  contentExclude: string[]
) {
  fs.readFile(file, { encoding: "utf-8" }, function (err: Error, data: string) {
    if (data) {
      const json = parser.xml2json(data);
      const objects = JSON.parse(json);

      const items = bruteForceSearch(objects);

      const dataObjects = items
        .map((i) => extractValues(dataProperties, i.elements))
        .map((x: Post) => contentTransformers(x))
        .filter((x: Post) => !exclude.some((y) => !x || y === x.title))
        .filter((x: Post) =>
          contentExclude.some((y) => !(x["content:encoded"].indexOf(y) !== -1))
        ) //
        .map((x) => transformIntoPost(x));

      const updatedContent = fs.writeFile(
        file.replace(".xml", ".data.transformed.json"),
        JSON.stringify(dataObjects),
        {},
        () => {}
      );

      dataObjects
        .filter((x) => x)
        .forEach((x) => {
          const fileName = `${destination}${x.name}`; //write-
          fs.writeFile(fileName, x.content, () => {});
        });
    }
  });
}

function transformDate(d: string) {
  const tDate = new Date(d || "2014-1-1");

  return `${tDate.getFullYear()}-${("0" + (tDate.getMonth() + 1)).slice(-2)}-${(
    "0" + tDate.getDate()
  ).slice(-2)}`;
}

function transformIntoPost(p: Post): MarkdownPost {
  if (!p || !p.title) {
    return undefined;
  }

  return {
    name: `${transformDate(p.pubDate)}-${p.title.replace(
      /[ '.]/g,
      "-"
    )}.markdown`,
    content: `---
layout: post
title: "${p.title}"
date: ${transformDate(p.pubDate)} 00:00:00 -0500
${p.categories ? `categories: ${p.categories}` : ""}
---

${p["content:encoded"]}`,
  };
}

run(file, extractValue, destination, exclude, "qaq".split(","));
