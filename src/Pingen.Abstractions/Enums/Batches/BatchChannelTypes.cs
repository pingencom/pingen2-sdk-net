namespace Pingen.Abstractions.Enums.Batches;

/// <summary>
/// Delivery channels a batch can dispatch through. Set <c>BatchCreate.ChannelType</c> to pick one when
/// creating a batch, and read it back from <c>Batch.ChannelType</c>.
/// </summary>
public static class BatchChannelTypes
{
    /// <summary>
    /// Physical mail - the default channel when none is set.
    /// </summary>
    public const string Post = "post";

    /// <summary>
    /// Swiss eBill delivery.
    /// </summary>
    public const string Ebill = "ebill";

    /// <summary>
    /// Email delivery.
    /// </summary>
    public const string Email = "email";
}
