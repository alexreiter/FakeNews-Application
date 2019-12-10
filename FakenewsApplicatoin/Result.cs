using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakenewsApplication
{
    class Result
    {
        public String label { get; set; }
        public double result { get; set; }

        public Result(string label, double result)
        {
            this.label = label;
            this.result = result;
        }
    }
}
