using UnityEngine;
using System;

namespace GestureRecognizer
{
	[Serializable]
    public class Gesture
    {
        [SerializeField]public string Name;
        [SerializeField]public bool IsShown;
        [SerializeField]public Point[] NormalizedPoints;
        [SerializeField]public Point[] OriginalPoints;
        [SerializeField] public int numberOfPoints = 64;


        public Gesture(Point[] points, string name = "")
        {
            IsShown = false;
            Name = name;
            OriginalPoints = points;
            NormalizedPoints = points;
            Normalize();


            for(int i = 0; i < points.Length; i ++)
            {
                Debug.Log(points[i].Position);
            }
        }


        public void Normalize()
        {
            Scale();
            TranslateToCenter();
            Resample();
        }

        public void Scale()
        {
            float minx = float.MaxValue, miny = float.MaxValue, maxx = float.MinValue, maxy = float.MinValue;
            for (int i = 0; i < this.NormalizedPoints.Length; i++)
            {
                if (minx > this.NormalizedPoints[i].Position.x)
                    minx = this.NormalizedPoints[i].Position.x;
                if (miny > this.NormalizedPoints[i].Position.y)
                    miny = this.NormalizedPoints[i].Position.y;
                if (maxx < this.NormalizedPoints[i].Position.x)
                    maxx = this.NormalizedPoints[i].Position.x;
                if (maxy < this.NormalizedPoints[i].Position.y)
                    maxy = this.NormalizedPoints[i].Position.y;
            }

            Point[] scaledPoints = new Point[this.NormalizedPoints.Length];
            float scale = Math.Max(maxx - minx, maxy - miny);

            for (int i = 0; i < this.NormalizedPoints.Length; i++)
            {
                scaledPoints[i] = new Point(this.NormalizedPoints[i].StrokeID, (this.NormalizedPoints[i].Position.x - minx) / scale, (this.NormalizedPoints[i].Position.y - miny) / scale);
            }

            this.NormalizedPoints = scaledPoints;
        }

        public void TranslateToCenter()
        {
            Vector2 p = this.GetCenter();
            Point[] translatedPoints = new Point[this.NormalizedPoints.Length];

            for (int i = 0; i < this.NormalizedPoints.Length; i++)
            {
                translatedPoints[i] = new Point(this.NormalizedPoints[i].StrokeID, this.NormalizedPoints[i].Position.x - p.x, this.NormalizedPoints[i].Position.y - p.y);
            }

            this.NormalizedPoints = translatedPoints;
        }
       
        public void Resample()
        {
            Point[] resampledPoints = new Point[numberOfPoints];
            resampledPoints[0] = new Point(this.NormalizedPoints[0].StrokeID, this.NormalizedPoints[0].Position);
            int n = 1;

            float increment = GetPathLength() / (numberOfPoints - 1);
            float distanceCovered = 0;

            for (int i = 1; i < this.NormalizedPoints.Length; i++)
            {

                if (this.NormalizedPoints[i].StrokeID == this.NormalizedPoints[i - 1].StrokeID)
                {
                    float distance = Vector2.Distance(this.NormalizedPoints[i - 1].Position, this.NormalizedPoints[i].Position);

                    if (distanceCovered + distance >= increment)
                    {

                        Point firstPoint = this.NormalizedPoints[i - 1];

                        while (distanceCovered + distance >= increment)
                        {

                            float t = Mathf.Min(Mathf.Max((increment - distanceCovered) / distance, 0.0f), 1.0f);

                            if (float.IsNaN(t))
                                t = 0.5f;

                            resampledPoints[n++] = new Point(
                                this.NormalizedPoints[i].StrokeID,
                                (1.0f - t) * firstPoint.Position.x + t * this.NormalizedPoints[i].Position.x,
                                (1.0f - t) * firstPoint.Position.y + t * this.NormalizedPoints[i].Position.y
                            );

                            distance = distanceCovered + distance - increment;
                            distanceCovered = 0;
                            firstPoint = resampledPoints[n - 1];
                        }

                        distanceCovered = distance;

                    }
                    else
                        distanceCovered += distance;
                }
            }

            if (n == numberOfPoints - 1)
            {
                resampledPoints[n++] = new Point(
                    this.NormalizedPoints[this.NormalizedPoints.Length - 1].StrokeID,
                    this.NormalizedPoints[this.NormalizedPoints.Length - 1].Position.x,
                    this.NormalizedPoints[this.NormalizedPoints.Length - 1].Position.y
                );
            }

            this.NormalizedPoints = resampledPoints;
        }

        // get center 
        public Vector2 GetCenter()
        {
            Vector2 total = Vector2.zero;

            for (int i = 0; i < this.NormalizedPoints.Length; i++)
            {
                total += this.NormalizedPoints[i].Position;
            }
            return new Vector2(total.x / this.NormalizedPoints.Length, total.y / this.NormalizedPoints.Length);
        }

        public float GetPathLength()
        {
            float length = 0;

            for (int i = 1; i < this.NormalizedPoints.Length; i++)
            {
                if (this.NormalizedPoints[i].StrokeID == this.NormalizedPoints[i - 1].StrokeID)
                {
                    length += Vector2.Distance(this.NormalizedPoints[i - 1].Position, this.NormalizedPoints[i].Position);
                }
            }

            return length;
        }


        public override string ToString()
        {
            string result = this.Name;

            for (int i = 0; i < this.NormalizedPoints.Length; i++)
            {
                result += "\n" + this.NormalizedPoints[i];
            }

            return result;
        }
    }
}