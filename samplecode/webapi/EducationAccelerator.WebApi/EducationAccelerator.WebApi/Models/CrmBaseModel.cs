namespace EducationAccelerator.Models
{
    public abstract class CrmBaseModel
    {
        public abstract string msk12_sourcedid { get; set; }
        public abstract string EntitySetName { get; }
        public abstract string ToJson();
    }
}
