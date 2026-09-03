using Aspire.Hosting.ApplicationModel;

namespace Waha.Aspire.Hosting
{
    /// <summary>
    /// Represents a Waha resource which is a type of container resource with a connection string.
    /// </summary>
    public sealed class WahaResource : ContainerResource, IResourceWithConnectionString
    {
        /// <summary>
        /// The name of the endpoint for Waha.
        /// </summary>
        internal const string WahaEndpointName = "http";

        private EndpointReference? _primaryEndpointReference;

        /// <summary>
        /// Initializes a new instance of the <see cref="WahaResource"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name of the resource.</param>
        public WahaResource(string name) : base(name)
        {
        }

        /// <summary>
        /// Gets the primary endpoint reference for the Waha resource.
        /// </summary>
        public EndpointReference PrimaryEndpoint => _primaryEndpointReference ??= new(this, WahaEndpointName);

        /// <summary>
        /// Gets the connection string expression for the Waha resource.
        /// </summary>
        public ReferenceExpression ConnectionStringExpression => ReferenceExpression.Create($"Endpoint={PrimaryEndpoint.Property(EndpointProperty.Scheme)}://{PrimaryEndpoint.Property(EndpointProperty.Host)}:{PrimaryEndpoint.Property(EndpointProperty.Port)}");
    }
}