namespace Olx.Models;

public class EnterpriseUser
{
    public string CompanyName { get; set; }

    public string StreetAddress { get; set; }

    public string ZipCode { get; set; }
    
    public string CityAndRegion { get; set; }

    public string RegistrationId { get; set; } // ЄДРПОУ
    
    public string TaxId { get; set; } // ІПН

    public bool IsVatPayer { get; set; }

    public string RepresenterName { get; set; }

    public string ContactName { get; set; }
    
    public string ContactPhone { get; set; }
    
    public string ContactEmail { get; set; }
}