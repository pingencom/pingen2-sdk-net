using Pingen.Abstractions.Models.Base;

namespace Pingen.Abstractions.Models.LetterEvents;

/// <summary>
/// Letter data object
/// </summary>
public sealed record LetterEventData : Data<LetterEvent, LetterEventRelationships>;
