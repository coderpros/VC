using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VC.Shared.Models
{
    public class Country
    {
        public bool Loaded { get; set; } = false;

        public int Res_Id { get; set; } = 0;

        public int Umb_Id { get; set; } = 0;

        public string Name { get; set; } = "";
    }
}
