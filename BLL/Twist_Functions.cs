using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public static class Twist_Functions
    {
        const int N = 3;
        public static void TurnLeft(eColors[,] mat)
        {
            eColors[,] temp = new eColors[N, N];
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    temp[j, N - i - 1] = mat[i, j];
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    mat[i, j] = temp[i, j];
        }
        public static void TurnRight(eColors[,] mat)
        {
            eColors[,] temp = new eColors[N, N];
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    temp[i, j] = mat[j, N - i - 1];
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    mat[i, j] = temp[i, j];
        }
        public static void horizontal_twist(int direction, int row, eColors[][,] bigArray)
        {
            //הפונקציה מקבלת מטריצה התחלתית שורה במטריצה לסיבוב ואת המטריצה הגדולה שמכילה את כל מטריצות הקוביה
            // הפונקציה תסובב את השורה  
            if (row >= bigArray[2].Length || row < 0)
                return;
            if (direction != 0 && direction != 1)
                return;
            eColors[] temp = new eColors[N];
            for (int j = 0; j < N; j++)
                temp[j] = bigArray[2][row, j];
            if (direction == 0)//Twist left
            {
                for (int i = 0; i < N; i++)
                {
                    bigArray[2][row, i] = bigArray[3][row, i];
                    bigArray[3][row, i] = bigArray[4][row, i];
                    bigArray[4][row, i] = bigArray[1][row, i];
                    bigArray[1][row, i] = temp[i];
                }
                if (row == 0)
                    TurnLeft(bigArray[0]);
                if (row == 2)
                    TurnLeft(bigArray[5]);

            }
            else if (direction == 1) //Twist right
            {
                for (int i = 0; i < N; i++)
                {
                    bigArray[2][row, i] = bigArray[1][row, i];
                    bigArray[1][row, i] = bigArray[4][row, i];
                    bigArray[4][row, i] = bigArray[3][row, i];
                    bigArray[3][row, i] = temp[i];
                }
                if (row == 0)
                    TurnRight(bigArray[0]);
                if (row == 2)
                    TurnRight(bigArray[5]);

            }
        }

        public static void vertical_twist(int column, int direction, eColors[][,] bigArray)
        {
            if (column >= N || column < 0)
                return;
            if (direction != 0 && direction != 1)
                return;
            for (int i = 0; i < N; i++)
            {
                eColors temp = bigArray[0][i, column];
                if (direction == 0)
                {
                    bigArray[0][i, column] = bigArray[4][i, column];
                    bigArray[4][i, column] = bigArray[5][i, column];
                    bigArray[5][i, column] = bigArray[2][i, column];
                    bigArray[2][i, column] = temp;
                    if (column == 0)
                        TurnRight(bigArray[1]);
                    if (column == 2)
                        TurnLeft(bigArray[3]);
                }
                else
                {
                    bigArray[0][i, column] = bigArray[2][i, column];
                    bigArray[2][i, column] = bigArray[5][i, column];
                    bigArray[5][i, column] = bigArray[4][i, column];
                    bigArray[4][i, column] = temp;
                    if (column == 0)
                        TurnLeft(bigArray[1]);
                    if (column == 2)
                        TurnRight(bigArray[3]);
                }
            }
        }
        public static void side_twist(int column, int direction, eColors[][,] bigArray)
        {
            for (int i = 0; i < N; i++)
            {
                eColors temp = bigArray[0][column, i];
                if (direction == 0)
                {
                    bigArray[0][column, i] = bigArray[3][i, N - column - 1];
                    bigArray[3][i, N - column - 1] = bigArray[5][N - column - 1, N - 1 - i];
                    bigArray[5][N - column - 1, N - 1 - i] = bigArray[1][N - i - 1, column];
                    bigArray[1][N - i - 1, column] = temp;
                    if (column == 0)
                        TurnLeft(bigArray[4]);
                    if (column == 2)
                        TurnLeft(bigArray[2]);
                }
                else
                {
                    bigArray[0][column, i] = bigArray[1][N - i - 1, column];
                    bigArray[1][N - i - 1, column] = bigArray[5][N - column - 1, N - 1 - i];
                    bigArray[5][N - column - 1, N - 1 - i] = bigArray[3][i, N - column - 1];
                    bigArray[3][i, N - column - 1] = temp;
                    if (column == 0)
                        TurnRight(bigArray[4]);
                    if (column == 2)
                        TurnRight(bigArray[2]);
                }

            }
        }
      
      
       
    }
}
