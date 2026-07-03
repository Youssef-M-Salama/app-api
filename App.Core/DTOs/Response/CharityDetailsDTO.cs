public class CharityDetailsDTO
{
    public string CharityName { get; set; } = string.Empty;
    public string CharityDescription { get; set; } = string.Empty;
    public string? RegistrationNumber { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public string? HeadquartersAddress { get; set; }
    public string? AuthorizedPersonName { get; set; }
    public string? AuthorizedPersonPosition { get; set; }

    public string? RegistrationCertificateUrl { get; set; }
    public string? BylawsUrl { get; set; }
    public string? FoundersListUrl { get; set; }
    public string? BoardMembersListUrl { get; set; }
    public string? HeadquartersProofUrl { get; set; }
    public string? DelegationDocumentUrl { get; set; }
}