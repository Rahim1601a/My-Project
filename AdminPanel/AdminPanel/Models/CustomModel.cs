using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdminPanel.Models
{
    public class CustomModel
    {
        public List<ServicesModel> ServicesModel { get; set; }
        public List<BlogModel> blogModel { get; set; }
        public List<ProjectModel> projectModel { get; set; }
    }
}