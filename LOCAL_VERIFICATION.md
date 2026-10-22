# Local verification

Verified on the Windows x64 host on 2026-10-22, using SDK 6.0.302 and Microsoft.WindowsDesktop.App 6.0.36.

Base: fork `main` at `a6aa5b16e11cefbceacf8ed9a3e2c0bbcc1b050e` (application version 3.6.0).

- Release build and x64 single-file publication succeeded with no warnings or errors in the final publish.
- Smoke checks passed for Simplified Chinese menus and Traditional Chinese fallback, both cat themes, five-frame animation, persisted theme selection, CPU-dependent speed, and independence from memory load.
- Live CPU, GPU, memory, storage and network indicators populated on this host. GPU, network and memory repositories returned samples.
- Startup registry enable/read/disable checks passed. The prior registry value was restored. This does not test an actual sign-out/sign-in cycle.
- Invoking the tray double-click event launched Windows Task Manager. Automatic cleanup of that elevated window was denied by Windows; the optional test now reports that cleanup limitation separately.
- Shutdown during background sampling and long-tooltip handling passed.
- The published EXE selected Microsoft.NETCore.App and Microsoft.WindowsDesktop.App 6.0.36 in host tracing. With the original RunCat already running, it exited with code 0 through the shared single-instance guard.
- No .NET 9 SDK/runtime or third-party runtime package was installed.

Output: `D:\RunCat-Local-x64\RunCat Local.exe` (1,125,523 bytes), with its configuration, README and Apache license.

The original `D:\RunCat-x64\RunCat.exe` was not overwritten or stopped. Its startup setting was not changed. Quit the original before using the local build and configure the new startup entry from the final executable location. Long-duration stability, physical mouse double-click and a fresh Windows login remain manual checks; the double-click handler itself was exercised through the NotifyIcon event.
