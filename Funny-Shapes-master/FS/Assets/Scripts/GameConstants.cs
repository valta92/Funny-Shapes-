using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GameConstants
{
    public class Score
    {
        public const string scoreTemplate = "Score : ";

        public static readonly Dictionary<int, int> AddScore = new Dictionary<int, int>()
        {
            { 0, 3 },
            { 1, 2 },
            { 2, 1 }
        };

    }
    public class Timer
    {
        public const string timerTemplate = "Time : ";

        public static readonly Dictionary<int, int> TimeInTheTimer = new Dictionary<int, int>()
        {
            { 0, 60 },
            { 1, 45 },
            { 2, 30 }
        };

        public static readonly Dictionary<int, int> AddTime = new Dictionary<int, int>()
        {
            { 0, 3 },
            { 1, 2 },
            { 2, 1 }
        };
    }
    public class DrawGesture
    {
        public const string textTemplate = "Draw a ";
    }

    public class GestureLibrary
    {
        public const string xmlFileName = "Shapes";

        public static readonly string[] ShapesNames = { "circle", "triangle", "rectangle"};
    }

 
     
}

public enum DifficultyEnum
{
    Easy = 0,
    Medium = 1,
    Hard = 2,
}