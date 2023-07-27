using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class PosibbleStatus
    {
        public string state { get; set; }
        public int d { get; set; }

        public Cube c { get; set; }
        public PosibbleStatus(string state, int d, Cube c)
        {
            this.state = state;
            this.d = d; 
            this.c = c;
        }
    }
}
