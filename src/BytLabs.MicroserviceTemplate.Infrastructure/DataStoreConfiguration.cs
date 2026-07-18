namespace BytLabs.MicroserviceTemplate.Infrastructure;

/// <summary>
/// Selects the persistence store. Bound from the <c>DataStore</c> configuration section.
/// </summary>
public sealed class DataStoreConfiguration
{
    /// <summary>"Mongo" (default) or "Postgres".</summary>
    public string Provider { get; set; } = "Mongo";
}
