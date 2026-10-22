# RunCat Local

A personal Windows tray application based on [RunCat365](https://github.com/runcat-dev/RunCat365), built for an existing .NET 6 Windows Desktop runtime. This fork uses the upstream 3.6.0 source with the .NET 6 deployment model used by RunCat 2.0. No reverse engineering is required.

## Features

- One running cat, with light and dark themes. Higher CPU usage makes it run faster.
- CPU, GPU, memory, network, disk and available thermal-zone readings in the tray menu.
- Double-click the tray icon to open Windows Task Manager.
- Optional launch at sign-in through the current user's Windows startup registry key.
- Simplified and Traditional Chinese resources, alongside the other upstream translations.
- Custom animations, other animals and the endless game have been removed.

The first launch chooses a cat color suitable for the Windows theme. Later theme choices are saved. Language follows the Windows UI language. GPU and thermal-zone information depends on the counters exposed by the machine; thermal-zone readings are not necessarily CPU package temperatures.

## Run locally

Use Windows 10/11 x64 with **Microsoft.WindowsDesktop.App 6.0.36** installed. .NET 6 is out of support; this target deliberately reuses the requested existing runtime. No .NET 9 SDK or runtime is needed.

Run `RunCat Local.exe` from its published directory. Quit the old RunCat first: both versions intentionally use the same single-instance mutex. Enable launch at startup in the new tray menu only after choosing a permanent executable location. Disable the old version's startup entry separately before switching; this application does not alter it.

The new startup value is `HKCU\Software\Microsoft\Windows\CurrentVersion\Run\RunCat Local`. Its executable path is quoted to support spaces. Moving the executable requires enabling startup again from its new location. Preferences use .NET's per-user settings storage; this is not a fully portable settings directory, and settings from 2.0 are not imported.

## Build and publish

The repository pins SDK 6.0.302 (latest patch within its feature band is allowed).

```powershell
dotnet build RunCat365/RunCat365.csproj -c Release
dotnet run --project tests/RunCat.Local.Smoke -c Release
./publish-local.ps1
```

Publication produces `artifacts/RunCat-Local-x64/RunCat Local.exe`, a framework-dependent single-file application, plus its configuration and license/documentation. The Windows Desktop runtime is not bundled. The Store packaging project is excluded from the solution and is not part of the local build or release path. Existing `WapForStore` files are retained only as upstream history.

The smoke program briefly creates a tray icon, checks both Chinese resource variants, switches both themes, samples live system metrics, checks CPU-driven animation, and verifies shutdown while sampling. It uses its own application settings and restores the startup registry value after testing it. The optional `-- --task-manager` flag verifies the actual tray double-click by launching Task Manager; automatic cleanup of an elevated window may be unavailable.

## Compatibility changes

| Upstream requirement | .NET 6 / C# 10 replacement |
| --- | --- |
| Collection expressions (`[]`, `[.. items]`) | Constructors, array initializers and `ToArray()` |
| Primary constructor on `BalloonTipInfo` | Explicit constructor and properties |
| `System.Threading.Lock` | Private object with `lock` |
| `LibraryImport` source generator | `DllImport`, preserving the Win32 BOOL marshaling contract |
| `Application.SetColorMode` | Existing explicit tray bitmap recoloring; no .NET 9 automatic window color mode |
| New `AddRange` overloads | Explicit `ToolStripItem[]` |
| Store `StartupTask` / package identity | Current-user Run registry entry |
| DPI in application manifest | `ApplicationHighDpiMode=PerMonitorV2` |
| Chinese culture resource discovery with SDK 6 on this host | `zh-Hans` / `zh-Hant` satellite resources, used by `zh-CN` / `zh-TW` fallback |

No third-party runtime packages were added. The application targets `net6.0-windows`, requests runtime 6.0.36, and allows newer patches only within .NET 6.

## Attribution

Original application by Takuto Nakamura and RunCat contributors, licensed under Apache-2.0. Modified in this fork for local .NET 6 deployment, cat-only UI, Task Manager access, and removal of custom runners and the game. See Git history for individual changes and `LICENSE` for terms.
