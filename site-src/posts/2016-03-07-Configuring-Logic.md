---
layout: post
title: "Configuring Logic"
date: 2016-03-07 00:00:00 -0500
---
This <a href="http://stackoverflow.com/questions/35628735/refactoring-class-to-get-rid-of-switch-case/35628935" target="_blank">question</a> talks about removing a switch statement so that every time the business logic changes concerning a multiplier value, the C# code itself doesn't have to be changed and the application recompiled.  I proposed loading the keys and multiplier values from a configuration file into a dictionary and accessing the data when needed. (The following example shows it loaded in the constructor for brevity.)

```

public class TransportationCostCalculator 
{
   Dictionary<string,double> _travelModifier;

   public double DistanceToDestination { get; set; }
   
   TransportationCostCalculator()
   {
      _travelModifier = new Dictionary&lt;string,double&gt; ();

      _travelModifier.Add("bicycle", 1);
      _travelModifier.Add("bus", 2);
      _travelModifier.Add("car", 3);
    }


    public decimal CostOfTravel(string transportationMethod) =>
     (decimal) _travelModifier[transportationMethod] * DistanceToDestination;
 }

```


A comment in the answer mentioned the benefits of creating extra classes, and how the dictionary approach could not handle more advanced calculations should the need arise.  With a slight modification, and some additional code, this no longer becomes a hinderance.  <a href="https://msdn.microsoft.com/en-us/library/bb397951.aspx" target="_blank">Expression Trees</a> allow the program to dynamically create functions and execute them as it would with compiled code.  

Based on the question and the example above, the current equation has two parts, the <strong>travelModifier</strong> (which is determined by the mode of transportation) and the <strong>DistanceToDestination</strong>.  These are multiplied together, and return a <strong>decimal</strong>.  Completely abstracting this out into its own function (which then becomes the model to base the configurable functions from), would make the method look like: 

```

public decimal CalculateTravel
  (int travelModifier, double distanceToDestination) => 
    (decimal) travelModifier * distanceToDestination

```

Since the travel modifier already comes from the configuration file, it is unnecessary to pass that into the function, because when the application reads the configuration and creates the method, each entry will have the <strong>travelModifier</strong> value already coded into the function so that parameter can be removed, and an example function in C# would look like: 

```

decimal CalculateBikeTravel(double distanceToDestination) => 
  1 * distanceToDestination;

```


To accomplish this, each entry in the configuration file would need to have two parts, the method of travel (bicycle, bus, car, etc.), and the equation. The latter is a combination of the <strong>travelModifier</strong> constant, the <strong>distanceToDestination</strong> and operators (+,-,/,*). An entry in the file would look like this: 

```
car,3 * distanceToDestination
```


Before loading the configuration file, the <a href="https://msdn.microsoft.com/en-us/library/xfhwa508(v=vs.110).aspx" target="_blank">dictionary</a> which will hold the function and retrieve it based on the selected method of travel will need to be changed.  Currently it has a string as the key and a double as the value: 

```
Dictionary<string,double> _travelModifier;
```

Instead, it needs a function as the value. 

```
Dictionary<string,Func<double,decimal>> _travelModifier
```


Loading the contents from the configuration file has a few different steps.  Retrieving and separating the parts, parsing the equation, and creating the method at runtime. 

<h3>Loading the Configuration File and Separating the Parts</h3> 

```

var entries = System.IO.File.ReadAllLines("ConfigFile.txt");
var keysAndEquations = entries.Select(entry => entry.split(','));

```


<h3>Parsing the Equation</h3>
It would be possible to parse the equation and immediately convert it to an <strong>Expression</strong>, but it's normally easier to load it into an intermediate structure so data can be transformed and grouped into a usable structure first.  The equation has three parts, and an enum can help distinguish between them.  

```

enum OperatorType
{
Variable, 
Operand,
Constant
}

```

and the class to hold the equation parts

```

class EquationPart
{
  public EquationPart LeftOperand;
  public EquationPart RightOperand;
  public String Name;
  public OperatorType OType;

  public override string ToString() => ToString(0);

  private string ToString(int indent)
  {
    return $@"{Name} : {OType}
      {LeftOperand.ToString(indent + 1)}
      {RightOperand.ToString(indent + 1)}";
  }
}

```


In order to parse the equation, the program needs to determine what is an operator and what is a variable or constant and its execution order. 

```

public static int OperandPrecedence(string item)
{
  switch (item)
  {
    case "+":
    case "-": return 1;
    case "*":
    case "/": return 2;
    default: return 0;
  }
}

```


```

public static EquationPart ParseStatement 
  (IEnumerable&lt;string&gt; statement, EquationPart tree)
{
  if (!statement.Any()) { return tree; }

  var part = statement.First();
  switch (OperandPrecedence(part))
  {
  case 2:
  {
   var op = new EquationPart
   {
    Name = part, 
    OType = OperatorType.Operand,
    LeftOperand = tree, 
    RightOperand = ParseStatement(statement.Skip(1).Take(1),tree) 
   };
   return ParseStatement(statement.Skip(2), op);
  }
  case 1:
  {
    return new EquationPart
    {
     Name = part,
     OType = OperatorType.Operand,
     LeftOperand = tree,
     RightOperand = ParseStatement(statement.Skip(1), null)
    };
   }
  default:
  {
    int result;
    return ParseStatement(statement.Skip(1), new EquationPart
    {
      Name = part,
      OType = (int.TryParse(part, out result))? 
      OperatorType.Constant : OperatorType.Variable
    });
   }
 }

```


<div class="blockquote">
<h3>A Note About Math things</h3>
<h4>Execution Order</h4>
Consider the following: **2 + 4 / 2**.  At first glance, it looks like the answer is three, but that is incorrect.  The multiplication and division have a higher operator order precedence and their calculations occur before addition and subtraction.  This makes the actual answer 4.  The C# compiler knows about order of operations and which happens first. When building the expression tree, the runtime doesn't take this into account, and will execute each operation strictly from left to right.  It is important to note this when creating and grouping the intermediate objects to form a tree with the execution order, so it is correct.
</div>

<h3>Making the Expression</h3>
The <a href="https://msdn.microsoft.com/en-us/library/system.linq.expressions.expression(v=vs.110).aspx" target="_blank">System.LINQ.Expressions.Expression</a> is the class used to create the lambda expressions.  The actual method to create the function is <a href="https://msdn.microsoft.com/en-us/library/system.linq.expressions.lambdaexpression(v=vs.110).aspx" target="_blank">Expression.Lambda&lt;T&gt;</a> and then call its compile function to turn it into a callable method.

```
Expression.Lambda<Func>int, double>> (body, parameterExpression).Compile();
```


The <strong>Lambda</strong> function requires two parameters, an <strong>Expression</strong>, and a <a href="https://msdn.microsoft.com/en-us/library/system.linq.expressions.parameterexpression(v=vs.110).aspx" target="_blank">ParameterExpression[]</a>. The entries in the <strong>ParameterExpression[]</strong> are the parameters to the function and they are made by calling <a href="https://msdn.microsoft.com/en-us/library/bb355908(v=vs.110).aspx" target="_blank">Expression.Parameter</a>.


```

var distanceParm = Expression.Parameter(typeof(int), "distanceToDestination");
var parameterExpression = new ParameterExpression[] {distanceParm};

```


<h4>Expression Body</h4>
Each <strong>Expression</strong> object is a tree of <strong>Expression</strong> objects.  The four  methods used to create the operator functions (<a href="https://msdn.microsoft.com/en-us/library/bb354402(v=vs.110).aspx" target="_blank">Expression.Add</a>, <a href="https://msdn.microsoft.com/en-us/library/bb357532(v=vs.110).aspx" target="_blank">Expression.Subtract</a>, <a href="https://msdn.microsoft.com/en-us/library/system.linq.expressions.expression.multiply(v=vs.110).aspx" target="_blank">Expression.Multiply</a>, and <a href="https://msdn.microsoft.com/en-us/library/bb358569(v=vs.110).aspx" target="_blank">Expression.Divide</a>) all take two <strong>Expression</strong> parameters (the left term and the right term), and each <strong>Expression</strong> can be one of three things, a constant (<a href="https://msdn.microsoft.com/en-us/library/system.linq.expressions.expression.constant(v=vs.110).aspx" target="_blank">Expression.Constant</a>), the supplied parameter (<strong>ParameterExpression</strong>), or another <strong>Expression</strong>.  

With this, all that is necessary is to convert the <strong>EquationPart</strong> tree into an expression. 

```

public static Expression MakeBody
  (EquationPart tree, ParameterExpression distance)
{
 if (tree.OType == OperatorType.Operand)
 {
    var leftAction = MakeBody(tree.LeftOperand, distance);
    var rightAction = MakeBody(tree.RightOperand, distance);
    var action = Code.GetModifier(tree.Name)(Expression.Convert
      (leftAction, typeof(double)), 
    Expression.Convert(rightAction, typeof(double)));

    return action;
  }

  return (tree.Name == distance.Name) ? 
  (Expression)distance : Expression.Constant(int.Parse(tree.Name), typeof(int));
}

```


<h4>Additional Actions</h4>
It might be necessary to do additional actions in the expression, for example method's output could be logged to the console.  To do this, the Lambda Expression would now need to:

1.  Calculate the result of the equation (calling the created equation).
2.  Assign that value to a variable.
3.  Write the variable contents out to the console. 
4.  Return the result stored in the variable.  

Right now, the body of the Lambda Expression is the result of a single <strong>Expression</strong> object. All the actions culminate to a single result, but when adding logging, this changes.  Calculating the result and logging it are separate unrelated actions.  The <a href="https://msdn.microsoft.com/en-us/library/dd324074(v=vs.110).aspx" target="_blank">Expression.Block</a> groups <strong>Expressions</strong> together, and returns the value from the last executed <strong>Expression</strong>.  

The first step is creating a variable using <a href="https://msdn.microsoft.com/en-us/library/dd294144(v=vs.110).aspx" target="_blank">Expression.Variable</a> it takes a <a href="https://msdn.microsoft.com/en-us/library/system.type(v=vs.110).aspx" target="_blank">Type</a> and optionally a variable name.


```
var result = Expression.Variable(typeof(double), "result");
```


Then assign the results of the body <strong>Expression</strong> to it: 


```

var assign = Expression.Assign(result, body);

```


Now the system can log the result, by using <a href="https://msdn.microsoft.com/en-us/library/bb349020(v=vs.110).aspx" target="_blank">Expression.Call</a>.    


```

var write = Expression.Call(
    typeof(Console).GetMethod(
      "WriteLine", new Type[1]{typeof(double)}), 
     result);

```


The <strong>Expression.Block</strong> method takes <strong>Expressions</strong> to be executed in the entered order.  The only exception to this is the creation of the variable which much be passed into the method by a <strong>ParameterExpression[]</strong>.


```

var block = Expression.Block(
  new ParameterExpression[] {result},assign,write,result);

return Expression.Lambda<Func<int, double>> (block, parameterExpression).Compile();

```


The full method with the console output looks like this:


```

public static Func<int, double> CreateStatement(string statement)
{
 var statementParts = statement.Split(' ');

 var tree = ParseStatement(statementParts, null);

 var travelParm = Expression.Parameter(typeof(int), "distanceToDestination");
 var parameterExpression = new ParameterExpression[] { travelParm };

 var body = MakeBody(tree, travelParm);

 var result = Expression.Variable(typeof(double), "result");
        
 var assign = Expression.Assign(result, body);
 var write = Expression.Call(typeof(Console)
    .GetMethod("WriteLine", new Type[1]{typeof(double)}), 
    result);

 var block = Expression.Block(
  new ParameterExpression[] {result},assign,write,result);
 
 return Expression.Lambda<Func<int, double>> 
  (block, parameterExpression).Compile();
}

```


<h4>If/Then</h4>
The methods use the <strong>double</strong> type resulting in the impossibility of a <a href="https://msdn.microsoft.com/en-us/library/system.dividebyzeroexception(v=vs.110).aspx" target="_blank">DivideByZeroException</a>.  <a href="https://msdn.microsoft.com/en-us/library/aa691146(v=vs.71).aspx" target="_blank">Per the C# specification</a>, it returns the value infinity.  

To create a conditional statement use the <a href="https://msdn.microsoft.com/en-us/library/bb340500(v=vs.110).aspx" target="_blank">Expression.Condition</a> method which has three parameters (the <strong>Expression</strong> for the test, the true block, and the false block).  

<h5>Test Condition</h5>
The test condition is an <strong>Expression</strong>, and the <strong>double</strong> type has a static method for checking for the infinity value.  To use it, the <strong>Expression.Call</strong> method works just like it did with writing data to the <strong>Console.WriteLine</strong>.  

```

Expression.Call(
  typeof(double).GetMethod("IsInfinity", new Type[1] {typeof(double)}), 
  resultToCheck);

```


<h5>True Block</h5>
If the condition is true (meaning that the value is infinity, then it should throw an exception indicating a problem.  <strong>Expression</strong> has a method for throwing exceptions, <a href="https://msdn.microsoft.com/en-us/library/dd294113(v=vs.110).aspx" target="_blank">Expression.Throw</a>


```

var trueBlock = Expression.Throw
  (Expression.Constant(new Exception("Result is infinity")));

```


<h5>Empty False Statement</h5>
A false statement isn't necessary, because if the condition is false, it will continue to the next statement outside of the condition.  The <strong>Expression.Condition</strong> will not allow <strong>null</strong> as the third parameter, so to have an empty false statement use <a href="https://msdn.microsoft.com/en-us/library/system.linq.expressions.expression.empty(v=vs.110).aspx" target="_blank">Expression.Empty</a> instead. 


```

static Expression CreateInfinityCondition(ParameterExpression resultToCheck)
{
  var test = Expression.Call(
    typeof(double).GetMethod("IsInfinity", new Type[1] {typeof(double)}),
    resultToCheck);
  var trueBlock = Expression.Throw(
    Expression.Constant(new Exception("Result is infinity")));

  return Expression.Condition(test, trueBlock, Expression.Empty());

}

```


<h4>Try Catch</h4>
Instead of passing the exception to the calling method, a second option would be to log it first by wrapping the method contents in a <a href="https://msdn.microsoft.com/en-us/library/0yd65esw.aspx" target="_blank">try-catch</a> block.  The <a href="https://msdn.microsoft.com/en-us/library/system.linq.expressions.expression.trycatch(v=vs.110).aspx" target="_blank">Expression.TryCatch</a> method has two parameters: the expression which contains the body information in the try statement, and the <a href="https://msdn.microsoft.com/en-us/library/system.linq.expressions.catchblock(v=vs.110).aspx" target="_blank">CatchBlock</a>.  <a href="https://msdn.microsoft.com/en-us/library/system.linq.expressions.expression.makecatchblock(v=vs.110).aspx" target="_blank">Expression.MakeCatchBlock</a> has three parameters: the type of <strong>Exception</strong> the catch block is for, the <strong>ParameterExpression</strong> which allows the Expression to bind the <strong>Exception</strong> to a variable for use, and the <strong>Expression</strong> code inside the catch statement. 


```

var parameterException = Expression.Parameter(typeof(Exception));

var logCatchException = Expression.Call(typeof(Console).GetMethod("WriteLine",
  new Type[1] { typeof(string) }),
  Expression.Call(parameterException,
  typeof(Exception).GetMethod("ToString"))
        );

var logCatchStatement = Expression.Call(typeof(Console).GetMethod("WriteLine",
  new Type[1] { typeof(string) }),
    Expression.Constant("Configuration statement: " + statement));

var catchBlockCode = Expression.Block(logCatchException, logCatchStatement, 
  Expression.Rethrow(typeof(double)));

var catchBlock = Expression.MakeCatchBlock
    (typeof(Exception), parameterException ,catchBlockCode,null);

var tryCatch = Expression.TryCatch(mainBody, catchBlock);

```


<h5>Expression.Rethrow</h5>
<a href="https://msdn.microsoft.com/en-us/library/dd267626(v=vs.110).aspx">Expression.Rethrow</a> has two method signatures.  The first has not parameters, and the second has a parameter of type of <strong>Type</strong>.  In this example, since it is the last statement in the catch block (the the statement in a block determines what is returned from the block), if you use <strong>Expression.Throw()</strong>, the application will return with this error: **Body of catch must have the same type as body of try.**  This is saying that the the try and catch blocks must have the same return type.  In the example, the try block returns type <strong>double</strong>, so the catch block must do the same.  The overload for <strong>Expression.Throw(Type)</strong>, tells the runtime "This catch statement will return this type if necessary."  Since it's throwing the exception, it won't ever return a value, but this tells the <strong>Expression</strong> generator this will be the intended behavior if an exception doesn't occur.  

<a href="https://raw.githubusercontent.com/kemiller2002/StructuredSight/master/ExpressionTrees/CreateExpressionTreeWithTryCatch.linq" target="_blank">Here are all the code examples.</a>