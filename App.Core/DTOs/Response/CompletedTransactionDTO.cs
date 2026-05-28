using App.Core.Enums;

namespace App.Core.DTOs.Response
{
    /// <summary>
    /// Represents a single completed (Fulfilled) transaction.
    /// Returned in the "completed transactions" list for both donors and charities.
    ///
    /// For a Donor:
    ///   - SourceType = "NeedApplication" — donor applied to a charity need and fulfilled it.
    ///   - SourceType = "OfferApplication" — a charity applied to the donor's offer and the donor accepted; charity later marked it fulfilled.
    ///
    /// For a Charity:
    ///   - SourceType = "OfferApplication" — charity applied to a donor offer and fulfilled it.
    ///   - SourceType = "NeedApplication"  — a donor applied to the charity's need; charity accepted; donor later marked it fulfilled.
    /// </summary>
    public class CompletedTransactionDTO
    {
        /// <summary>The primary key of the underlying application record.</summary>
        public Guid ApplicationId { get; set; }

        /// <summary>"NeedApplication" or "OfferApplication".</summary>
        public string SourceType { get; set; } = string.Empty;

        /// <summary>Name of the product/item that was exchanged.</summary>
        public string ProductName { get; set; } = string.Empty;

        public decimal Quantity { get; set; }
        public MeasurementUnit Unit { get; set; }

        /// <summary>The charity that was involved in this transaction.</summary>
        public string CharityName { get; set; } = string.Empty;
        public string CharityEmail { get; set; } = string.Empty;
        public string CharityPhone { get; set; } = string.Empty;
        public string CharityWhatsapp { get; set; } = string.Empty;
        public string CharityGovernorate { get; set; } = string.Empty;
        public string CharityCity { get; set; } = string.Empty;

        /// <summary>The donor organization that was involved in this transaction.</summary>
        public string DonorOrganizationName { get; set; } = string.Empty;
        public string DonorEmail { get; set; } = string.Empty;
        public string DonorPhone { get; set; } = string.Empty;
        public string DonorWhatsapp { get; set; } = string.Empty;
        public string DonorGovernorate { get; set; } = string.Empty;
        public string DonorCity { get; set; } = string.Empty;

        public string? ProductImage { get; set; }

        /// <summary>When the application was originally submitted.</summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>When the transaction was marked as fulfilled.</summary>
        public DateTime FulfillmentDate { get; set; }
    }
}
