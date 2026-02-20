# 🚀 SharpFetch

[![NuGet Version](https://img.shields.io/nuget/v/SharpFetch?style=flat-square&color=004880)](https://www.nuget.org/packages/SharpFetch)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://opensource.org/licenses/MIT)
[![Build Status](https://img.shields.io/github/actions/workflow/status/patrikackermann/SharpFetch/dotnet.yml?style=flat-square)](https://github.com/patrikackermann/SharpFetch/actions)

**The simplicity of the JS Fetch API meets the power of .NET.**

SharpFetch is a lightweight, high-performance wrapper around `HttpClient` that brings the intuitive syntax of JavaScript's Fetch API to C#. It eliminates boilerplate while adding C# essentials like strict type safety, dependency injection support, and modern async patterns.

## ⚡ The Comparison

Stop fighting with `HttpClient` boilerplate. SharpFetch makes network requests feel natural.

### JavaScript (Native)
```javascript
const data = await fetch("https://api.example.com/user")
               .then(res => res.json());

```

### C# with SharpFetch

```csharp
var data = await Fetch("https://api.example.com/user")
               .Then(res => res.Json());

```

## ✨ Key Features

-   **1:1 Syntax:** If you know Fetch, you already know SharpFetch.
    
-   **Type Safety:** Seamlessly parse JSON into C# Classes/Records.
    
-   **Global Opt-in:** Use `SharpFetch.Global` to call `Fetch()` anywhere without extra namespaces.
    
-   **Modern .NET:** Fully compatible with .NET 8, 9, and 10.
    
-   **Zero Bloat:** Minimal dependencies to keep your project lean.

## 🚀 Quick Start

### 1. Installation

```bash
dotnet add package SharpFetch

```
Optionally also add SharpFetch.Global for an automatic global static using.

### 2. Basic Usage (Fluent)

```csharp
// Simple string-based fetch
var user = await Fetch("https://api.example.com/profile")
               .Then(res => res.Json<UserProfile>());

```

### 3. Professional Usage (Instantiated)

For enterprise apps, use the instantiated version for better control over the lifecycle:

```csharp
var api = new SharpFetch();

var response = await api.Fetch("https://api.example.com/data", new Options {
    Method = Method.Post,
    Body = new { name = "John Doe" }
});

var result = await response.Json<ExampleClass>();

```

## 🛠 Project Status

`SharpFetch` is currently in active development.

-   [ ] Core Fetch implementation
    
-   [ ] JSON Deserialization

## 🤝 Contributing

Contributions are welcome! Whether it's a bug report or a feature request, feel free to open an issue.

Built with ❤️ for the .NET community.
