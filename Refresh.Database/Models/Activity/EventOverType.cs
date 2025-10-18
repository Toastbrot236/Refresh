namespace Refresh.Database.Models.Activity;

// EventType was already taken and i had no better ideas on what to name this
public enum EventOverType : byte
{
    /// <summary>
    /// A recent activity event, public to everyone.
    /// </summary>
    Activity,
    /// <summary>
    /// Moderation done by either a mod to a user (e.g. level deleted),
    /// or by a user to another user (e.g. comment deleted off level).
    /// Only public to staff, the actor and the involved user (usually the publisher
    /// of the moderated content).
    /// </summary>
    Moderation,
}