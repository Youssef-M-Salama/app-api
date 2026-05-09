namespace App.Core.Enums
{
    /// <summary>
    /// Represents the lifecycle status of an <see cref="App.Core.Domain.Entities.Offer"/>.
    /// </summary>
    public enum OfferStatus
    {
        /// <summary>Submitted by donor organization, waiting for admin approval.</summary>
        Pending,

        /// <summary>Approved by admin, visible to charities.</summary>
        Approved,

        /// <summary>Rejected by admin, not visible to charities.</summary>
        Rejected,

        /// <summary>Donation completed successfully.</summary>
        Fulfilled,

        /// <summary>Offer has passed its expiry date.</summary>
        Expired
    }
}