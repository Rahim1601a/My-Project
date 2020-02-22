using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechWorld.Models
{
    public class CustomModel
    {
        public IEnumerable<ServicesModel> ServicesModel { get; set; }
        public IEnumerable<BlogModel> blogModel { get; set; }
        public IEnumerable<ProjectModel> projectModel { get; set; }
        public IEnumerable<ProjectCategoryModel> projectCategoryModel { get; set; }
    }
}