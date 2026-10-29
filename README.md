# Orabot

Orabot is an OpenRA community Discord bot, hosted as a Windows console application.

## Structure

The application is a single-process modular monolith organized into vertical feature slices. This groundwork groups the existing implementation by capability; it preserves runtime behavior, command definitions, configuration and dependency versions.

```text
Orabot.sln
src/
  Orabot.Hosts.ConsoleHost/   Console entry point and dependency composition
  Orabot.Bot/
    Features/
      Documentation/        Documentation commands, autocomplete, cache and rendering
      General/              General commands, information and mod-timer responses
      GitHubReferences/     Issue and pull-request references
      Logs/                 Log attachments and stack traces
      Maps/                 Map references, previews and announcements
      Quotes/               Message and custom quotations
      Replays/              Replay attachments, metadata and rendering
      Roles/                Self-assigned and reaction-assigned roles
    Discord/                Connection lifecycle, dispatch and shared Discord contracts
    Infrastructure/         Shared background-work contracts and serialization helpers
    DependencyInjection/    Bot service registration
```

Feature-specific handlers, models and rendering live together. Shared Discord dispatch contracts are in the bot project; the former `Orabot.Core.Abstractions` project has been absorbed into it.

The current feature relationships are preserved:

- General help uses the Quotes module name to group quote commands.
- Replay rendering uses the Maps renderer to include map information.
- Logs and Replays share the attachment-handler base in `Discord/Messages`.

These folders establish ownership and navigation. Further dependency and responsibility improvements can be made within individual feature changes.

## Build

The projects currently target .NET 6 and use Discord.Net 3.11.0. Use a .NET SDK capable of building `net6.0` projects.

From the repository root:

```powershell
dotnet restore Orabot.sln
dotnet build Orabot.sln --configuration Release --no-restore
```

Build output for the executable is under `src/Orabot.Hosts.ConsoleHost/bin/Release/net6.0/`.

## Current runtime behavior

Configuration still loads the required `appsettings.json` from the process working directory, followed by optional User Secrets. Matching User Secrets override JSON settings. Keep credentials out of source control.

When running the bot from source, use `src/Orabot.Hosts.ConsoleHost` as the working directory so its existing configuration file is found. A .NET 6 runtime is required to run the framework-dependent output. Running the host connects to Discord using its configured bot token.

The host retains its existing stdin-controlled lifetime. Prefix commands and custom slash/autocomplete dispatch remain in place. Slash-command publication remains disabled during normal startup. Passive message and reaction behavior, polling workers, service lifetimes and registration order are unchanged.

Generic Host, the modern interaction framework, platform upgrades and migration of remaining prefix commands to slash commands are subsequent refactor stages.
