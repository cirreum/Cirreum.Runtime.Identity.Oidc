# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is **Cirreum.Runtime.Identity.Oidc**, the Runtime Extensions package for the Cirreum Identity Oidc provider. Contributes two app-facing extension methods that wrap the config-driven registration plumbing from `Cirreum.Runtime.IdentityProvider`:

- `AddOidcIdentity()` on `IHostApplicationBuilder` — services-phase registration of the Oidc registrar + optional `IIdentityBuilder` callback for provisioner registration.
- `MapOidcIdentity()` on `IEndpointRouteBuilder` — endpoints-phase mapping, filtered to Oidc instances only.

## Build Commands

```bash
dotnet build Cirreum.Runtime.Identity.Oidc.slnx
dotnet pack --configuration Release
```

## Architecture

### What this package does

1. **`AddOidcIdentity(builder, configure?)`** (`Extensions/Hosting/HostApplicationBuilderExtensions.cs`)
   - Marker-type dedup via `AddOidcIdentityMarker` — provider registration runs once even across repeat calls.
   - Calls `builder.RegisterIdentityProvider<OidcIdentityProviderRegistrar, OidcIdentityProviderSettings, OidcIdentityProviderInstanceSettings>()` from Layer 4.
   - Invokes the optional `Action<IIdentityBuilder>` callback so apps can call `.AddProvisioner<T>(key)` per configured instance.
   - Namespace `Microsoft.Extensions.Hosting` so consumers get it for free.

2. **`MapOidcIdentity(endpoints)`** (`Extensions/Builder/EndpointRouteBuilderExtensions.cs`)
   - Resolves `IEnumerable<IdentityProviderMapping>` from DI.
   - Filters to `ProviderName == "Oidc"` and invokes each mapping's deferred `Map(endpoints)` closure.
   - Namespace `Microsoft.AspNetCore.Builder` (where Map*/MapGet live by convention).

### What this package does NOT do

- **Does not duplicate or re-implement the Oidc registrar, handler, or settings types** — those all live in `Cirreum.Identity.Oidc` (Infra layer).
- **Does not register `IUserProvisioner`** — that's the app's job, via the `IIdentityBuilder.AddProvisioner<T>(key)` callback.

## Project Structure

```
src/Cirreum.Runtime.Identity.Oidc/
├── Extensions/
│   ├── Hosting/
│   │   └── HostApplicationBuilderExtensions.cs   # AddOidcIdentity
│   └── Builder/
│       └── EndpointRouteBuilderExtensions.cs     # MapOidcIdentity
└── Cirreum.Runtime.Identity.Oidc.csproj
```

`RootNamespace` = `Cirreum.Runtime` (matches sibling Runtime Extensions packages), but extension classes override to `Microsoft.Extensions.Hosting` / `Microsoft.AspNetCore.Builder` for discoverability.

## Dependencies

- **Cirreum.Runtime.IdentityProvider** (v1.0.1+) — registration helper, `IIdentityBuilder` + `IdentityBuilder`, `IdentityProviderMapping`
- **Cirreum.Identity.Oidc** (v1.0.0+) — Oidc registrar + settings types (referenced by the `RegisterIdentityProvider<>` generic arguments)
- **Microsoft.AspNetCore.App**

## Umbrella vs per-protocol

The umbrella `Cirreum.Runtime.Identity` exposes `AddIdentity()` / `MapIdentity()` which compose this package and `Cirreum.Runtime.Identity.EntraExternalId`. Apps that need only OIDC install this package directly; apps that need multi-protocol install the umbrella.

Installing both per-protocol + umbrella is not supported — method-name distinction means both sets of methods are in scope at compile time but the umbrella's `AddIdentity()` already calls the per-protocol `AddOidcIdentity()` internally, so duplicate calls are defensive-no-op'd by the marker-type dedup.

## Development Notes

- Uses .NET 10.0 with latest C# language version
- Nullable reference types enabled
- Thin wrapper around Layer 4's `RegisterIdentityProvider<>` helper — no protocol-specific logic lives here
- File-scoped namespaces
- K&R braces, tabs for indentation (matches repo `.editorconfig`)
