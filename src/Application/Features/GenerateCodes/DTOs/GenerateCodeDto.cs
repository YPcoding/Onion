namespace Application.Features.GenerateCodes.DTOs;

public class GenerateCodeDto
{
    public List<string> Entities { get; set; } = new List<string>();
    public string NamespaceName { get; set; } = "Application.Features";
    public string BackendSavePath { get; set; }
    public string FrontendSavePath { get; set; }
}
