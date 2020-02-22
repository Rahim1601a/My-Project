using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TechWorld.Models
{
    public class BlogModel
    {
        public int ID { get; set; }
        public string Tittle { get; set; }
        public string CreateBy { get; set; }
        public string BlogContent { get; set; }
        public string Image { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public string IsActive { get; set; }
    }
}