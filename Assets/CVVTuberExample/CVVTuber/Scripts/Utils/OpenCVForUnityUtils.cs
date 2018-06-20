using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

using OpenCVForUnity;
using DlibFaceLandmarkDetector;

namespace CVVTuber
{
    /// <summary>
    /// Utility class for the integration of DlibFaceLandmarkDetector and OpenCVForUnity.
    /// </summary>
    public static class OpenCVForUnityUtils
    {
        /// <summary>
        /// Sets a image.
        /// </summary>
        /// <param name="faceLandmarkDetector">Face landmark detector.</param>
        /// <param name="imgMat">Image mat.</param>
        public static void SetImage (FaceLandmarkDetector faceLandmarkDetector, Mat imgMat)
        {
            if (!imgMat.isContinuous ()) {
                throw new ArgumentException ("imgMat.isContinuous() must be true.");
            }
            faceLandmarkDetector.SetImage ((IntPtr)imgMat.dataAddr (), imgMat.width (), imgMat.height (), (int)imgMat.elemSize ());
        }

        /// <summary>
        /// Draws a face rect.
        /// </summary>
        /// <param name="imgMat">Image mat.</param>
        /// <param name="rect">Rect.</param>
        /// <param name="color">Color.</param>
        /// <param name="thickness">Thickness.</param>
        public static void DrawFaceRect (Mat imgMat, UnityEngine.Rect rect, Scalar color, int thickness)
        {
            Imgproc.rectangle (imgMat, new Point (rect.xMin, rect.yMin), new Point (rect.xMax, rect.yMax), color, thickness);
        }

        /// <summary>
        /// Draws a face rect.
        /// </summary>
        /// <param name="imgMat">Image mat.</param>
        /// <param name="rect">RectDetection.</param>
        /// <param name="color">Color.</param>
        /// <param name="thickness">Thickness.</param>
        public static void DrawFaceRect (Mat imgMat, DlibFaceLandmarkDetector.FaceLandmarkDetector.RectDetection rect, Scalar color, int thickness)
        {            
            UnityEngine.Rect _rect = rect.rect;
            Imgproc.putText (imgMat, "detection_confidence : " + rect.detection_confidence, new Point (_rect.xMin, _rect.yMin - 20), Core.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar (255, 255, 255, 255), 1, Imgproc.LINE_AA, false);
            Imgproc.putText (imgMat, "weight_index : " + rect.weight_index, new Point (_rect.xMin, _rect.yMin - 5), Core.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar (255, 255, 255, 255), 1, Imgproc.LINE_AA, false);
            Imgproc.rectangle (imgMat, new Point (_rect.xMin, _rect.yMin), new Point (_rect.xMax, _rect.yMax), color, thickness);
        }

        /// <summary>
        /// Draws a face rect.
        /// </summary>
        /// <param name="imgMat">Image mat.</param>
        /// <param name="rect">Detected object's data. [left, top, width, height, detection_confidence, weight_index]</param>
        /// <param name="color">Color.</param>
        /// <param name="thickness">Thickness.</param>
        public static void DrawFaceRect (Mat imgMat, double[] rect, Scalar color, int thickness)
        {            
            if (rect.Length > 4)
                Imgproc.putText (imgMat, "detection_confidence : " + rect[4], new Point (rect[0], rect[1] - 20), Core.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar (255, 255, 255, 255), 1, Imgproc.LINE_AA, false);
            if (rect.Length > 5)
                Imgproc.putText (imgMat, "weight_index : " + rect[5], new Point (rect[0], rect[1] - 5), Core.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar (255, 255, 255, 255), 1, Imgproc.LINE_AA, false);
            Imgproc.rectangle (imgMat, new Point (rect[0], rect[1]), new Point (rect[0] + rect[2], rect[1] + rect[3]), color, thickness);
        }

        /// <summary>
        /// Draws a face landmark.
        /// This method supports 68 landmark points.
        /// </summary>
        /// <param name="imgMat">Image mat.</param>
        /// <param name="points">Points.</param>
        /// <param name="color">Color.</param>
        /// <param name="thickness">Thickness.</param>
        /// <param name="drawIndexNumbers">Determines if draw index numbers.</param>
        public static void DrawFaceLandmark (Mat imgMat, List<Vector2> points, Scalar color, int thickness, bool drawIndexNumbers = false)
        {
            if (points.Count == 5) {

                Imgproc.line (imgMat, new Point (points [0].x, points [0].y), new Point (points [1].x, points [1].y), color, thickness);
                Imgproc.line (imgMat, new Point (points [1].x, points [1].y), new Point (points [4].x, points [4].y), color, thickness);
                Imgproc.line (imgMat, new Point (points [4].x, points [4].y), new Point (points [3].x, points [3].y), color, thickness);
                Imgproc.line (imgMat, new Point (points [3].x, points [3].y), new Point (points [2].x, points [2].y), color, thickness);

            } else if (points.Count == 68) {
                
                for (int i = 1; i <= 16; ++i)
                    Imgproc.line (imgMat, new Point (points [i].x, points [i].y), new Point (points [i - 1].x, points [i - 1].y), color, thickness);
                
                for (int i = 28; i <= 30; ++i)
                    Imgproc.line (imgMat, new Point (points [i].x, points [i].y), new Point (points [i - 1].x, points [i - 1].y), color, thickness);
                
                for (int i = 18; i <= 21; ++i)
                    Imgproc.line (imgMat, new Point (points [i].x, points [i].y), new Point (points [i - 1].x, points [i - 1].y), color, thickness);
                for (int i = 23; i <= 26; ++i)
                    Imgproc.line (imgMat, new Point (points [i].x, points [i].y), new Point (points [i - 1].x, points [i - 1].y), color, thickness);
                for (int i = 31; i <= 35; ++i)
                    Imgproc.line (imgMat, new Point (points [i].x, points [i].y), new Point (points [i - 1].x, points [i - 1].y), color, thickness);
                Imgproc.line (imgMat, new Point (points [30].x, points [30].y), new Point (points [35].x, points [35].y), color, thickness);
                
                for (int i = 37; i <= 41; ++i)
                    Imgproc.line (imgMat, new Point (points [i].x, points [i].y), new Point (points [i - 1].x, points [i - 1].y), color, thickness);
                Imgproc.line (imgMat, new Point (points [36].x, points [36].y), new Point (points [41].x, points [41].y), color, thickness);
                
                for (int i = 43; i <= 47; ++i)
                    Imgproc.line (imgMat, new Point (points [i].x, points [i].y), new Point (points [i - 1].x, points [i - 1].y), color, thickness);
                Imgproc.line (imgMat, new Point (points [42].x, points [42].y), new Point (points [47].x, points [47].y), color, thickness);
                
                for (int i = 49; i <= 59; ++i)
                    Imgproc.line (imgMat, new Point (points [i].x, points [i].y), new Point (points [i - 1].x, points [i - 1].y), color, thickness);
                Imgproc.line (imgMat, new Point (points [48].x, points [48].y), new Point (points [59].x, points [59].y), color, thickness);
                
                for (int i = 61; i <= 67; ++i)
                    Imgproc.line (imgMat, new Point (points [i].x, points [i].y), new Point (points [i - 1].x, points [i - 1].y), color, thickness);
                Imgproc.line (imgMat, new Point (points [60].x, points [60].y), new Point (points [67].x, points [67].y), color, thickness);
            } else {
                for (int i = 0; i < points.Count; i++) {
                    Imgproc.circle (imgMat, new Point (points [i].x, points [i].y), 2, color, -1);
                }
            }

            // Draw the index number of facelandmark points.
            if (drawIndexNumbers) {
                for (int i = 0; i < points.Count; ++i)
                    Imgproc.putText (imgMat, i.ToString (), new Point (points [i].x, points [i].y), Core.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar (255, 255, 255, 255), 1, Imgproc.LINE_AA, false);
            }
        }

        /// <summary>
        /// Draws a face landmark.
        /// This method supports 68 landmark points.
        /// </summary>
        /// <param name="imgMat">Image mat.</param>
        /// <param name="points">Points.</param>
        /// <param name="color">Color.</param>
        /// <param name="thickness">Thickness.</param>
        /// <param name="drawIndexNumbers">Determines if draw index numbers.</param>
        public static void DrawFaceLandmark (Mat imgMat, List<Point> points, Scalar color, int thickness, bool drawIndexNumbers = false)
        {
            if (points.Count == 5) {

                Imgproc.line (imgMat, points [0], points [1], color, thickness);
                Imgproc.line (imgMat, points [1], points [4], color, thickness);
                Imgproc.line (imgMat, points [4], points [3], color, thickness);
                Imgproc.line (imgMat, points [3], points [2], color, thickness);

            } else if (points.Count == 68) {

                for (int i = 1; i <= 16; ++i)
                    Imgproc.line (imgMat, points [i], points [i - 1], color, thickness);

                for (int i = 28; i <= 30; ++i)
                    Imgproc.line (imgMat, points [i], points [i - 1], color, thickness);

                for (int i = 18; i <= 21; ++i)
                    Imgproc.line (imgMat, points [i], points [i - 1], color, thickness);
                for (int i = 23; i <= 26; ++i)
                    Imgproc.line (imgMat, points [i], points [i - 1], color, thickness);
                for (int i = 31; i <= 35; ++i)
                    Imgproc.line (imgMat, points [i], points [i - 1], color, thickness);
                Imgproc.line (imgMat, points [30], points [35], color, thickness);

                for (int i = 37; i <= 41; ++i)
                    Imgproc.line (imgMat, points [i], points [i - 1], color, thickness);
                Imgproc.line (imgMat, points [36], points [41], color, thickness);

                for (int i = 43; i <= 47; ++i)
                    Imgproc.line (imgMat, points [i], points [i - 1], color, thickness);
                Imgproc.line (imgMat, points [42], points [47], color, thickness);

                for (int i = 49; i <= 59; ++i)
                    Imgproc.line (imgMat, points [i], points [i - 1], color, thickness);
                Imgproc.line (imgMat, points [48], points [59], color, thickness);

                for (int i = 61; i <= 67; ++i)
                    Imgproc.line (imgMat, points [i], points [i - 1], color, thickness);
                Imgproc.line (imgMat, points [60], points [67], color, thickness);
            } else {
                for (int i = 0; i < points.Count; i++) {
                    Imgproc.circle (imgMat, points [i], 2, color, -1);
                }
            }

            // Draw the index number of facelandmark points.
            if (drawIndexNumbers) {
                for (int i = 0; i < points.Count; ++i)
                    Imgproc.putText (imgMat, i.ToString (), points [i], Core.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar (255, 255, 255, 255), 1, Imgproc.LINE_AA, false);
            }
        }

        /// <summary>
        /// Draws a face landmark.
        /// This method supports 68 landmark points.
        /// </summary>
        /// <param name="imgMat">Image mat.</param>
        /// <param name="points">Detected object landmark data.[x_0, y_0, x_1, y_1, ...]</param>
        /// <param name="color">Color.</param>
        /// <param name="thickness">Thickness.</param>
        /// <param name="drawIndexNumbers">Determines if draw index numbers.</param>
        public static void DrawFaceLandmark (Mat imgMat, double[] points, Scalar color, int thickness, bool drawIndexNumbers = false)
        {
            List<Vector2> _points = new List<Vector2> ();
            for (int i = 0; i < points.Length; i = i + 2) {
                _points.Add (new Vector2 ((float)points [i], (float)points [i + 1]));
            }
            DrawFaceLandmark (imgMat, _points, color, thickness, drawIndexNumbers);
        }

        /// <summary>
        /// Convert vector2 list to point list.
        /// </summary>
        /// <param name="vecs">List of vector2.</param>
        /// <param name="pts">List of point.</param>
        /// <returns>List of point.</returns>
        public static List<Point> ConvertVector2ListToPointList (List<Vector2> vecs, List<Point> pts = null)
        {
            if (pts == null) {
                pts = new List<Point> ();
            }

            if (pts.Count != vecs.Count) {
                pts.Clear ();
                for (int i = 0; i < vecs.Count; i++) {
                    pts.Add (new Point ());
                }
            }

            for (int i = 0; i < vecs.Count; ++i) {
                pts [i].x = vecs [i].x;
                pts [i].y = vecs [i].y;
            }

            return pts;
        }

        /// <summary>
        /// Convert vector2 list to array.
        /// </summary>
        /// <param name="vecs">List of vector2.</param>
        /// <param name="arr">Array of double.</param>
        /// <returns>Array of double.</returns>
        public static double[] ConvertVector2ListToArray (List<Vector2> vecs, double[] arr = null)
        {
            if (arr == null || (arr != null && arr.Length != vecs.Count * 2)) {
                arr = new double[vecs.Count * 2];
            }

            for (int i = 0; i < vecs.Count; ++i) {
                arr [i * 2] = vecs [i].x;
                arr [i * 2 + 1] = vecs [i].y;
            }

            return arr;
        }

        /// <summary>
        /// Convert point list to vector2 list.
        /// </summary>
        /// <param name="pts">List of point.</param>
        /// <param name="vecs">List of vector2.</param>
        /// <returns>List of vector2.</returns>
        public static List<Vector2> ConvertPointListToVector2List (List<Point> pts, List<Vector2> vecs = null)
        {
            if (vecs == null) {
                vecs = new List<Vector2> ();
            }

            if (vecs.Count != pts.Count) {
                vecs.Clear ();
                for (int i = 0; i < pts.Count; i++) {
                    vecs.Add (new Vector2 ());
                }
            }

            for (int i = 0; i < pts.Count; ++i) {
                vecs.Add (new Vector2((float)pts[i].x, (float)pts[i].y));
            }

            return vecs;
        }

        /// <summary>
        /// Convert point list to array.
        /// </summary>
        /// <param name="pts">List of point.</param>
        /// <param name="arr">Array of double.</param>
        /// <returns>Array of double.</returns>
        public static double[] ConvertPointListToArray (List<Point> pts, double[] arr = null)
        {
            if (arr == null || (arr != null && arr.Length != pts.Count * 2)) {
                arr = new double[pts.Count * 2];
            }

            for (int i = 0; i < pts.Count; ++i) {
                arr [i * 2] = pts [i].x;
                arr [i * 2 + 1] = pts [i].y;
            }

            return arr;
        }

        /// <summary>
        /// Convert array to vector2 list.
        /// </summary>
        /// <param name="arr">Array of double.</param>
        /// <param name="vecs">List of vector2.</param>
        /// <returns>List of vector2.</returns>
        public static List<Vector2> ConvertArrayToVector2List (double[] arr, List<Vector2> vecs = null)
        {
            if (vecs == null) {
                vecs = new List<Vector2> ();
            }

            if (vecs.Count != arr.Length / 2) {
                vecs.Clear ();
                for (int i = 0; i < arr.Length / 2; i++) {
                    vecs.Add (new Vector2 ());
                }
            }

            for (int i = 0; i < vecs.Count; ++i) {
                vecs [i] = new Vector2((float)arr [i * 2], (float)arr [i * 2 + 1]);
            }

            return vecs;
        }

        /// <summary>
        /// Convert array to point list.
        /// </summary>
        /// <param name="arr">Array of double.</param>
        /// <param name="pts">List of point.</param>
        /// <returns>List of point.</returns>
        public static List<Point> ConvertArrayToPointList (double[] arr, List<Point> pts = null)
        {
            if (pts == null) {
                pts = new List<Point> ();
            }

            if (pts.Count != arr.Length / 2) {
                pts.Clear ();
                for (int i = 0; i < arr.Length / 2; i++) {
                    pts.Add (new Point ());
                }
            }

            for (int i = 0; i < pts.Count; ++i) {
                pts [i].x = arr [i * 2];
                pts [i].y = arr [i * 2 + 1];
            }

            return pts;
        }
    }
}
