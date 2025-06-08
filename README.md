![Language](https://badgen.net/badge/Language/C%23/purple)
![.NET](https://badgen.net/badge/.NET/.9)
![OS](https://badgen.net/badge/OS/linux%2C%20windows%2C%20macOS)
![License](https://badgen.net/github/license/waha-net/waha-aspire-hosting)
![Release](https://badgen.net/github/release/waha-net/waha-aspire-hosting)

![BuildStatus](https://github.com/Waha-net/waha-aspire-hosting/actions/workflows/dotnet.yml/badge.svg?branch=main)

![Stars](https://badgen.net/github/stars/waha-net/waha-aspire-hosting)⭐ Like what you see? Give us a star — it keeps the code flowing!

# Waha Aspire Hosting
Provides extension methods and resource definitions for the .NET Aspire AppHost to support running Waha containers.

### Installation
To install `Waha.Aspire.Hosting`, use the following command in your .NET Aspire AppHost project:

> dotnet add package Waha.Aspire.Hosting

### Usage
Below is a short example of how you can integrate `Waha.Aspire.Hosting` into your .NET Aspire AppHost project:

```csharp
using Aspire;
using Waha.Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var waha = builder.AddWaha("waha")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

builder.AddProject<Projects.API>("api")
    .WithReference(waha)
    .WaitFor(waha);
```
Explanation:

* `builder.AddWaha("waha")` registers a named container resource with default or custom settings.
* `.WithDataVolume()` attaches a persistent data volume to the container.
* `.WithLifetime(ContainerLifetime.Persistent)` ensures the container remains running across application restarts.

### Contributing
We welcome and appreciate contributions from the community. You can open a pull request or report issues through our [GitHub Issues](https://github.com/Waha-net/aspire-hosting-waha/issues/). Please review our contribution guidelines for details on coding standards and development practices.

### Feedback & Support
For any questions, issues, or ideas, feel free to reach out via:

* [GitHub Issues](https://github.com/Waha-net/aspire-hosting-waha/issues)
  
Your feedback helps us make `Waha.Aspire.Hosting` library even better!
