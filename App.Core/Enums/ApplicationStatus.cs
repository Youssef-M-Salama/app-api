namespace App.Core.Enums
{
    /// <summary>
    /// Represents the status of an application — shared by
    /// <see cref="App.Core.Domain.Entities.NeedApplication"/> and
    /// <see cref="App.Core.Domain.Entities.OfferApplication"/>.
    /// </summary>
    public enum ApplicationStatus
    {
        /// <summary>Submitted, waiting for the other party to respond.</summary>
        Pending,

        /// <summary>Accepted by the receiving party.</summary>
        Accepted,

        /// <summary>Rejected by the receiving party.</summary>
        Rejected
    }
}