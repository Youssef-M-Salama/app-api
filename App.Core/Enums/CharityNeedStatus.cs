namespace App.Core.Enums
{
    /// <summary>
    /// Represents the lifecycle status of a <see cref="App.Core.Domain.Entities.CharityNeed"/>.
    /// </summary>
    public enum CharityNeedStatus
    {
        /// <summary>Submitted by charity, waiting for admin approval.</summary>
        Pending,

        /// <summary>Approved by admin, visible to donor organizations.</summary>
        Approved,

        /// <summary>Rejected by admin, not visible to donors.</summary>
        Rejected,

        /// <summary>Donation completed successfully.</summary>
        Fulfilled
    }
}