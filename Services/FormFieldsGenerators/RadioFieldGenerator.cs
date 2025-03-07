using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Interfaces.FormFieldsGenerators;
using ApiElecateProspectsForm.Models;
using System.Text;

namespace ApiElecateProspectsForm.Services.FormFieldsGenerators
{
    public class RadioFieldGenerator(HttpClient httpClient, IConfiguration configuration) : IFormFieldGenerator
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IConfiguration _configuration = configuration;

        public async Task<string> GenerateComponent(FieldGenerateFormRequestDTO field)
        {
            if (field is not SelectFieldRequestDTO radioField)
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

            // HTML Construction
            StringBuilder htmlBuilder = new StringBuilder();

            foreach (MaritalStatusModel option in maritalStatuses)
            {
                htmlBuilder.Append($"<input type=\"radio\" id=\"{option.Id}\" name=\"{field.Name}\" value=\"{option.Id}\">\n");
                htmlBuilder.Append($"<label for=\"{option.Id}\">{option.MaritalStatus?.Trim()}</label>\n");
            }

            return htmlBuilder.ToString();
        }
    }
}
