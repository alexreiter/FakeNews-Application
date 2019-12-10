using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FakenewsApplication
{
    class Vector
    {
        public String label { get; set; }
        public int[] vector { get; set; }

        public Vector(string label, int[] vector)
        {
            this.label = label;
            this.vector = vector;
        }
    }
}
