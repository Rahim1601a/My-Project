using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminPanel.Models
{
    public class ProjectModel
    {
        public int ID { get; set; }
        public string Tittle { get; set; }
        public string Description { get; set; }
        public int ProjectType { get; set; }
        public string ProjectTypeName { get; set; }
        public string ProjectYear { get; set; }
        public string ProjectHead { get; set; }
        public string Image { get; set; }
        public string IsActive { get; set; }
    }
}