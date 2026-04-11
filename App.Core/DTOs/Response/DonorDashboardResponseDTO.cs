using System;
using System.Collections.Generic;
using System.Text;

namespace App.Core.DTOs.Response
{
    public class DonorDashboardResponseDTO
    {
        // ── Offer counts ──────────────────────────────────────────────────────
        /// <summary>Total offers posted by this donor (all statuses).</summary>
        public int TotalOffers { get; set; }

        /// <summary>Offers waiting for admin approval.</summary>
        public int PendingOffers { get; set; }

        /// <summary>Offers approved by admin and visible to charities.</summary>
        public int ApprovedOffers { get; set; }

        /// <summary>Offers rejected by admin.</summary>
        public int RejectedOffers { get; set; }

        /// <summary>Offers that have been fulfilled.</summary>
        public int FulfilledOffers { get; set; }

        /// <summary>Offers that have passed their expiry date.</summary>
        public int ExpiredOffers { get; set; }

        // ── OfferApplications received (charities applied to MY offers) ───────
        /// <summary>Total applications received from charities.</summary>
        public int TotalOfferApplicationsReceived { get; set; }

        /// <summary>Applications waiting for the donor to accept or reject.</summary>
        public int PendingOfferApplicationsReceived { get; set; }

        /// <summary>Applications accepted by this donor.</summary>
        public int AcceptedOfferApplicationsReceived { get; set; }

        /// <summary>Applications rejected by this donor.</summary>
        public int RejectedOfferApplicationsReceived { get; set; }

        // ── NeedApplications sent (I applied to charity needs) ───────────────
        /// <summary>Total applications this donor sent to charity needs.</summary>
        public int TotalNeedApplicationsSent { get; set; }

        /// <summary>Need applications still waiting for the charity to respond.</summary>
        public int PendingNeedApplicationsSent { get; set; }

        /// <summary>Need applications accepted by the charity.</summary>
        public int AcceptedNeedApplicationsSent { get; set; }

        /// <summary>Need applications rejected by the charity.</summary>
        public int RejectedNeedApplicationsSent { get; set; }
    }

}
