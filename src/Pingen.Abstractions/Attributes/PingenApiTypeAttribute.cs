namespace Pingen.Abstractions.Attributes;

/// <summary>
/// Declares the JSON:API <c>type</c> discriminator(s) an attributes model deserialises from, so the
/// <c>included</c> resolver can map a resource's <c>type</c> back to its .NET type without a hand-maintained
/// lookup table. A single model may carry several discriminators (e.g. a webhook event arrives as
/// <c>webhook_issues</c>, <c>webhook_sent</c>, ...), so the constructor takes one or more values.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class PingenApiTypeAttribute : Attribute
{
    /// <summary>
    /// Initialises the attribute with the JSON:API <c>type</c> discriminator(s) the model deserialises from.
    /// </summary>
    /// <param name="types">One or more <c>type</c> discriminator values (at least one).</param>
    public PingenApiTypeAttribute(params string[] types) => Types = types;

    /// <summary>
    /// The JSON:API <c>type</c> discriminator(s) this model deserialises from.
    /// </summary>
    public IReadOnlyList<string> Types { get; }
}
