namespace Microsoft.Extensions.Hosting;

using Cirreum.Identity;
using Cirreum.Identity.Configuration;
using Cirreum.Identity.Provisioning;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// App-facing extensions for registering the Cirreum Identity Oidc provider.
/// </summary>
public static class HostApplicationBuilderExtensions {

	private sealed class AddOidcIdentityMarker { }

	/// <summary>
	/// Registers the Cirreum Identity Oidc provider — a webhook-style pre-token
	/// provisioning endpoint compatible with Descope, Auth0, and any OIDC IdP that
	/// supports a custom-claims HTTP callback. Binds instances from
	/// <c>Cirreum:Identity:Providers:Oidc:Instances:*</c>.
	/// </summary>
	/// <param name="builder">The host application builder.</param>
	/// <param name="configure">
	/// Optional callback to register per-instance <see cref="IUserProvisioner"/>
	/// implementations using the fluent <see cref="IIdentityBuilder.AddProvisioner{TProvisioner}"/>
	/// method.
	/// </param>
	/// <returns>The host application builder for chaining.</returns>
	/// <example>
	/// <code>
	/// builder.AddOidcIdentity(p => p
	///     .AddProvisioner&lt;ClientABorrowerProvisioner&gt;("clientA_descope")
	///     .AddProvisioner&lt;ClientBBorrowerProvisioner&gt;("clientB_descope"));
	/// </code>
	/// </example>
	public static IHostApplicationBuilder AddOidcIdentity(
		this IHostApplicationBuilder builder,
		Action<IIdentityBuilder>? configure = null) {

		// Marker dedup — provider registration runs once regardless of repeat calls.
		// The configure callback always runs so repeated calls can still add provisioners.
		if (!builder.Services.IsMarkerTypeRegistered<AddOidcIdentityMarker>()) {
			builder.Services.MarkTypeAsRegistered<AddOidcIdentityMarker>();

			builder.RegisterIdentityProvider<
				OidcIdentityProviderRegistrar,
				OidcIdentityProviderSettings,
				OidcIdentityProviderInstanceSettings>();
		}

		configure?.Invoke(new IdentityBuilder(builder));
		return builder;
	}
}
