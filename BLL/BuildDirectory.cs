using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BuildDirectory
    {
        public List<ActionInCube> actions { get; set; }
        public List<eActType> actionTypes { get; set; }
        public BuildDirectory()
        {
            this.actions = new List<ActionInCube>();
            this.actionTypes = new List<eActType>();
            this.actionTypes.Add(eActType.horizontal);
            this.actionTypes.Add(eActType.vertical);
            this.actionTypes.Add(eActType.side);
            buidStates();
        }
        public void buidStates()
        {
            for (int i = 0; i < 3; i++)//לולאה חיצונית שעוברת על 3 השורות של הקוביה
            {
                for (int j = 0; j < 2; j++)//לולאה שעוברת על 2 הסוגים האפשריים של הפעולה- 0-למטה, 1-למעלה
                {
                    for (int k = 0; k < 3; k++)//לולאה שעוברת על אורך הקוביה-3
                    {
                        actions.Add(new ActionInCube(j, actionTypes[i], k));
                    }
                }
            }
        }
    }
}
