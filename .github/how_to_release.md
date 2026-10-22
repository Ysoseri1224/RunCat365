# Release a local Windows executable

This fork does not submit packages to the Microsoft Store.

1. Update the application version in `RunCat365/RunCat365.csproj` when needed.
2. Run `dotnet build RunCat365/RunCat365.csproj -c Release` with SDK 6.0.302.
3. Run `dotnet run --project tests/RunCat.Local.Smoke -c Release` on Windows with Desktop runtime 6.0.36. The optional `-- --task-manager` flag also invokes the tray double-click and opens Task Manager if it is not already running. Automatic cleanup of an elevated Task Manager window may be unavailable.
4. Run `./publish-local.ps1`.
5. Verify the contents of `artifacts/RunCat-Local-x64`. Distribute that directory as a ZIP, including the Apache license. The runtime is not bundled.
6. Quit existing RunCat instances before launching the new EXE. Verify its tray menu and startup behavior from the permanent install location.

GitHub releases are optional; publication to a local directory does not create an online release. The `WapForStore` files are retained upstream history and are not used by this process.
