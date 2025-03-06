using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Interfaces.FormFieldsGenerators;
using ApiElecateProspectsForm.Models;
using System.Text;

namespace ApiElecateProspectsForm.Services.FormFieldsGenerators
{
    public class RadioFieldGenerator : IFormFieldGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RadioFieldGenerator(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string> GenerateComponent(FormFieldRequestDTO field)
        {
            if (field is not SelectFieldRequestDTO radioField)
                throw new ArgumentException("Invalid field type");

            string? maritalStatusUrl = _configuration["ApiUrls:MaritalStatusUrl"];

            if (string.IsNullOrEmpty(maritalStatusUrl))
            {
                throw new InvalidOperationException("MaritalStatusUrl is not configured.");
            }

            HttpResponseMessage response = await _httpClient.GetAsync(maritalStatusUrl);
            response.EnsureSuccessStatusCode();

            IEnumerable<MaritalStatusModel> maritalStatuses = await response.Content.ReadFromJsonAsync<IEnumerable<MaritalStatusModel>>() ?? [];

            // Construcción del HTML
            var htmlBuilder = new StringBuilder();
            foreach (var option in maritalStatuses)
            {
                htmlBuilder.Append($"<input type=\"radio\" id=\"{option.Id}\" name=\"{field.Name}\" value=\"{option.Id}\">\n");
                htmlBuilder.Append($"<label for=\"{option.Id}\">{option.MaritalStatus?.Trim()}</label>\n");
            }

            return htmlBuilder.ToString();
        }
    }
}
