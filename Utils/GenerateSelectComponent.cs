using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Text;
using static System.Net.WebRequestMethods;

namespace ApiElecateProspectsForm.Utils
{
    public class GenerateSelectComponent : IGenerateFormComponent
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public GenerateSelectComponent(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async void GenerateComponent(StringBuilder htmlBuilder, FormFieldRequestDTO field)
        {
            try
            {
                string? maritalStatusUrl = _configuration["ApiUrls:MaritalStatusUrl"];

                HttpResponseMessage response = await _httpClient.GetAsync(maritalStatusUrl);
                response.EnsureSuccessStatusCode();

                IEnumerable<MaritalStatusModel> maritalStatuses = await response.Content.ReadFromJsonAsync<IEnumerable<MaritalStatusModel>>() ??
                    Enumerable.Empty<MaritalStatusModel>();

                htmlBuilder.Append($"<select id=\"{field.Name}\" name=\"{field.Name}\">\n");
                foreach (MaritalStatusModel status in maritalStatuses)
                {
                    htmlBuilder.Append($"<option value=\"{status.Id}\">{status.MaritalStatus?.Trim()}</option>\n");
                }
                htmlBuilder.Append("</select>\n");
            }
            catch (HttpRequestException httpEx)
            {
                ErrorResponseDTO errorResponse = new()
                {
                    Status = HttpStatusCode.InternalServerError,
                    Title = "Bad Request",
                    Message = $"Error fetching data: {httpEx.Message}"
                };
            }
            catch (Exception ex)
            {
                ErrorResponseDTO errorResponse = new()
                {
                    Status = HttpStatusCode.InternalServerError,
                    Title = "Bad Request",
                    Message = $"An unexpected error occurred: {ex.Message}"
                };
            }
        }
    }
}
