---
layout: post
title: "It's OK, My eval is Sandboxed (No It's Not)"
date: 2016-07-06 00:00:00 -0500

---

The idea of using eval has always been in interesting debate. Instead of writing logic which accounts for possibly hundreds of different scenarios, creating a string with the correct JavaScript and then executing it dynamically is a much simpler solution. This isn't a new approach to programming and is commonly seen in languages such as SQL (stored procedures vs. dynamically generating statements). On one hand it can save a developer an immense amount of time writing and debugging code. On the other, it's power is something which can be abused because of it's high execution privileges in the browser.

The question is, "should ever be used?" It technically would be safe if there is a way of securing all the code it evaluates, but this limits its effectiveness and goes against its dynamic nature. So with this, is there a balance point where using it is secure, but also flexible enough to warrant the risk?

For example purposes, we'll use the following piece of code to show the browser has been successfully exploited: <strong>alert('Be sure to drink your Ovaltine.');</strong> If the browser is able to execute that code, then restricting the use of eval failed.

In the most obvious example where nothing is sanitized executing the alert is trivial:


```

function runCode(code) {
  var executingCode = code
  console.log(executingCode)

  eval(executingCode);
}

var inputFromExternalSource = "alert('Be sure to drink your Ovaltine.');"
runCode(inputFromExternalSource);

```


eval will treat any input as code and execute it. So what if eval is restricted to only execute which will correctly evaluate to a complete statement?


```

function runCode(code) {
  var executingCode = code
  console.log(executingCode)

  eval("var total = " + executingCode);
}

var inputFromExternalSource = "alert('Be sure to drink your Ovaltine.');"
runCode(inputFromExternalSource);

```


Nope, this still successfully executes. In JavaScript all functions return something, so calling alert and assigning undefined to total is perfectly valid.

What about forcing a conversion to a number?

```

function runCode(code) {
  var executingCode = 'var total = parseInt(' + code + ')'
  console.log(executingCode)

  eval(executingCode);
}

var inputFromExternalSource = 
    "alert('Be sure to drink your Ovaltine.')"
runCode(inputFromExternalSource);

```


This still executes also, because the alert function fires when it is parsed and its return value is converted to a string and then parsed.

The following does stop the alert from firing,

```

function runCode(code) {
  var executingCode = 'var total = ' + parseInt(code)
  console.log(executingCode)

  eval(executingCode);
}

var inputFromExternalSource = 
    "alert('Be sure to drink your Ovaltine.')"
runCode(inputFromExternalSource);

```


But this is rather pointless, because eval isn't necessary. It's much easier to assign the value to the total variable directly.


```



function runCode(code) {
  var total = parseInt(code);
}

```


What about overriding the global function alert with a local function?


```

function runCode(code) {
  var alert = x => console.log(x);
  var executingCode = code;

  console.log(executingCode)

  eval(executingCode);
}

var inputFromExternalSource = 
    "alert('Be sure to drink your Ovaltine.')"
runCode(inputFromExternalSource);

```


This does work for the current scenario. It overrides the global alert function with the local one but doesn't solve the problem. The alert function can still be called explicitly from the window object itself.


```

function runCode(code) {
  var alert = x => console.log(x);
  var executingCode = code;

  console.log(executingCode)

  eval(executingCode);
}

var inputFromExternalSource = 
    "window.alert('Be sure to drink your Ovaltine.')"
runCode(inputFromExternalSource);

```


With this in mind, it is possible to remove any reference to window (or alert for that matter) in the code string before executing.


```

function runCode(code) {
  var alert = x => console.log(x);
  var executingCode = code.replace('window.', '');

  console.log(executingCode)

  eval(executingCode);
}

var inputFromExternalSource = 
    "window.alert('Be sure to drink your Ovaltine.')"
runCode(inputFromExternalSource);

```


This works when the word 'window' is together, but the following code executes successfully:


```

function runCode(code) {
  var alert = x => console.log(x);
  var executingCode = code.replace('window.', '');

  console.log(executingCode)

  eval(executingCode);
}

var inputFromExternalSource = 
    "eval('win' + 'dow.alert(\"Be sure to drink your Ovaltine.\")')"
runCode(inputFromExternalSource);

```


Since 'win' and 'dow' are separated, the replacement does not find it. The code works by using the first eval to join the execution code together while the second executes it. Since replace is used to remove the window code, it's also possible to do the same thing to eval like so:


```

function runCode(code) {
  var alert = x => console.log(x);
  var executingCode = code.replace('window.', '').replace('eval','');

  console.log(executingCode)

  eval(executingCode);
}

var inputFromExternalSource = 
    "eval('win' + 'dow.alert(\"Be sure to drink your Ovaltine.\")')"
runCode(inputFromExternalSource);

```


That stops the code from working, but it doesn't stop this:

```

function runCode(code) {
  var alert = x => console.log(x);
  var executingCode = code.replace('window.', '').replace('eval','');

  console.log(executingCode)

  eval(executingCode);
}

var inputFromExternalSource = 
    "new Function('win' + 
        'dow.alert(\"Be sure to drink your Ovaltine.\")')()"
runCode(inputFromExternalSource);

```


It is possible to keep accounting for different scenarios whittling down the different attack vectors, but this gets extremely complicated and cumbersome. Further more, using eval opens up other scenarios besides direct execution which may not be accounted for. Take the following example:


```

function runCode(code) {
  var alert = x => console.log(x);
  var executingCode = code.replace('window.', '').replace('eval','');

  console.log(executingCode)

  eval(executingCode);

  JSON.parse ('alert("ruh roh raggy")');
}

var inputFromExternalSource = 
    "JSON.parse = this.parent['e' + 'val']"
runCode(inputFromExternalSource);

```


This code bypasses the replace sanitations, and it's goal wasn't to execute malicious code. It's goal is to replace the JSON.parse with eval and depending on the application might assume that malicious code is blocked, because JSON.parse doesn't natively execute rogue code.

Take the following example:

```

function runCode(code) {
  var alert = x => console.log(x);
  var executingCode = code.replace('window.', '').replace('eval','');

  console.log(executingCode)

  eval(executingCode);
 
  JSON.parse ('alert("ruh roh raggy")');
}

var inputFromExternalSource = 
    "JSON.parse = this.parent['e' + 'val']"
runCode(inputFromExternalSource);

function runAssumedSafeCode(code){
  return JSON.parse (code);
}

var assumedSafeCodeFromExternalSource = 
    "{'id':23, 'message' : 'this is a harm \' }" + 
    (function () {alert('that Ovaltine was good!'); return ""}()) + 
    ";{ }";
runAssumedSafeCode(assumedSafeCodeFromExternalSource);

```


The code does throw an exception at the end due to invalid parsing, but that isn't a problem for the attacker, because eval already executed the rogue code. The eval statement was used to perform a lateral attack against the functions which are assumed not to execute harmful instructions.

<h4>Server Side Validation</h4>

A great extent of the time, systems validate user input on the server trying to ensure harmful information is never stored in the system. This is a smart idea, because removing before storing it tries to ensure everything accessing potentially harmful code doesn't need to make certain it isn't executing something it shouldn't (you really shouldn't and can't make this assumption, but it is a good start in protecting against attacks). With eval, this causes a false sense of security, because code like C# does not handle strings the same way that JavaScript does. For example:


```

public string Sanitize (string input) => input.Replace("window", "").Replace("eval", "");
public void Run()
{
  var santizedInput = Sanitize(
    "window.alert('Be sure to drink your Ovaltine.')");
  Console.WriteLine(santizedInput);

  var santizedInputUnicode = Sanitize(System.IO.File.ReadAllText(
    @"\u0077\u0069\u006e\u0064\u006f\u0077\u002e\u0061\u006c\u0065\u0072\u0074\u0028\u0027\u0042\u0065 \u0073\u0075\u0072\u0065 \u0074\u006f \u0064\u0072\u0069\u006e\u006b \u0079\u006f\u0075\u0072 \u004f\u0076\u0061\u006c\u0074\u0069\u006e\u0065\u002e\u0027\u0029\u003b"));
  Console.WriteLine(santizedInputUnicode);
}

```


In the first example, the C# code successfully removed the word 'window', but in the second, it was unable to interpret this when presented with Unicode characters which JavaScript interprets as executable instructions. (In order to test the unicode characters, you need to place an @ symbol in front of the string so that it will treat it exactly as it is written. Without it, the C# compiler will convert it.) Worse yet, JavaScript can interpret strings which are a mixture of text and Unicode values making it more difficult to search and replace potentially harmful values.


```

public string Sanitize (string input) => input.Replace("window", "").Replace("eval", "");
public void Run()
{
  var santizedInput = Sanitize(
    "window.alert('Be sure to drink your Ovaltine.')");
  Console.WriteLine(santizedInput);

  var santizedInputUnicode = Sanitize(System.IO.File.ReadAllText(@"w\u0069n\u0064o\u0077\u002e\u0061\u006c\u0065\u0072\u0074\u0028\u0027\u0042\u0065 \u0073\u0075\u0072\u0065 \u0074\u006f \u0064\u0072\u0069\u006e\u006b \u0079\u006f\u0075\u0072 Ovaltine.\u0027\u0029\u003b"));
Console.WriteLine(santizedInputUnicode);
}

```


Assuming the dynamic code passed into eval is completely sanitized, and there is no possibility of executing rogue code, it should be safe to use. The problem is that it's most likely not sanitized, and at best it's completely sanitized <strong>for now</strong>. 