namespace Pingen.Abstractions.Interfaces.Data;

/// <summary>
/// Interface for data objects to patch to the API
/// </summary>
public interface IDataPatch : IDataPost
{
    /// <summary>
    /// Id of the object
    /// </summary>
    public string Id { get; init; }
}
