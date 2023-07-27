using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{ 
    public class BestAction
    {
        public string a_str;
        public ActionInCube action;
        public BestAction(string _a_str, ActionInCube a)
        {
            a_str = _a_str;
            action = a;
        }
    }
}
