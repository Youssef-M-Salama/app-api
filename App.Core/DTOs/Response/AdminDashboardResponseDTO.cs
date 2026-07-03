namespace App.Core.DTOs.Response
{
    public class AdminDashboardResponseDTO
    {
        // ── Existing fields (unchanged) ──────────────────────────────────
        public int PendingVerifications { get; set; }
        public int PendingCharityNeeds { get; set; }
        public int PendingOffers { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveCharityNeeds { get; set; }
        public int ActiveOffers { get; set; }

        // ── Users ────────────────────────────────────────────────────────
        /// <summary>Users whose IsActive == true.</summary>
        public int ActiveUsers { get; set; }
        /// <summary>Users whose IsActive == false.</summary>
        public int SuspendedUsers { get; set; }
        /// <summary>Number of verified charity accounts.</summary>
        public int TotalCharities { get; set; }
        /// <summary>Number of verified donor-organisation accounts.</summary>
        public int TotalDonors { get; set; }

        // ── Verifications ────────────────────────────────────────────────
        /// <summary>Charities + donors whose VerificationState == Verified.</summary>
        public int TotalVerified { get; set; }
        /// <summary>Charities + donors whose VerificationState == Rejected.</summary>
        public int TotalRejectedVerifications { get; set; }

        // ── Charity Needs ────────────────────────────────────────────────
        /// <summary>CharityNeeds with Status == Rejected.</summary>
        public int RejectedCharityNeeds { get; set; }
        /// <summary>CharityNeeds with Status == Fulfilled.</summary>
        public int FulfilledCharityNeeds { get; set; }

        // ── Offers ───────────────────────────────────────────────────────
        /// <summary>Offers with Status == Rejected.</summary>
        public int RejectedOffers { get; set; }
        /// <summary>Offers with Status == Fulfilled.</summary>
        public int FulfilledOffers { get; set; }
        /// <summary>Offers with Status == Expired.</summary>
        public int ExpiredOffers { get; set; }

        // ── Applications (NeedApplications + OfferApplications) ──────────
        /// <summary>Total number of NeedApplications ever submitted.</summary>
        public int TotalNeedApplications { get; set; }
        /// <summary>NeedApplications with Status == Fulfilled.</summary>
        public int FulfilledNeedApplications { get; set; }
        /// <summary>Total number of OfferApplications ever submitted.</summary>
        public int TotalOfferApplications { get; set; }
        /// <summary>OfferApplications with Status == Fulfilled.</summary>
        public int FulfilledOfferApplications { get; set; }
    }
}
