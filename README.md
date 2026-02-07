# AutoCAD Layout & Quantity Extraction Plugin
 
## Overview

A professional AutoCAD .NET plugin that generates evenly spaced interior lines inside a closed polyline and exports their lengths as structured JSON for quantity estimation workflows.

Ideal for:

* Slab layout
* Flooring estimation
* Steel / rebar spacing
* Decking layouts
* Construction quantity takeoffs

---

## Features

✔ Generate horizontal, vertical, or both-direction interior lines
✔ Accurate geometry (no hatch dependency)
✔ Automatic length aggregation with quantities
✔ Command-line length display
✔ Structured JSON export
✔ Transaction-safe and production-ready architecture

---

## Project Structure

```
Commands/
    HatchCommands.cs

Services/
    HatchService.cs
    JsonService.cs
    LineData.cs
```

---

## Installation

1. Build the project in **x64** mode.
2. Ensure AutoCAD references are added:

   * acdbmgd.dll
   * acmgd.dll
   * accoremgd.dll

(Set `Copy Local = False`)

3. Load the plugin in AutoCAD:

```
NETLOAD
```

Select the compiled DLL from:

```
bin/x64/Debug/
```

---

## Usage

Run:

```
AUTOHATCH
```

Steps:

1. Select a closed polyline.
2. Choose line direction (Horizontal / Vertical / Both).
3. Enter spacing (drawing units).

The plugin will create interior lines and export JSON.

---

## Example JSON

```json
{
  "Horizontal": [
    { "Length": 2989.99, "Quantity": 3 }
  ],
  "Vertical": [
    { "Length": 2100.00, "Quantity": 4 }
  ]
}
```

---

## Notes

* Spacing follows drawing units (mm, meters, etc.).
* Currently supports closed polylines.
* Designed using computational geometry for higher accuracy than hatch-based workflows.

---

