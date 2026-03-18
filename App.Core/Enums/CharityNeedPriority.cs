namespace App.Core.Enums
{
    /// <summary>
    /// Represents the priority level of a <see cref="App.Core.Domain.Entities.CharityNeed"/>.
    /// </summary>
    public enum CharityNeedPriority
    {
        /// <summary>Immediate need, critical shortage.</summary>
        Urgent,

        /// <summary>Important need, needed soon.</summary>
        High,

        /// <summary>Standard need, no immediate urgency.</summary>
        Normal,

        /// <summary>Low priority, can wait.</summary>
        Low
    }
}