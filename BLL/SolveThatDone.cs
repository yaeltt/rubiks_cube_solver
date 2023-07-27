using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    [Serializable]
    public class SolveThatDone
    {
        public string cube { get; set; }
        public List<ActionInCube> moves { get; set; }
    }
}
