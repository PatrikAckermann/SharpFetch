using SharpFetch;
using SharpFetch.Demo.Models;
using static SharpFetch.SharpFetch;

// Normal C# Syntax

var getResult = await Fetch("https://jsonplaceholder.typicode.com/todos/1", new SharpFetch.Models.Options
{
    Method = HttpMethod.Get
});

Console.WriteLine("GET Result: ", getResult.StatusText);

var obj = getResult.Json<JsonPlaceHolderObj>();

// JS Fetch Like THEN

var thenTest = await Fetch("https://jsonplaceholder.typicode.com/todos/1").Then(x => x.Json<JsonPlaceHolderObj>());

// JS Fetch Like THEN with immediate action for the data

var thenTestNoAwait = Fetch("https://jsonplaceholder.typicode.com/todos/1").Then(x => x.Json<JsonPlaceHolderObj>()).Then(x => Console.WriteLine(x.Title));

// JS Fetch like THEN with error handling

await Fetch("https://jsonplaceholder.typicode.com/todos/1").Then(x =>
{
    if (x.Ok)
    {
        return x.Json<JsonPlaceHolderObj>();
    }
    else
    {
        throw new Exception($"Request failed with status {x.Status}");
    }
}).Then(x => Console.WriteLine($"Title: {x.Title}"));

await Fetch("https://jsonplaceholder.typicode.com/todos/1").Then(x =>
{
    if (x.Ok)
    {
        return x.Json<JsonPlaceHolderObj>();
    }
    else
    {
        throw new Exception($"Request failed with status {x.Status}");
    }
});

// Text Test

var text = await Fetch("https://jsonplaceholder.typicode.com/todos/1").Then(x => x.Text());

Console.WriteLine("byebye");
Console.ReadKey();