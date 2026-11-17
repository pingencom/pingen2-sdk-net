namespace Pingen.Abstractions.Interfaces.Data;

/// <summary>
/// Data POST contract: the JSON:API <c>type</c> discriminator plus attributes.
/// </summary>
public interface IDataPost
{
    /// <summary>
    /// JSON:API resource type (e.g. <c>letters</c>). Set by the service that sends the request.
    /// </summary>
    public string Type { get; init; }
}
