using System;

namespace EducationAccelerator.Models
{
    public class LinkEntityData
    {
        public string[] Attributes { get; set; }
        public string[] Filters { get; set; }
        public string EntityName { get; set; }
        public Type EntityType { get; set; }
        public string JoinMapping { get; set; }
        public string Alias { get; set; }
    }
}
