---
layout: post
title: "Project Oxford - Image Text Detection"
date: 2016-03-21 00:00:00 -0500
---
Microsoft has a new set of services which use machine learning to extrapolate data from images and speech called <a href="https://www.projectoxford.ai/" target="_blank">Project Oxford</a>.  Each service has a public facing REST api allowing any program capable of communicating through HTTP to utilize it, and for smaller amounts of data, Project Oxford is <a href="https://www.projectoxford.ai/pricing" target="_blank">free</a> to use.  

Microsoft classifies these services into three categories: vision, speech, and language.  The vision section contains several different capabilities including facial recognition, facial emotion detection, video stabilization, and optical character recognition (OCR).  It's OCR api allows a system to send an image URL and in turn will return the text it detected.  Since the service is system agnostic, it's possible to create an HTML page (with a little help from a service like <a href="http://imgur.com/" target="_blank">Imgur</a>) which can upload an image and extract the text without using server side resources.  (The completed example can be found <a href="https://cdn.rawgit.com/kemiller2002/StructuredSight/master/OxfordAndTranslate/ImageTranslator.html" target="_blank">here</a>.)

<h2>How to create</h2>

<h4>Uploading the Image</h4>
The first step is uploading the image to a publicity accessible store.  Imgur works well for this, because it's easy to use and free.  It requires an account and registering the "application", but this is relatively quick and painless.  The page to do so is found <a href="https://api.imgur.com/oauth2/addclient" target="_blank">here</a>, and with the api key, all that is left is to retrieve the image from the input and upload the file.  

<h4>Uploading the File</h4> 
HTML 5 has the input type: <a href="https://developer.mozilla.org/en-US/docs/Using_files_from_web_applications" target="_blank">file</a>.  To upload the file when the user selects it, add an <strong>onchange</strong> event to the input.

```

<input type="file" id="fileUpload" name="fileUpload" 
    onChange="uploadImage(event)" />

```


Now the function uploadImage fires whenever the user select a new file. 

```

function uploadImage (event) {
  var input = event.target;

  var reader = new FileReader();
  reader.onload = function () {
    var dataURL = reader.result;
    postImage(dataURL);
  }

    reader.readAsDataURL(input.files[0]);
}

```


With the help of the <a href="https://developer.mozilla.org/en-US/docs/Web/API/FileReader" target="_blank">FileReader</a>, the webpage can load the image into memory to send to the server.  It has several methods for retrieving files such as <a href="https://developer.mozilla.org/en-US/docs/Web/API/FileReader/readAsText" target="_blank">readAsText</a> and <a href="https://developer.mozilla.org/en-US/docs/Web/API/FileReader/readAsBinaryString" target="_blank">readAsBinaryString</a>, but the one which allows the image to upload correctly is <a href="https://developer.mozilla.org/en-US/docs/Web/API/FileReader/readAsDataURL" target="_blank">readAsDataURL</a> as this uploads the image in the correct encoded format.  

Reading the file is an asynchronous operation and requires a method to call when loading. There is a property in the FileReader object named <a href="https://developer.mozilla.org/en-US/docs/Web/API/FileReader/onload" target="_blank">onload</a> and has event parameter, but it's not necessary in this context, because the reader's result object is available through <a href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Closures" target="_blank">closure</a>, so omitting it is fine. 

<h4>Sending the File to Imgur</h4>
Whether using JQuery, another framework, or plain JavaScript, the upload process is straight forward.  This example is in JQuery.


```
function postImage (img) {
    $.ajax({
        url: 'https://api.imgur.com/3/image',
        headers: {
            'Authorization': 'Client-ID ad65ed241de3567'
        },
        type: 'POST',
        data: {
            'image': img.split(',')[1]
        },
        success: function(response) {
          ocrImage(response.data.link, response.data.deletehash);
        }
    });
}
```


The Imgur api exposes the <strong>https://api.imgur.com/3/image</strong> URL, and to upload an image, use <strong>POST</strong> and add the HTTP header <strong>Authorization</strong> with the client id: <strong>Client-ID application_client_id</strong>.  The trickiest part concerns the file upload from the FileReader.  The <strong>readAsDataURL</strong> returns more data than necessary and the contents look like 

```

data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAARMAAAC3....

```

The comma and everything before it is extraneous and will cause an error, so it's necessary to remove it. Using <a href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/String/split" target="_blank">String.Split(',')</a> and picking the second item in the array is the easiest approach.  

Posting the image to Imgur returns several pieces of data about the object, height, width, size, etc., but the key pieces are, link (which is the URL to the image), and deletehash (this will be used to remove the image once completed.  The api documentation says in some cases the id can be used, but since this is an anonymous posting, the delete hash is necessary).  

<h4>Data OCR</h4>
Project Oxford requires a login to grant the necessary api key.  After signing up and logging in, the service portal lists a page with access to the various API keys (there is one for each service).  The once necessary for OCR looks like: 

<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/OxfordAndTranslate/ComputerVisionGetApiKey.png" alt="get api key for ocr" />

To retrieve the image text information, POST to: <strong>https://api.projectoxford.ai/vision/v1/ocr</strong>.  It requires the HTTP Header <strong>Ocp-Apim-Subscription-Key</strong> with the value being the client id retrieved from the key dashboard.  The data contains the URL as the key, and there are two optional querystring parameters which the request can have: <strong>language</strong> and <strong>detectOrientation</strong>.  

<h6>language</h6>
The parameter <strong>language</strong> specifies which language to use when performing the OCR.  If it's not provided, the service attempts a determination based on the image and return what it thinks is correct.  This can be helpful, if the language is unknown, and goal of the OCR process is to extract the text and then translate it.  (Unfortunately, Microsoft has disabled the features, mainly <a href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Access_control_CORS" target="_blank">Cross Origin Request Sharing</a>, for retrieving the authentication token necessary to create an application not having any server side processing in it's <a href="https://www.microsoft.com/en-us/translator/translatorapi.aspx" target="_blank">Microsoft Translator API</a>).  Not including leaves the service up to guess and can provide incorrect results.  

The parameter is the <a href="https://tools.ietf.org/html/bcp47" target="_blank">BCP-47 language code</a> which follows <a href="https://en.wikipedia.org/wiki/ISO_639-1" target="_blank">ISO 639-1</a>. (A lowercase two character code representing each language e.g. en for English, es for Spanish, etc.)  At this time the API only supports about 20 languages, but Microsoft has said it is working to expand the list.  The <a href="https://dev.projectoxford.ai/docs/services/54ef139a49c3f70a50e79b7d/operations/5527970549c3f723cc5363e4" target="_blank">API documentation</a> lists the supported languages under the language parameter.

<h6>detectOrientation</h6>
This is a boolean parameter indicating it should attempt to recognize the text and orient it to be parallel with the top of image's bounding box.  Sometimes this can help in how it groups the returned text. 


```

  $.ajax({
    url: 'https://api.projectoxford.ai/vision/v1/ocr?detectOrientation=true',
    headers:{
      'Ocp-Apim-Subscription-Key' : '197a2be138ac41f79ef06255d6db5a7c',
    },
    type:'POST',
    contentType:"application/json",
    data: JSON.stringify({"Url": url}),
    success: function (response) {
      console.log(response);
      deleteImage(id);
      $("#language").text(response.language);
      $("#orientation").text(response.orientation);
      let text = response
        .regions
        .map(r=>r.lines
                  .map(l=>l.words.map(w=>w.text).join("<br/>"))
                  .join("<br/><br/>")
            ).join("<br/><br/>");

      $("#response").html(text);
    },
    error:function () {deleteImage(id);}

```


<h5>Response</h5>
The service returns an object looking like this: 

```
{language: "en", textAngle: -38.70000000000005, 
       orientation: "Right", regions: Array[1]}
```


<h6>language</h6>
The <strong>language</strong> property is what the service thinks the language in the image is, and if there are multiple languages in the text, it still only returns one.  The service makes its best determination and won't necessarily pick the first it encounters.

<h6>TextAngle</h6>
TextAngle is the tilt the service thinks the image is set to, and orientation is the direction the top text is facing.  If the text is facing 45 degrees to the right, the text angle would be "-45.000" and the orientation would be "Right".

<h6>Regions</h6>
<img src="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/OxfordAndTranslate/MultiLanguage.png" alt="multilanguage" align="right" style="border:thin solid gray;padding-left:5px;" /> The found text in the image is not lumped together when returned. There are separate entries for each section of text found. In the example to the right, Hola and Hello are found in two different areas, so there are two regions returned in the array.  Each region has a property <strong>boundingBox</strong> which is a list of comma separated coordinates where the text region exists on the page.  In each region there is a <strong>lines</strong> property which is an array of objects each with their own <strong>boundingBox</strong> and each <strong>line</strong> has a <strong>words</strong> property object array separating each word and also containing the <strong>boundingBox</strong> property. 

```

{
  "language": "en",
  "textAngle": 0,
  "orientation": "Up",
  "regions": [
    {
      "boundingBox": "81,63,1340,1055",
      "lines": [
        {
          "boundingBox": "321,63,855,117",
          "words": [
            {
              "boundingBox": "321,63,174,94",
              "text": "An"
            },
            {
              "boundingBox": "529,87,126,69",
              "text": "Example"
            }
          ]
        }
     ]
   }
 ]
}

```


<h4>Removing the Image</h4>
Removing the image is similar to the initial <strong>POST</strong> action.  This time the action is <strong>DELETE</strong> and the URL has the <strong>deletehash</strong> appended to the end.  


```

function deleteImage(deleteHash){
  $.ajax({
      url: `https://api.imgur.com/3/image/${deleteHash}`,
      headers: {
          'Authorization': 'Client-ID ad65ed241de3567'
      },
      type: 'DELETE',
      success: function(response) {
        console.log(response);
      }
  });
}

```


<a href="https://github.com/kemiller2002/StructuredSight/blob/master/OxfordAndTranslate/PostImage.js" target="_blank">The Javascript code can be found here.</a>