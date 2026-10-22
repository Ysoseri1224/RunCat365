# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## You Must

- Respond in the same language the user is writing in.
- Commit to Git only when instructed.
- If multiple requirements are given in a single instruction, divide the commits into appropriate sizes/granularities.
- When creating a pull request, use the `.claude/skills/create-pr` skill so the project's `.github/pull_request_template.md` is followed.

## Project Overview

Windows Forms application (.NET 6 / C# 10) distributed as a local executable. Runs as a cat-only tray icon whose animation reflects CPU load.

**Top-level layout:**

- `RunCat365.sln` — Main solution
- `RunCat365/` — Main application project
- `WapForStore/` — Legacy upstream packaging files, excluded from the local solution

**Build:** `dotnet build RunCat365/RunCat365.csproj -c Release`. Target: `net6.0-windows`, runtime 6.0.36. Local x64 publication: `./publish-local.ps1`. Windows smoke checks: `dotnet run --project tests/RunCat.Local.Smoke -c Release`.

**Releasing:** Follow `.github/how_to_release.md` for the local executable workflow. Update only the application version for this fork; the legacy Store manifest is not used.


## Architecture (patterns, not file lists)

- **Entry point:** `Program.cs` hosts the tray application context that owns the timers, the tray icon, and the context menu.
- **Repository pattern for system metrics:** Each metric has a `*Repository` class that produces a matching `*Info` struct (CPU / GPU / memory / storage / network / temperature). Add a new metric by adding a paired repository and info struct.
- **Animation loop:** A 1-second fetch timer samples CPU/GPU; other metrics and the animation interval refresh every five samples. CPU is the only animation speed source. Theme-aware cat recoloring lives in `BitmapExtension`.
- **Scope:** Only light/dark cats; no custom runners, other animals or game. Double-click opens Task Manager. Startup uses a quoted current-user Run entry.
- **Settings & resources:** Persisted user preferences in `Properties/UserSettings.settings`; embedded images/icons in `Properties/Resources.resx`.

## Localization

- Localized strings live in `Properties/Strings.resx` (English default) and sibling `Properties/Strings.{locale}.resx` files. Any new string must be added to every variant.
- Per-locale font and culture settings live in `SupportedLanguage.cs`.
- When adding a brand-new locale (not just new strings), follow the `.claude/skills/add-locale` skill.

## Coding Rules

General contribution rules — indentation style, `var` usage, single-change PRs, English-only code, license, and the rest — are documented in [`CONTRIBUTING.md`](CONTRIBUTING.md). Follow those first.

Project-specific additions:

- Do not write comments within the source code. Use names that make intent obvious.
- Write abbreviations such as URL or ID in either all lowercase or all uppercase (do not use Upper Camel Case for them).
- Do not use abbreviations like `img` for `image` or `cnt` for `count`.
