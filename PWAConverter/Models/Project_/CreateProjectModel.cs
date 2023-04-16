using PWAConverter.Entities;
using System.ComponentModel.DataAnnotations;

namespace PWAConverter.Models.Project_
{
    public class CreateProjectModel
    {
        public string Name { get; set; }
        
        public string IconId { get; set; }
       
        public string ProjectDetailId { get; set; }
        

    }
}
