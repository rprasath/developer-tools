# Developer Tools

A compact Blazor WebAssembly app with practical developer utilities for text, JSON, SQL, GUIDs, timestamps, JWTs, table data, and timezone conversion.

## Features

- Line Break Toolkit: Convert pasted lines into CSV, quoted lists, and SQL helper snippets.
- SQL Formatter: In-place SQL formatting with clause breaks and indentation options.
- Tabular Data Formatter: Convert between CSV, JSON, SQL, and Markdown tables.
- String Workbench: JSON/SQL formatting, casing transforms, encoding/decoding, and line operations.
- JSON Refiner: Validate, format/minify, sort, and prune JSON fields.
- JSON to Code: Generate C# and TypeScript models from JSON payloads.
- Diff Viewer: Monaco-based side-by-side comparison with fullscreen support.
- GUID Helpers: New GUID + GUID <-> Long/Int conversions.
- Unix Timestamp Converter: Timestamp/date round-trip (seconds and milliseconds).
- Time Zone Converter: Convert date-time across source and target zones.
- JWT Decoder: Decode and inspect JWT tokens locally.

## Tech Stack

- .NET 10 (`net10.0`)
- Blazor WebAssembly
- Tailwind CSS (local bundle)
- BlazorMonaco (diff editor)
- Newtonsoft.Json

## Getting Started

### Prerequisites

- .NET SDK 10.0+

### Run locally

```bash
dotnet restore DevTools.sln
dotnet build DevTools.sln
dotnet run --project DevTools/DevTools.csproj
```

Then open the local URL printed by `dotnet run`.

## Project Structure

```text
DevTools.sln
DevTools/
  Pages/               # Tool pages
  Shared/              # Layout/navigation
  wwwroot/             # Static assets and JS helpers
  Program.cs           # Blazor WASM bootstrap
```

## Contributing

Contributions are welcome.

1. Fork the repository
2. Create a branch
3. Make your changes
4. Run build locally
5. Open a pull request

## Security & Privacy

Most processing is done client-side in the browser. Validate any sensitive workflow before sharing production data.
