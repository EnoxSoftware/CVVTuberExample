using DlibFaceLandmarkDetector;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using System;
using System.Collections.Generic;
using UnityEngine;

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
        public static void SetImage(FaceLandmarkDetector faceLandmarkDetector, Mat imgMat)
        {
            if (faceLandmarkDetector == null)
                throw new ArgumentNullException("faceLandmarkDetector");
            if (faceLandmarkDetector != null)
                faceLandmarkDetector.ThrowIfDisposed();

            if (imgMat == null)
                throw new ArgumentNullException("imgMat");
            if (imgMat != null)
                imgMat.ThrowIfDisposed();
            if (!imgMat.isContinuous())
                throw new ArgumentException("imgMat.isContinuous() must be true.");

            faceLandmarkDetector.SetImage((IntPtr)imgMat.dataAddr(), imgMat.width(), imgMat.height(), (int)imgMat.elemSize());
        }

        /// <summary>
        /// Draws a face rect.
        /// </summary>
        /// <param name="imgMat">Image mat.</param>
        /// <param name="rect">Rect.</param>
        /// <param name="color">Color.</param>
        /// <param name="thickness">Thickness.</param>
        public static void DrawFaceRect(Mat imgMat, UnityEngine.Rect rect, Scalar color, int thickness)
        {
            Imgproc.rectangle(imgMat, new Point(rect.xMin, rect.yMin), new Point(rect.xMax, rect.yMax), color, thickness);
        }

        /// <summary>
        /// Draws a face rect.
        /// </summary>
        /// <param name="imgMat">Image mat.</param>
        /// <param name="rect">Rect.</param>
        /// <param name="color">Color.</param>
        /// <param name="thickness">Thickness.</param>
        public static void DrawFaceRect(Mat imgMat, OpenCVForUnity.CoreModule.Rect rect, Scalar color, int thickness)
        {
            Imgproc.rectangle(imgMat, rect, color, thickness);
        }

        /// <summary>
        /// Draws a face rect.
        /// </summary>
        /// <param name="imgMat">Image mat.</param>
        /// <param name="rect">RectDetection.</param>
        /// <param name="color">Color.</param>
        /// <param name="thickness">Thickness.</param>
        public static void DrawFaceRect(Mat imgMat, DlibFaceLandmarkDetector.FaceLandmarkDetector.RectDetection rect, Scalar color, int thickness)
        {
            if (rect == null)
                throw new ArgumentNullException("rect");

            UnityEngine.Rect _rect = rect.rect;
            Imgproc.putText(imgMat, "detection_confidence : " + rect.detection_confidence, new Point(_rect.xMin, _rect.yMin - 20), Imgproc.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar(255, 255, 255, 255), 1, Imgproc.LINE_AA, false);
            Imgproc.putText(imgMat, "weight_index : " + rect.weight_index, new Point(_rect.xMin, _rect.yMin - 5), Imgproc.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar(255, 255, 255, 255), 1, Imgproc.LINE_AA, false);
            Imgproc.rectangle(imgMat, new Point(_rect.xMin, _rect.yMin), new Point(_rect.xMax, _rect.yMax), color, thickness);
        }

        /// <summary>
        /// Draws a face rect.
        /// </summary>
        /// <param name="imgMat">Image mat.</param>
        /// <param name="rect">Detected object's data. [left, top, width, height, detection_confidence, weight_index]</param>
        /// <param name="color">Color.</param>
        /// <param name="thickness">Thickness.</param>
        public static void DrawFaceRect(Mat imgMat, double[] rect, Scalar color, int thickness)
        {
            if (rect == null)
                throw new ArgumentNullException("rect");

            if (rect.Length > 4)
                Imgproc.putText(imgMat, "detection_confidence : " + rect[4], new Point(rect[0], rect[1] - 20), Imgproc.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar(255, 255, 255, 255), 1, Imgproc.LINE_AA, false);
            if (rect.Length > 5)
                Imgproc.putText(imgMat, "weight_index : " + rect[5], new Point(rect[0], rect[1] - 5), Imgproc.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar(255, 255, 255, 255), 1, Imgproc.LINE_AA, false);
            Imgproc.rectangle(imgMat, new Point(rect[0], rect[1]), new Point(rect[0] + rect[2], rect[1] + rect[3]), color, thickness);
        }

        /// <summary>
        /// Draws a face landmark.
        /// This method supports 68,17,6,5 landmark points.
        /// </summary>
        /// <param name="imgMat">Image mat.</param>
        /// <param name="points">Points.</param>
        /// <param name="color">Color.</param>
        /// <param name="thickness">Thickness.</param>
        /// <param name="drawIndexNumbers">Determines if draw index numbers.</param>
        public static void DrawFaceLandmark(Mat imgMat, IList<Vector2> points, Scalar color, int thickness, bool drawIndexNumbers = false)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            if (points.Count == 5)
            {

                Imgproc.line(imgMat, new Point(points[0].x, points[0].y), new Point(points[1].x, points[1].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[1].x, points[1].y), new Point(points[4].x, points[4].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[4].x, points[4].y), new Point(points[3].x, points[3].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[3].x, points[3].y), new Point(points[2].x, points[2].y), color, thickness);

            }
            else if (points.Count == 6)
            {

                Imgproc.line(imgMat, new Point(points[2].x, points[2].y), new Point(points[3].x, points[3].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[4].x, points[4].y), new Point(points[5].x, points[5].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[3].x, points[3].y), new Point(points[0].x, points[0].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[4].x, points[4].y), new Point(points[0].x, points[0].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[0].x, points[0].y), new Point(points[1].x, points[1].y), color, thickness);

            }
            else if (points.Count == 17)
            {

                Imgproc.line(imgMat, new Point(points[2].x, points[2].y), new Point(points[9].x, points[9].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[9].x, points[9].y), new Point(points[3].x, points[3].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[3].x, points[3].y), new Point(points[10].x, points[10].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[10].x, points[10].y), new Point(points[2].x, points[2].y), color, thickness);

                Imgproc.line(imgMat, new Point(points[4].x, points[4].y), new Point(points[11].x, points[11].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[11].x, points[11].y), new Point(points[5].x, points[5].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[5].x, points[5].y), new Point(points[12].x, points[12].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[12].x, points[12].y), new Point(points[4].x, points[4].y), color, thickness);

                Imgproc.line(imgMat, new Point(points[3].x, points[3].y), new Point(points[0].x, points[0].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[4].x, points[4].y), new Point(points[0].x, points[0].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[0].x, points[0].y), new Point(points[1].x, points[1].y), color, thickness);

                for (int i = 14; i <= 16; ++i)
                    Imgproc.line(imgMat, new Point(points[i].x, points[i].y), new Point(points[i - 1].x, points[i - 1].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[16].x, points[16].y), new Point(points[13].x, points[13].y), color, thickness);

                for (int i = 6; i <= 8; i++)
                    Imgproc.circle(imgMat, new Point(points[i].x, points[i].y), 2, color, -1);

            }
            else if (points.Count == 68)
            {

                for (int i = 1; i <= 16; ++i)
                    Imgproc.line(imgMat, new Point(points[i].x, points[i].y), new Point(points[i - 1].x, points[i - 1].y), color, thickness);

                for (int i = 28; i <= 30; ++i)
                    Imgproc.line(imgMat, new Point(points[i].x, points[i].y), new Point(points[i - 1].x, points[i - 1].y), color, thickness);

                for (int i = 18; i <= 21; ++i)
                    Imgproc.line(imgMat, new Point(points[i].x, points[i].y), new Point(points[i - 1].x, points[i - 1].y), color, thickness);
                for (int i = 23; i <= 26; ++i)
                    Imgproc.line(imgMat, new Point(points[i].x, points[i].y), new Point(points[i - 1].x, points[i - 1].y), color, thickness);
                for (int i = 31; i <= 35; ++i)
                    Imgproc.line(imgMat, new Point(points[i].x, points[i].y), new Point(points[i - 1].x, points[i - 1].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[30].x, points[30].y), new Point(points[35].x, points[35].y), color, thickness);

                for (int i = 37; i <= 41; ++i)
                    Imgproc.line(imgMat, new Point(points[i].x, points[i].y), new Point(points[i - 1].x, points[i - 1].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[36].x, points[36].y), new Point(points[41].x, points[41].y), color, thickness);

                for (int i = 43; i <= 47; ++i)
                    Imgproc.line(imgMat, new Point(points[i].x, points[i].y), new Point(points[i - 1].x, points[i - 1].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[42].x, points[42].y), new Point(points[47].x, points[47].y), color, thickness);

                for (int i = 49; i <= 59; ++i)
                    Imgproc.line(imgMat, new Point(points[i].x, points[i].y), new Point(points[i - 1].x, points[i - 1].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[48].x, points[48].y), new Point(points[59].x, points[59].y), color, thickness);

                for (int i = 61; i <= 67; ++i)
                    Imgproc.line(imgMat, new Point(points[i].x, points[i].y), new Point(points[i - 1].x, points[i - 1].y), color, thickness);
                Imgproc.line(imgMat, new Point(points[60].x, points[60].y), new Point(points[67].x, points[67].y), color, thickness);
            }
            else
            {
                for (int i = 0; i < points.Count; i++)
                {
                    Imgproc.circle(imgMat, new Point(points[i].x, points[i].y), 2, color, -1);
                }
            }

            // Draw the index number of facelandmark points.
            if (drawIndexNumbers)
            {
                for (int i = 0; i < points.Count; ++i)
                    Imgproc.putText(imgMat, i.ToString(), new Point(points[i].x, points[i].y), Imgproc.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar(255, 255, 255, 255), 1, Imgproc.LINE_AA, false);
            }
        }

        /// <summary>
        /// Draws a face landmark.
        /// This method supports 68,17,6,5 landmark points.
        /// </summary>
        /// <param name="imgMat">Image mat.</param>
        /// <param name="points">Points.</param>
        /// <param name="color">Color.</param>
        /// <param name="thickness">Thickness.</param>
        /// <param name="drawIndexNumbers">Determines if draw index numbers.</param>
        public static void DrawFaceLandmark(Mat imgMat, IList<Point> points, Scalar color, int thickness, bool drawIndexNumbers = false)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            if (points.Count == 5)
            {

                Imgproc.line(imgMat, points[0], points[1], color, thickness);
                Imgproc.line(imgMat, points[1], points[4], color, thickness);
                Imgproc.line(imgMat, points[4], points[3], color, thickness);
                Imgproc.line(imgMat, points[3], points[2], color, thickness);

            }
            else if (points.Count == 6)
            {

                Imgproc.line(imgMat, points[2], points[3], color, thickness);
                Imgproc.line(imgMat, points[4], points[5], color, thickness);
                Imgproc.line(imgMat, points[3], points[0], color, thickness);
                Imgproc.line(imgMat, points[4], points[0], color, thickness);
                Imgproc.line(imgMat, points[0], points[1], color, thickness);

            }
            else if (points.Count == 17)
            {

                Imgproc.line(imgMat, points[2], points[9], color, thickness);
                Imgproc.line(imgMat, points[9], points[3], color, thickness);
                Imgproc.line(imgMat, points[3], points[10], color, thickness);
                Imgproc.line(imgMat, points[10], points[2], color, thickness);

                Imgproc.line(imgMat, points[4], points[11], color, thickness);
                Imgproc.line(imgMat, points[11], points[5], color, thickness);
                Imgproc.line(imgMat, points[5], points[12], color, thickness);
                Imgproc.line(imgMat, points[12], points[4], color, thickness);

                Imgproc.line(imgMat, points[3], points[0], color, thickness);
                Imgproc.line(imgMat, points[4], points[0], color, thickness);
                Imgproc.line(imgMat, points[0], points[1], color, thickness);

                for (int i = 14; i <= 16; ++i)
                    Imgproc.line(imgMat, points[i], points[i - 1], color, thickness);
                Imgproc.line(imgMat, points[16], points[13], color, thickness);

                for (int i = 6; i <= 8; i++)
                    Imgproc.circle(imgMat, points[i], 2, color, -1);

            }
            else if (points.Count == 68)
            {

                for (int i = 1; i <= 16; ++i)
                    Imgproc.line(imgMat, points[i], points[i - 1], color, thickness);

                for (int i = 28; i <= 30; ++i)
                    Imgproc.line(imgMat, points[i], points[i - 1], color, thickness);

                for (int i = 18; i <= 21; ++i)
                    Imgproc.line(imgMat, points[i], points[i - 1], color, thickness);
                for (int i = 23; i <= 26; ++i)
                    Imgproc.line(imgMat, points[i], points[i - 1], color, thickness);
                for (int i = 31; i <= 35; ++i)
                    Imgproc.line(imgMat, points[i], points[i - 1], color, thickness);
                Imgproc.line(imgMat, points[30], points[35], color, thickness);

                for (int i = 37; i <= 41; ++i)
                    Imgproc.line(imgMat, points[i], points[i - 1], color, thickness);
                Imgproc.line(imgMat, points[36], points[41], color, thickness);

                for (int i = 43; i <= 47; ++i)
                    Imgproc.line(imgMat, points[i], points[i - 1], color, thickness);
                Imgproc.line(imgMat, points[42], points[47], color, thickness);

                for (int i = 49; i <= 59; ++i)
                    Imgproc.line(imgMat, points[i], points[i - 1], color, thickness);
                Imgproc.line(imgMat, points[48], points[59], color, thickness);

                for (int i = 61; i <= 67; ++i)
                    Imgproc.line(imgMat, points[i], points[i - 1], color, thickness);
                Imgproc.line(imgMat, points[60], points[67], color, thickness);
            }
            else
            {
                for (int i = 0; i < points.Count; i++)
                {
                    Imgproc.circle(imgMat, points[i], 2, color, -1);
                }
            }

            // Draw the index number of facelandmark points.
            if (drawIndexNumbers)
            {
                for (int i = 0; i < points.Count; ++i)
                    Imgproc.putText(imgMat, i.ToString(), points[i], Imgproc.FONT_HERSHEY_SIMPLEX, 0.5, new Scalar(255, 255, 255, 255), 1, Imgproc.LINE_AA, false);
            }
        }

        /// <summary>
        /// Draws a face landmark.
        /// This method supports 68,17,6,5 landmark points.
        /// </summary>
        /// <param name="imgMat">Image mat.</param>
        /// <param name="points">Detected object landmark data.[x_0, y_0, x_1, y_1, ...]</param>
        /// <param name="color">Color.</param>
        /// <param name="thickness">Thickness.</param>
        /// <param name="drawIndexNumbers">Determines if draw index numbers.</param>
        public static void DrawFaceLandmark(Mat imgMat, double[] points, Scalar color, int thickness, bool drawIndexNumbers = false)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            List<Vector2> _points = new List<Vector2>();
            for (int i = 0; i < points.Length; i = i + 2)
            {
                _points.Add(new Vector2((float)points[i], (float)points[i + 1]));
            }
            DrawFaceLandmark(imgMat, _points, color, thickness, drawIndexNumbers);
        }




        /// <summary>
        /// Convert Vector2 list to Vector2 array.
        /// </summary>
        /// <param name="src">List of Vector2.</param>
        /// <param name="dst">Array of Vector2.</param>
        /// <returns>Array of Vector2.</returns>
        public static Vector2[] ConvertVector2ListToVector2Array(IList<Vector2> src, Vector2[] dst = null)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (dst != null && src.Count != dst.Length)
                throw new ArgumentException("src.Count != dst.Length");

            if (dst == null)
            {
                dst = new Vector2[src.Count];
            }

            for (int i = 0; i < src.Count; ++i)
            {
                dst[i].x = src[i].x;
                dst[i].y = src[i].y;
            }

            return dst;
        }

        /// <summary>
        /// Convert Vector2 list to Point list.
        /// </summary>
        /// <param name="src">List of Vector2.</param>
        /// <param name="dst">List of Point.</param>
        /// <returns>List of Point.</returns>
        public static List<Point> ConvertVector2ListToPointList(IList<Vector2> src, List<Point> dst = null)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (dst == null)
            {
                dst = new List<Point>();
            }

            if (dst.Count != src.Count)
            {
                dst.Clear();
                for (int i = 0; i < src.Count; i++)
                {
                    dst.Add(new Point());
                }
            }

            for (int i = 0; i < src.Count; ++i)
            {
                dst[i].x = src[i].x;
                dst[i].y = src[i].y;
            }

            return dst;
        }

        /// <summary>
        /// Convert Vector2 list to Point array.
        /// </summary>
        /// <param name="src">List of Vector2.</param>
        /// <param name="dst">Array of Point.</param>
        /// <returns>Array of Point.</returns>
        public static Point[] ConvertVector2ListToPointArray(IList<Vector2> src, Point[] dst = null)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (dst != null && src.Count != dst.Length)
                throw new ArgumentException("src.Count != dst.Length");

            if (dst == null)
            {
                dst = new Point[src.Count];
            }

            for (int i = 0; i < src.Count; ++i)
            {
                dst[i] = new Point(src[i].x, src[i].y);
            }

            return dst;
        }

        /// <summary>
        /// Convert Vector2 list to array.
        /// </summary>
        /// <param name="src">List of Vector2.</param>
        /// <param name="dst">Array of double.</param>
        /// <returns>Array of double.</returns>
        public static double[] ConvertVector2ListToArray(IList<Vector2> src, double[] dst = null)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (dst != null && src.Count * 2 != dst.Length)
                throw new ArgumentException("src.Count * 2 != dst.Length");

            if (dst == null)
            {
                dst = new double[src.Count * 2];
            }

            for (int i = 0; i < src.Count; ++i)
            {
                dst[i * 2] = src[i].x;
                dst[i * 2 + 1] = src[i].y;
            }

            return dst;
        }




        /// <summary>
        /// Convert Point list to Point array.
        /// </summary>
        /// <param name="src">List of Point.</param>
        /// <param name="dst">Array of Point.</param>
        /// <returns>Array of Point.</returns>
        public static Point[] ConvertPointListToPointArray(IList<Point> src, Point[] dst = null)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (dst != null && src.Count != dst.Length)
                throw new ArgumentException("src.Count != dst.Length");

            if (dst == null)
            {
                dst = new Point[src.Count];
            }

            for (int i = 0; i < src.Count; ++i)
            {
                dst[i] = new Point(src[i].x, src[i].y);
            }

            return dst;
        }

        /// <summary>
        /// Convert Point list to Vector2 list.
        /// </summary>
        /// <param name="src">List of Point.</param>
        /// <param name="dst">List of Vector2.</param>
        /// <returns>List of Vector2.</returns>
        public static List<Vector2> ConvertPointListToVector2List(IList<Point> src, List<Vector2> dst = null)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (dst == null)
            {
                dst = new List<Vector2>();
            }

            dst.Clear();

            for (int i = 0; i < src.Count; ++i)
            {
                dst.Add(new Vector2((float)src[i].x, (float)src[i].y));
            }

            return dst;
        }

        /// <summary>
        /// Convert Point list to Vector2 array.
        /// </summary>
        /// <param name="src">List of Point.</param>
        /// <param name="dst">Array of Vector2.</param>
        /// <returns>Array of Vector2.</returns>
        public static Vector2[] ConvertPointListToVector2Array(IList<Point> src, Vector2[] dst = null)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (dst != null && src.Count != dst.Length)
                throw new ArgumentException("src.Count != dst.Length");

            if (dst == null)
            {
                dst = new Vector2[src.Count];
            }

            for (int i = 0; i < src.Count; ++i)
            {
                dst[i].x = (float)src[i].x;
                dst[i].y = (float)src[i].y;
            }

            return dst;
        }

        /// <summary>
        /// Convert Point list to array.
        /// </summary>
        /// <param name="src">List of Point.</param>
        /// <param name="dst">Array of double.</param>
        /// <returns>Array of double.</returns>
        public static double[] ConvertPointListToArray(IList<Point> src, double[] dst = null)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (dst != null && src.Count * 2 != dst.Length)
                throw new ArgumentException("src.Count * 2 != dst.Length");

            if (dst == null)
            {
                dst = new double[src.Count * 2];
            }

            for (int i = 0; i < src.Count; ++i)
            {
                dst[i * 2] = src[i].x;
                dst[i * 2 + 1] = src[i].y;
            }

            return dst;
        }




        /// <summary>
        /// Convert array to Vector2 list.
        /// </summary>
        /// <param name="src">Array of double.</param>
        /// <param name="dst">List of Vector2.</param>
        /// <returns>List of Vector2.</returns>
        public static List<Vector2> ConvertArrayToVector2List(double[] src, List<Vector2> dst = null)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (dst == null)
            {
                dst = new List<Vector2>();
            }

            dst.Clear();

            int len = src.Length / 2;
            for (int i = 0; i < len; ++i)
            {
                dst.Add(new Vector2((float)src[i * 2], (float)src[i * 2 + 1]));
            }

            return dst;
        }

        /// <summary>
        /// Convert array to Vector2 array.
        /// </summary>
        /// <param name="src">Array of double.</param>
        /// <param name="dst">Array of Vector2.</param>
        /// <returns>Array of Vector2.</returns>
        public static Vector2[] ConvertArrayToVector2Array(double[] src, Vector2[] dst = null)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (dst != null && src.Length / 2 != dst.Length)
                throw new ArgumentException("src.Length / 2 != dst.Length");

            if (dst == null)
            {
                dst = new Vector2[src.Length / 2];
            }

            for (int i = 0; i < dst.Length; ++i)
            {
                dst[i].x = (float)src[i * 2];
                dst[i].y = (float)src[i * 2 + 1];
            }

            return dst;
        }

        /// <summary>
        /// Convert array to Point list.
        /// </summary>
        /// <param name="src">Array of double.</param>
        /// <param name="dst">List of Point.</param>
        /// <returns>List of Point.</returns>
        public static List<Point> ConvertArrayToPointList(double[] src, List<Point> dst = null)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (dst == null)
            {
                dst = new List<Point>();
            }

            if (dst.Count != src.Length / 2)
            {
                dst.Clear();
                for (int i = 0; i < src.Length / 2; i++)
                {
                    dst.Add(new Point());
                }
            }

            for (int i = 0; i < dst.Count; ++i)
            {
                dst[i].x = src[i * 2];
                dst[i].y = src[i * 2 + 1];
            }

            return dst;
        }

        /// <summary>
        /// Convert array to Point array.
        /// </summary>
        /// <param name="src">Array of double.</param>
        /// <param name="dst">Array of Point.</param>
        /// <returns>Array of Point.</returns>
        public static Point[] ConvertArrayToPointArray(double[] src, Point[] dst = null)
        {
            if (src == null)
                throw new ArgumentNullException("src");

            if (dst != null && src.Length / 2 != dst.Length)
                throw new ArgumentException("src.Length / 2 != dst.Length");

            if (dst == null)
            {
                dst = new Point[src.Length / 2];
            }

            for (int i = 0; i < dst.Length; ++i)
            {
                dst[i] = new Point(src[i * 2], src[i * 2 + 1]);
            }

            return dst;
        }
    }
}
