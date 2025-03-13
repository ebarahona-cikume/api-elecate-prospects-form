using ApiElecateProspectsForm.Controllers;
using ApiElecateProspectsForm.DTOs;
using ApiElecateProspectsForm.DTOs.Errors;
using ApiElecateProspectsForm.Interfaces;
using ApiElecateProspectsForm.Models;
using System.Net;
using System.Text.Json;

namespace ApiElecateProspectsForm.Utils
{
    public class ProspectMapper(IValidateFields validateFields, IResponseHandler responseHandler) : IProspectMapper
    {
        private readonly IValidateFields _validateFields = validateFields;
        private readonly IResponseHandler _responseHandler = responseHandler;

        public object MapRequestToProspect(
            SaveFormDataRequestDTO request,
            List<FormFieldsModel> formFields,
            IMaskFormatter maskFormatter)
        {
            ProspectModel prospectModel = new();
            ProspectModel prospect = prospectModel;
            Dictionary<string, object> prospectData = [];

            List<FieldErrorDTO> errors = [];
            List<FieldErrorDTO> unmappedFields = []; // Campos no mapeados en la BD
            List<FieldErrorDTO> duplicatedFields = []; // Campos duplicados en la petición

            // Diccionario para contar ocurrencias y almacenar índices de cada campo
            Dictionary<string, List<int>> fieldOccurrences = new(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < request.Fields!.Count; i++)
            {
                FieldSaveFormRequestDTO field = request.Fields[i];

                FormFieldsModel? matchingField = formFields
                    .FirstOrDefault(f => f.Name != null && f.Name.Equals(field.Name, StringComparison.OrdinalIgnoreCase));

                if (matchingField != null)
                {
                    // validamos la longitud del campo
                    _validateFields.ValidateFieldLength(field, matchingField, i, errors);
                    
                    // Aplicar máscara si existe
                    string fieldValue = field.Value ?? "";

                    if (!string.IsNullOrEmpty(matchingField.Mask))
                    {
                        fieldValue = maskFormatter.ApplyMask(fieldValue, matchingField.Mask);
                    }

                    if (!string.IsNullOrEmpty(field.Name))
                    {
                        prospectData[field.Name] = fieldValue;

                        // Guardar la ocurrencia del campo con su índice
                        if (!fieldOccurrences.TryGetValue(field.Name, out List<int>? value))
                        {
                            fieldOccurrences[field.Name] = [i];
                        }
                        else
                        {
                            value.Add(i);
                        }
                    }
                }
                else
                {
                    // Guardar campo no mapeado con su índice
                    unmappedFields.Add(new FieldErrorDTO
                    {
                        Index = i.ToString(),
                        FieldErrors = [$"The field '{field.Name}' is not mapped in the database."]
                    });
                }
            }

            // Identificar y registrar campos duplicados
            foreach (var entry in fieldOccurrences)
            {
                if (entry.Value.Count > 1) // Si el campo apareció más de una vez
                {
                    duplicatedFields.Add(new FieldErrorDTO
                    {
                        Index = string.Join(", ", entry.Value), // Convertir los índices en string separados por comas
                        FieldErrors = [$"The field '{entry.Key}' has been sent multiple times."]
                    });
                }
            }

            foreach (FieldErrorDTO duplicatedField in duplicatedFields)
            {
                errors.Add(duplicatedField);
            }

            foreach(FieldErrorDTO unmappedField in unmappedFields)
            {
                errors.Add(unmappedField);
            }

            if (errors.Count > 0)
            {
                return _responseHandler.HandleError("Validation errors occurred",
                    HttpStatusCode.BadRequest,
                    new Exception(JsonSerializer.Serialize(errors)),
                    true,
                    errors);
            }

            return _responseHandler.HandleSuccess("Validation successful");
        }
    }
}