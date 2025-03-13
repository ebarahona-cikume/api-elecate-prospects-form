using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Interfaces.FormFieldsGenerators;
using ApiElecateProspectsForm.Models;
using System.Text;

namespace ApiElecateProspectsForm.Services.FormComponentsGenerators
{
    public class SelectFieldGenerator(HttpClient httpClient, IConfiguration configuration) : IFormFieldGenerator
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _configuration = configuration;

        public async Task<string> GenerateComponent(FieldGenerateFormRequestDTO field)
        {
            if (field is not SelectFieldRequestDTO)
            {
                throw new ArgumentException("Invalid field type");
            }

            string? maritalStatusUrl = _configuration["ApiUrls:MaritalStatusUrl"];
            
            if (string.IsNullOrEmpty(maritalStatusUrl))
            {
                throw new InvalidOperationException("MaritalStatusUrl is not configured.");
            }

            HttpResponseMessage response = await _httpClient.GetAsync(maritalStatusUrl);
            response.EnsureSuccessStatusCode();

            IEnumerable<MaritalStatusModel> maritalStatuses = await response.Content.ReadFromJsonAsync<IEnumerable<MaritalStatusModel>>() ?? [];

            StringBuilder htmlBuilder = new StringBuilder($"<select id=\"{field.Name}\" name=\"{field.Name}\">\n");

            foreach (MaritalStatusModel status in maritalStatuses)
            {
                htmlBuilder.Append($"<option value=\"{status.Id}\">{status.MaritalStatus?.Trim()}</option>\n");
            }

            htmlBuilder.Append("</select>\n");

            return htmlBuilder.ToString();
        }
    }
}
