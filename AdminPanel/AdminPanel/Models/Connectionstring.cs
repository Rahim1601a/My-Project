using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AdminPanel.Models
{
    public class Connectionstring
    {
        public string Constr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
    }
}