using System.Collections.Generic;

namespace EducationAccelerator.Models
{
    internal class ODataResponse<T>
    {
        public List<T> Value { get; set; }
    }
}
