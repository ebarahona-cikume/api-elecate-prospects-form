namespace ApiElecateProspectsForm.Models
{
    public class FormFieldsModel
    {
        public int Id { get; set; }

        public Guid IdForm { get; set; }

        public string? Type { get; set; }
        
        public string? Name { get; set; }

        public int? Size { get; set; }

        public string? Mask { get; set; }

        public int Link { get; set; }

        public string? Relation { get; set; }

        public bool IsDeleted { get; set; }
    }
}
