namespace App.Core.Enums
{
    /// <summary>
    /// Represents the storage folder category for uploaded PDF files.
    /// Maps to a subfolder inside wwwroot/pdfs/.
    /// </summary>
    public enum PdfFolder
    {
        CommercialRegister,
        TaxCard,
        BusinessLicense,
        CivilProtectionApproval,
        EnvironmentalApproval,
        OwnershipContract,
        RegistrationCertificate,
        Bylaws,
        FoundersList,
        BoardMembersList,
        HeadquartersProof,
        DelegationDocument
    }
}
