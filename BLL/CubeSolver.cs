using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System;
using System.Net;


namespace BLL
{
    public class CubeSolver
    {
        #region משתנים
        const int max_moves = 5;

        public IDictionary<string, int> heuristic { get; set; }
        public IDictionary<string, int> fromJson { get; set; }
        List<SolveThatDone> lSolve = new List<SolveThatDone>();
        public IDictionary<string, List<ActionInCube>> solveThatDone { get; set; }
        public IDictionary<string, List<ActionInCube>> fromSolveThatDone { get; set; }
        public Stack<PosibbleStatus> stack { get; set; }
        public Cube myCube { get; set; }
        public PosibbleStatus SD { get; set; }
        public string a_str { get; set; }
        public bool status { get; set; }

        public List<ActionInCube> moves { get; set; }
        public List<ActionInCube> moves1 { get; set; }
        public int min_val { get; set; }//מכיל בתוכו את מספר הצעדים שנצרך לעשות ממצב מסויים עד לקובייה פתורה 
        public BestAction best_action { get; set; }//שמירת הפעולה הטובה ביותר מתוך כל ה-18
        public int h_score { get; set; }//מספר הצעדים שיש לעשות לא כולל הצעד שנעשה
        public int f_score { get; set; }//מספר הפעולות שיש לעשות כולל הצעד שנעשה 
        public BuildDirectory b1 { get; set; }
        public string text { get; set; }

        public CubeSolver()
        {
            myCube = new Cube();
            moves = new List<ActionInCube>();
            heuristic = new Dictionary<string, int>();//המילון שמכיל את כל המצבים האפשריים לפתרון הקוביה
            status = true;
            b1 = new BuildDirectory();
        }
        #endregion
        
        public void build_heuristic_db()
        {
            //בניית הקובץ שמכיל את מצבי הקובייה האפשריים
            Cube c1 = new Cube();
            string state = c1.matToStr();// מחרוזת המתארת את מצב הקוביה
            b1 = new BuildDirectory();
            List<ActionInCube> actions = b1.actions;//רשימה בגודל 18 שמתארת את כל הפעולות האפשריות
            SD = new PosibbleStatus(state, 0, c1);
            stack = new Stack<PosibbleStatus>();
            stack.Push(SD);
            while (stack.Count > 0)
            {
                //לולאה שעוברת עד שהמחסנית מתרוקנת
                SD = stack.Pop();
                c1 = SD.c;
                if (SD.d > max_moves)
                    continue;
                for (int i = 0; i < actions.Count; i++)
                {
                    //לולאה שעוברת על כל המצבים האפשריים
                    //בתוך הלולאה נעשה כל פעם מצב אחר, ונשמור אותה במחסנית ובמילון 
                    myCube = new Cube();
                    myCubeEquleC1(SD.c);//נחזיר את הקוביה למצב שהיתה כשהוצאנו מהמחסנית
                    if (actions[i].actType == eActType.horizontal)
                        myCube.horizontal_twist(actions[i].direction, actions[i].index);
                    else if (actions[i].actType == eActType.vertical)
                        myCube.vertical_twist(actions[i].direction, actions[i].index);
                    else if (actions[i].actType == eActType.side)
                        myCube.side_twist(actions[i].direction, actions[i].index);
                    a_str = myCube.matToStr();//המרה של הקוביה למחרוזת
                    if (SD.d == 6)
                        try

                        {
                            if (!(heuristic.ContainsKey(a_str)) && !(fromJson.ContainsKey(a_str)))
                                heuristic.Add(a_str, SD.d + 1);//דחיפה למילון
                            else if (heuristic.FirstOrDefault(x => x.Key == a_str).Value > SD.d + 1)
                                heuristic[a_str] = SD.d + 1;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            return;
                        }
                    stack.Push(new PosibbleStatus(a_str, SD.d + 1, myCube));//דחיפה למחסנית
                    if (heuristic.Count == 100000)//אם המילון הגיע ל-100000 הוספה לקובץ
                        ApendToJsonFile();
                }

            }
        }

        public void ApendToJsonFile()
        {
            //json הוספת המילון לקובץ ה 
            string json = JsonSerializer.Serialize(heuristic);
            File.AppendAllText(Path.Combine(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location), "heuristic.json"), json);
            heuristic = new Dictionary<string, int>();
        }

       
        public List<ActionInCube> run(Cube myCube)
        {
            //הפונקציה שפותרת את הקובייה

            readFromJson();
            status = false;
           status = search(myCube, b1.actions);
            if (status)
            {
              //  ApendToJson(moves);
                return moves;//הרשימה שמכילה את הפעולות שיש לבצע
            }
            else
            {
                myCube = ConvertToPCube(myCube);
                myCube.state = myCube.pythonMatToStr();
                string url = "http://127.0.0.1:5000/getresult/" + myCube.state;
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    Stream resStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(resStream);
                    string responseString = reader.ReadToEnd();
                    moves1 = ConvertToMoves(responseString);
                    checkMoves();
                }
                catch
                {
                    moves = new List<ActionInCube>();
                    
                }
            }
            return moves;
        }
        public void checkMoves()
        {
            //
            for (int i = 0; i < moves1.Count; i++)
            {
                eActType act = moves1[i].actType;
                if(act==eActType.horizontal)
                myCube.horizontal_twist(moves1[i].direction, moves1[i].index);
                else if(act==eActType.vertical)
                myCube.vertical_twist(moves1[i].direction, moves1[i].index);
                else
                myCube.side_twist(moves1[i].direction, moves1[i].index);
                if (fromJson.ContainsKey(myCube.matToStr()))
                {
                    moves.Add(moves1[i]);
                    search(myCube, b1.actions);
                    break;
                }
                else
                    moves.Add(moves1[i]);
            }
        }

        public List<ActionInCube> ConvertToMoves(string res)
        {
            //פונקצייה הממירה את המחרוזת שחזרה מהפתרון למחרוזת שמוכרת בצד הלקוח
            List<ActionInCube> moves = new List<ActionInCube>();
            string str = "";
            for (int i = 0; i < res.Length; i++)
            {
                if (res[i] != ' ')
                    str += res[i];
                 if(res[i]==' '||i==res.Length-1)
                {
                    if (str == "U")
                        moves.Add(new ActionInCube(0, eActType.horizontal, 0));
                    else if (str == "U'")
                        moves.Add(new ActionInCube(1, eActType.horizontal, 0));
                    else if (str == "U2")
                    {
                        moves.Add(new ActionInCube(0, eActType.horizontal, 0));
                        moves.Add(new ActionInCube(0, eActType.horizontal, 0));
                    }
                    else if (str == "D")
                        moves.Add(new ActionInCube(1, eActType.horizontal, 2));
                    else if (str == "D'")
                        moves.Add(new ActionInCube(0, eActType.horizontal, 2));
                    else if (str == "D2")
                    {
                        moves.Add(new ActionInCube(0, eActType.horizontal, 2));
                        moves.Add(new ActionInCube(0, eActType.horizontal, 2));
                    }
                    else if (str == "R")
                        moves.Add(new ActionInCube(1, eActType.vertical, 2));
                    else if (str == "R'")
                        moves.Add(new ActionInCube(0, eActType.vertical, 2));
                    else if (str == "R2")
                    {
                        moves.Add(new ActionInCube(0, eActType.vertical, 2));
                        moves.Add(new ActionInCube(0, eActType.vertical, 2));
                    }
                    else if (str == "L")
                        moves.Add(new ActionInCube(0, eActType.vertical, 0));
                    else if (str == "L'")
                        moves.Add(new ActionInCube(1, eActType.vertical, 0));
                    else if (str == "L2")
                    {
                        moves.Add(new ActionInCube(0, eActType.vertical, 0));
                        moves.Add(new ActionInCube(0, eActType.vertical, 0));
                    }
                    else if (str == "F")
                        moves.Add(new ActionInCube(1, eActType.side, 2));
                    else if (str == "F'")
                        moves.Add(new ActionInCube(0, eActType.side, 2));
                    else if (str == "F2")
                    {
                        moves.Add(new ActionInCube(0, eActType.side, 2));
                        moves.Add(new ActionInCube(0, eActType.side, 2));
                    }
                    else if (str == "B")
                        moves.Add(new ActionInCube(0, eActType.side, 0));
                    else if (str == "B'")
                        moves.Add(new ActionInCube(1, eActType.side, 0));
                    else if (str == "B2")
                    {
                        moves.Add(new ActionInCube(0, eActType.side, 0));
                        moves.Add(new ActionInCube(0, eActType.side, 0));
                    }
                    str = "";
                }
            }
            return moves;
        }

        public void readFromJson()
        {
            fromJson = new Dictionary<string, int>();
            text = File.ReadAllText(Path.Combine(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location), "heuristic.json"));
            fromJson = JsonSerializer.Deserialize<Dictionary<string, int>>(text);
        }

        public Cube ConvertToPCube(Cube c1)
        {
            //המרה לקובייה בסדר שונה
            Cube newCube = new Cube();
            for (int d = 0; d < 3; d++)
                for (int y = 0; y < 3; y++)
                {
                    newCube.Up[d, y] = c1.Up[d, y];
                    newCube.Left[d, y] = c1.Right[d, y];
                    newCube.Front[d, y] = c1.Front[d, y];
                    newCube.Right[d, y] = c1.Down[d, y];
                    newCube.Back[d, y] = c1.Left[d, y];
                    newCube.Down[d, y] = c1.Back[d, y];

                }
            return newCube;
        }

        public void myCubeEquleC1(Cube c1)
        {
            //הפונקציה מעבירה מטריצה אחת למטריצה אחרת מאחר והם מצביעים
            for (int d = 0; d < 3; d++)
                for (int y = 0; y < 3; y++)
                {
                    myCube.Up[d, y] = c1.Up[d, y];
                    myCube.Down[d, y] = c1.Down[d, y];
                    myCube.Left[d, y] = c1.Left[d, y];
                    myCube.Right[d, y] = c1.Right[d, y];
                    myCube.Front[d, y] = c1.Front[d, y];
                    myCube.Back[d, y] = c1.Back[d, y];
                }
        }
        public bool search(Cube c1,  List<ActionInCube> actions)
        { 
            //הפונקציה שמחפשת בעץ המשחק- בקובץ
            myCube = c1;
            myCube.state = myCube.matToStr();
            if (myCube.isSolvedCube())//true אם הקוביה פתורה יוחזר הערך
                return true;
            if (!fromJson.ContainsKey(myCube.state))
                return false;
            min_val = fromJson.FirstOrDefault(x => x.Key == myCube.state).Value;
            best_action = null;
            int i;
            for (i = 0; i < actions.Count; i++)
            {
                //לולאה שעוברת על כל המצבים האפשריים
                //בתוך הלולאה נעשה כל פעם מצב אחר,
                //ונבדוק האם לאחר השינוי שנעשה מספר הפעולות שנשאר לעשות קטן ממספר הפעולות שעלינו לעשות על המצב שיצא מהמחסנית 
                myCube = new Cube();
                myCubeEquleC1(c1);
                if (actions[i].actType == eActType.horizontal)
                    myCube.horizontal_twist(actions[i].direction, actions[i].index);
                else if (actions[i].actType == eActType.vertical)
                    myCube.vertical_twist(actions[i].direction, actions[i].index);
                else if (actions[i].actType == eActType.side)
                    myCube.side_twist(actions[i].direction, actions[i].index);
                a_str = myCube.matToStr();
                myCube.state = a_str;
                if (myCube.isSolvedCube())
                {
                    moves.Add(actions[i]);
                    return true;
                }
                if (fromJson.Keys.Contains(a_str))
                    //מספר הצעדים שיש לעשות לא כולל הצעד שנעשה
                    h_score = fromJson.FirstOrDefault(x => x.Key == a_str).Value;
                else
                    continue;
                //מספר הפעולות שיש לעשות כולל הצעד שנעשה 
                f_score = 1 + h_score;
                if (f_score < min_val)
                    //לוודא שלא חרגתי ממספר הפעולות המינימלי שאפשר לעשות למצב זה
                {
                    min_val = f_score;
                    best_action = new BestAction(a_str, actions[i]);
                }
                else if (f_score == min_val)
                    if (best_action == null)
                        best_action = new BestAction(a_str, actions[i]);
                if (best_action != null)
                {
                    moves.Add(actions[i]);
                    status = search(myCube,actions);
                    if (status)
                        return true;
                }
            }

            return false;
        }
    }
}


