# Cirreum Runtime Identity Oidc

[![NuGet Version](https://img.shields.io/nuget/v/Cirreum.Runtime.Identity.Oidc.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.Identity.Oidc/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Cirreum.Runtime.Identity.Oidc.svg?style=flat-square&labelColor=1F1F1F&color=003D8F)](https://www.nuget.org/packages/Cirreum.Runtime.Identity.Oidc/)
[![GitHub Release](https://img.shields.io/github/v/release/cirreum/Cirreum.Runtime.Identity.Oidc?style=flat-square&labelColor=1F1F1F&color=FF3B2E)](https://github.com/cirreum/Cirreum.Runtime.Identity.Oidc/releases)
[![License](https://img.shields.io/github/license/cirreum/Cirreum.Runtime.Identity.Oidc?style=flat-square&labelColor=1F1F1F&color=F2F2F2)](https://github.com/cirreum/Cirreum.Runtime.Identity.Oidc/blob/main/LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-003D8F?style=flat-square&labelColor=1F1F1F)](https://dotnet.microsoft.com/)

**Runtime Extensions package for Cirreum Identity Oidc — the app-facing entry point for the OIDC webhook-style provisioning callback.**

## Overview

Install this package when your application uses an OIDC-compliant IdP (Descope, Auth0, Okta, etc.) that emits a pre-token webhook to a Cirreum Identity Oidc endpoint. Install the umbrella `Cirreum.Runtime.Identity` instead if you need multiple identity provider protocols (e.g. Oidc + Entra External ID).

This package contributes two extension methods:

- `builder.AddOidcIdentity(configure?)` — registers the Oidc provider and, via the optional callback, app-provided `IUserProvisioner` implementations keyed per configured instance.
- `app.MapOidcIdentity()` — maps the provisioning routes for every enabled Oidc instance.

## Installation

```
dotnet add package Cirreum.Runtime.Identity.Oidc
```

## Usage

```csharp
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

builder.AddOidcIdentity(p => p
    .AddProvisioner<ClientABorrowerProvisioner>("clientA_descope")
    .AddProvisioner<ClientBBorrowerProvisioner>("clientB_descope"));

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapOidcIdentity();

app.Run();
```

### App-provided provisioner class

Derive from the base that matches the instance's onboarding model:

```csharp
using Cirreum.Identity.Provisioning;

public sealed class ClientABorrowerProvisioner(AppDbContext db)
    : SelfServiceUserProvisionerBase<BorrowerUser> {

    protected override Task<BorrowerUser?> FindUserAsync(string externalUserId, CancellationToken ct) =>
        db.Users.FirstOrDefaultAsync(u => u.ExternalUserId == externalUserId, ct);

    protected override async Task<BorrowerUser?> CreateSelfServiceUserAsync(
        ProvisionContext context, CancellationToken ct) {
        var user = new BorrowerUser {
            ExternalUserId = context.ExternalUserId,
            Email = context.Email,
            Roles = [BorrowerRoles.Default],
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        return user;
    }
}
```

See `Cirreum.IdentityProvider` for the full provisioner hierarchy (`UserProvisionerBase<TUser>`, `InvitationUserProvisionerBase<TUser>`, `SelfServiceUserProvisionerBase<TUser>`) and `Cirreum.Identity.Oidc` for the Oidc wire contract, configuration keys, and security model.

## Configuration

```json
{
  "Cirreum": {
    "Identity": {
      "Providers": {
        "Oidc": {
          "Instances": {
            "clientA_descope": {
              "Enabled": true,
              "Route": "/auth/clientA/provision",
              "SharedSecret": "<long-random-value>",
              "AllowedAppIds": "P2Xn9Kq..."
            },
            "clientB_descope": {
              "Enabled": true,
              "Route": "/auth/clientB/provision",
              "SharedSecret": "<long-random-value>"
            }
          }
        }
      }
    }
  }
}
```

See [`Cirreum.Identity.Oidc`](https://www.nuget.org/packages/Cirreum.Identity.Oidc/) for the full per-instance settings reference.

## Dependencies

- **Cirreum.Runtime.IdentityProvider** — the `RegisterIdentityProvider<>` helper, `IIdentityBuilder`, and `IdentityProviderMapping` types
- **Cirreum.Identity.Oidc** — the OIDC registrar, handler, and settings

## Multi-protocol apps

If you need both Oidc and Entra External ID, install the umbrella `Cirreum.Runtime.Identity` instead — it exposes `builder.AddIdentity(configure?)` / `app.MapIdentity()` which register both providers and map all their routes.

## License

MIT — see [LICENSE](LICENSE).

---

**Cirreum Foundation Framework**  
*Layered simplicity for modern .NET*
