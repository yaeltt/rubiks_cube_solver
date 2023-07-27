using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    [Serializable]
    public class ActionInCube
    {
        public int direction { get; set; }
        public eActType actType { get; set; }
        public int index { get; set; }
        public ActionInCube(int direction, eActType actType, int index)
        {
            this.direction = direction;
            this.actType = actType;
            this.index = index;
        }

        public override string ToString()
        {
            return actType.ToString() + " " + direction.ToString() + " " + index.ToString();
        }
    }
}
