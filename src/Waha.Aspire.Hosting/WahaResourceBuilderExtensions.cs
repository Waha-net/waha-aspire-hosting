using System.Diagnostics;
using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Aspire.Hosting
{
    /// <summary>
    /// Provides extension methods for building and configuring Waha resources in a distributed application.
    /// </summary>
    public static class WahaResourceBuilderExtensions
    {
        private const string WAHA_VOLUME_NAME = "waha-volume";
        private const string WAHA_VOLUME_MOUNT_PATH = "/tmp";
        private const string WAHA_DASHBOARD_PATH = "/dashboard";

        /// <summary>
        /// Adds a Waha resource to the distributed application builder.
        /// </summary>
        /// <param name="builder">The distributed application builder.</param>
        /// <param name="name">The name of the Waha resource.</param>
        /// <param name="port">The optional port for the HTTP endpoint.</param>
        /// <returns>An <see cref="IResourceBuilder{WahaResource}"/> for further configuration.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> or <paramref name="name"/> is null.</exception>
        public static IResourceBuilder<WahaResource> AddWaha(
            this IDistributedApplicationBuilder builder,
            [ResourceName] string name,
            int? port = null)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));
            ArgumentNullException.ThrowIfNull(name, nameof(name));

            var resource = new WahaResource(name);

            return builder
                .AddResource(resource)
                .WithAnnotation(new ContainerImageAnnotation { Image = WahaContainerImageTags.Image, Tag = WahaContainerImageTags.Tag, Registry = WahaContainerImageTags.Registry })
                .WithHttpEndpoint(port: port, targetPort: 3000, name: WahaResource.WahaEndpointName)
                .WithOtlpExporter()
                .WithHttpHealthCheck("/")
                .WithCommand(
                    "RunWaha",
                    "Waha Dashboard",
                    executeCommand: context => OnRunDashboardCommandAsync(builder, resource.PrimaryEndpoint.Url, context),
                    new CommandOptions()
                    {
                        ConfirmationMessage = "Waha dashboard is running. Open it in your browser?",
                        Description = "Runs the Waha dashboard in your browser.",
                        IconName = "Info",
                        IconVariant = IconVariant.Filled,
                        IsHighlighted = true,
                        UpdateState = (UpdateCommandStateContext context) => context.ResourceSnapshot.HealthStatus is HealthStatus.Healthy ? ResourceCommandState.Enabled : ResourceCommandState.Disabled
                    }
                )
                .ExcludeFromManifest();
        }

        /// <summary>
        /// Adds a data volume to the Waha resource using the default volume name.
        /// </summary>
        /// <param name="builder">The resource builder.</param>
        /// <returns>An <see cref="IResourceBuilder{WahaResource}"/> for further configuration.</returns>
        public static IResourceBuilder<WahaResource> WithDataVolume(this IResourceBuilder<WahaResource> builder)
        {
            return builder.WithVolume(WAHA_VOLUME_NAME, WAHA_VOLUME_MOUNT_PATH);
        }

        /// <summary>
        /// Adds a data volume to the Waha resource using the specified volume name.
        /// </summary>
        /// <param name="builder">The resource builder.</param>
        /// <param name="name">The name of the volume.</param>
        /// <returns>An <see cref="IResourceBuilder{WahaResource}"/> for further configuration.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/> or <paramref name="name"/> is null.</exception>
        public static IResourceBuilder<WahaResource> WithDataVolume(this IResourceBuilder<WahaResource> builder, string name)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));
            ArgumentNullException.ThrowIfNull(name, nameof(name));

            return builder.WithVolume(name, WAHA_VOLUME_MOUNT_PATH);
        }

        /// <summary>
        /// Adds a data volume to the Waha resource using the specified volume name and mount path.
        /// </summary>
        /// <param name="builder">The resource builder.</param>
        /// <param name="name">The name of the volume.</param>
        /// <param name="mountPath">The mount path for the volume.</param>
        /// <returns>An <see cref="IResourceBuilder{WahaResource}"/> for further configuration.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="builder"/>, <paramref name="name"/>, or <paramref name="mountPath"/> is null.</exception>
        public static IResourceBuilder<WahaResource> WithDataVolume(this IResourceBuilder<WahaResource> builder, string name, string mountPath)
        {
            ArgumentNullException.ThrowIfNull(builder, nameof(builder));
            ArgumentNullException.ThrowIfNull(name, nameof(name));
            ArgumentNullException.ThrowIfNull(mountPath, nameof(mountPath));

            return builder.WithVolume(name, mountPath);
        }

        /// <summary>
        /// Sets the lifetime of the Waha resource container.
        /// </summary>
        /// <param name="builder">The resource builder.</param>
        /// <param name="lifetime">The container lifetime.</param>
        /// <returns>An <see cref="IResourceBuilder{WahaResource}"/> for further configuration.</returns>
        public static IResourceBuilder<WahaResource> WithLifetime(this IResourceBuilder<WahaResource> builder, ContainerLifetime lifetime)
        {
            return builder.WithAnnotation(new ContainerLifetimeAnnotation { Lifetime = lifetime });
        }

        private static Task<ExecuteCommandResult> OnRunDashboardCommandAsync(
            IDistributedApplicationBuilder builder,
            string url,
            ExecuteCommandContext context)
        {
            Process.Start(new ProcessStartInfo { FileName = url + WAHA_DASHBOARD_PATH, UseShellExecute = true });
            return Task.FromResult(new ExecuteCommandResult() { Success = true });
        }

        internal static class WahaContainerImageTags
        {
            internal const string Registry = "docker.io";
            internal const string Image = "devlikeapro/waha";
            internal const string Tag = "latest";
        }
    }
}