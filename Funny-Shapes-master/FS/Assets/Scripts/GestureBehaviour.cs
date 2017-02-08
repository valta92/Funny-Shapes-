using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.EventSystems;

namespace GestureRecognizer
{
 
    public class GestureBehaviour : MonoBehaviour
    {
        public bool isEnabled = true;

        public GestureLibrary library;

        public float distanceBetweenPoints = 10f;
        public int minimumPointsToRecognize = 10;
        [SerializeField]private Vector2 lastPoint = Vector2.zero;
        private int vertexCount = 0;

        public Material lineMaterial;
        public float startThickness = 0.5f;
        public float endThickness = 0.05f;
        public Color startColor = Color.red;
        public Color endColor = Color.white;

        private LineRenderer currentStrokeRenderer;
        private int currentStrokeID = -1;

        [SerializeField]private List<Point> points = new List<Point>();
        [SerializeField]private List<GameObject> strokes = new List<GameObject>();

        private Gesture gesture;
        private bool isRecognized = false;
        private Canvas canvas;
        private Vector3 screenPoint;


        public static event Action<Gesture, Result> OnGestureRecognition;

        void Update()
        {
            if (!isEnabled)
                return;
            
            if(Input.GetMouseButtonDown(0))
            {
                CreateNewStroke(Input.mousePosition);
            }
            if(Input.GetMouseButtonUp(0))
            {
                ClearGesture();
            }
        }

		private void CreateNewStroke(Vector2 point)
        {
            if (isRecognized)
            {
                ClearGesture();
            }

            lastPoint = Vector2.zero;
            currentStrokeID++;
            vertexCount = 0;

            GameObject newStroke = new GameObject();
            newStroke.name = "Stroke " + currentStrokeID;
            newStroke.transform.parent = this.transform;

            currentStrokeRenderer = newStroke.AddComponent<LineRenderer>();
            currentStrokeRenderer.SetVertexCount(0);
            currentStrokeRenderer.material = lineMaterial;
            currentStrokeRenderer.SetColors(startColor, endColor);
            currentStrokeRenderer.SetWidth(startThickness, endThickness);

            strokes.Add(newStroke);

            RegisterPoint(point);
        }

        private void RegisterPoint(Vector2 point)
        {
            screenPoint = new Vector3(point.x, Screen.height - point.y);

            if (Vector2.Distance(point, lastPoint) > distanceBetweenPoints)
            {
                points.Add(new Point(currentStrokeID, screenPoint.x, screenPoint.y));
                lastPoint = point;

                currentStrokeRenderer.SetVertexCount(++vertexCount);
                currentStrokeRenderer.SetPosition(vertexCount - 1, WorldCoordinateForGesturePoint(point));
            }
        }

        private  Vector3 WorldCoordinateForGesturePoint(Vector3 gesturePoint) 
        {
            return Camera.main.ScreenToWorldPoint(new Vector3(gesturePoint.x, gesturePoint.y, 10));
        }

        public void SaveToLibraryGesture(Gesture gesture)
        {
            
        }

		private void Recognize()
        {
            Debug.Log("Recognize");
            if (points.Count < minimumPointsToRecognize)
            {
                ClearGesture();
                return;
            }
                
            Gesture gesture = CreateGesture();
            Result result = library.Recognize(gesture);

            isRecognized = true;

            if (OnGestureRecognition != null)
                OnGestureRecognition(gesture, result);
        }
            

        private Gesture CreateGesture()
        {
            return new Gesture(points.ToArray());
        }
            
        public void ClearGesture()
        {
            vertexCount = 0;
            currentStrokeID = -1;
            points.Clear();

            for (int i = strokes.Count - 1; i >= 0; i--)
            {
                Destroy(strokes[i]);
            }

            strokes.Clear();
            isRecognized = false;
        }
            
        //OnDrag
        public void OnDrag(BaseEventData eventData)
        {
            RegisterPoint(Input.mousePosition);
        }


      
    }
       

}