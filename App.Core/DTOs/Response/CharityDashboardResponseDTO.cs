namespace App.Core.DTOs.Response
{
    /// <summary>
    /// Dashboard statistics for the authenticated charity user.
    /// </summary>
    public class CharityDashboardResponseDTO
    {
        // ── CharityNeed counts ────────────────────────────────────────────────
        /// <summary>Total charity needs posted by this charity (all statuses).</summary>
        public int TotalCharityNeeds { get; set; }

        /// <summary>Charity needs currently waiting for admin approval.</summary>
        public int PendingCharityNeeds { get; set; }

        /// <summary>Charity needs approved by admin and visible to donors.</summary>
        public int ApprovedCharityNeeds { get; set; }

        /// <summary>Charity needs rejected by admin.</summary>
        public int RejectedCharityNeeds { get; set; }

        /// <summary>Charity needs that have been fulfilled.</summary>
        public int FulfilledCharityNeeds { get; set; }

        // ── NeedApplications received (donors applied to MY needs) ───────────
        /// <summary>Total applications received from donor organizations.</summary>
        public int TotalNeedApplicationsReceived { get; set; }

        /// <summary>Applications waiting for the charity to accept or reject.</summary>
        public int PendingNeedApplicationsReceived { get; set; }

        /// <summary>Applications accepted by this charity.</summary>
        public int AcceptedNeedApplicationsReceived { get; set; }

        /// <summary>Applications rejected by this charity.</summary>
        public int RejectedNeedApplicationsReceived { get; set; }

        // ── OfferApplications sent (I applied to donor offers) ───────────────
        /// <summary>Total applications this charity sent to donor offers.</summary>
        public int TotalOfferApplicationsSent { get; set; }

        /// <summary>Offer applications still waiting for the donor to respond.</summary>
        public int PendingOfferApplicationsSent { get; set; }

        /// <summary>Offer applications accepted by the donor.</summary>
        public int AcceptedOfferApplicationsSent { get; set; }

        /// <summary>Offer applications rejected by the donor.</summary>
        public int RejectedOfferApplicationsSent { get; set; }
    }
}