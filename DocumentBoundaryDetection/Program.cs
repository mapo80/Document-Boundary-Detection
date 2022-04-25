using OpenCvSharp;

var file = File.ReadAllBytes("images/1.jpg");

using var src = Mat.FromStream(new MemoryStream(file), ImreadModes.AnyColor);

//# Compute the ratio of the old height to the new height, clone it, 
//# and resize it easier for compute and viewing
var h = 500;
var ratio = (double)h / (double)src.Height;//(double)src.Height / (double)src.Width;
var newSrc = src.Resize(new Size(src.Width * ratio, h)); //.Resize(new Size(src.Width / ratio, h));

var size = 5;

//### convert the image to grayscale, blur it, and find edges in the image
var gray = new Mat();
Cv2.CvtColor(newSrc, gray, ColorConversionCodes.RGB2GRAY);

//# Gaussian Blurring to remove high frequency noise helping in
//# Contour Detection 
var gaussian = new Mat();
Cv2.GaussianBlur(gray, gaussian, new Size(size, size), 0, 0);

//# Canny Edge Detection
var edged = new Mat();
Cv2.Canny(gaussian, edged, 75, 200);


//# finding the contours in the edged image, keeping only the
//# largest ones, and initialize the screen contour
Cv2.FindContours(edged, out var contours, out var hierarchy, RetrievalModes.List,
    ContourApproximationModes.ApproxSimple);

//## What are Contours ?
//## Contours can be explained simply as a curve joining all the continuous
//## points (along the boundary), having same color or intensity. 
//## The contours are a useful tool for shape analysis and object detection 
//## and recognition.


//# Taking only the top 5 contours by Area
contours = contours.OrderByDescending(c => Cv2.ContourArea(c)).Take(5).ToArray();

//### Heuristic & Assumption

//# A document scanner simply scans in a piece of paper.
//# A piece of paper is assumed to be a rectangle.
//# And a rectangle has four edges.
//# Therefore use a heuristic like : we’ll assume that the largest
//# contour in the image with exactly four points is our piece of paper to 
//# be scanned.

//# looping over the contours
Point[]? screenCnt = null;

foreach (var contour in contours)
{
    var peri = Cv2.ArcLength(contour, true);
    var approx = Cv2.ApproxPolyDP(contour, 0.01 * peri, true);

    if (approx.Length == 4)
    {
        screenCnt = approx;
        break;
    }
}

var image = newSrc.Clone();
var warped = new Mat();
if (screenCnt != null)
{
    Cv2.DrawContours(image, new[] { screenCnt }, -1, Scalar.Red, 1);
    FourPointTransform(src.Clone(), warped, screenCnt, 1 / ratio);
}

using (new Window("edged", edged))
using (new Window("DrawContours", image))
using (new Window("warped", warped))
{
    while (true)
    {
        int c = Cv2.WaitKey(20);
        if ((char)c == 27 | (char)c == 32)
        {
            Cv2.DestroyAllWindows();
            break;
        }
    }
}

IList<Point2f> OrderPoints(Point[] pts, double ratio)
{
    //# initialize a list of coordinates that will be ordered
    //# such that the first entry in the list is the top-left,
    //# the second entry is the top-right, the third is the
    //# bottom-right, and the fourth is the bottom-left
    var rect = new List<Point2f>();

    var sum = pts.Select(p => (p.X + p.Y) * ratio).ToArray();

    //# now, compute the difference between the points, the
    //# top-right point will have the smallest difference,
    var diff = pts.Select(p => (p.Y - p.X) * ratio).ToArray();

    //# the top-left point will have the smallest sum, whereas
    var point = pts[Array.IndexOf(sum, sum.Min())];
    rect.Add(new Point(point.X * ratio, point.Y * ratio));

    point = pts[Array.IndexOf(diff, diff.Min())];
    rect.Add(new Point(point.X * ratio, point.Y * ratio));

    //# the bottom-right point will have the largest sum
    point = pts[Array.IndexOf(sum, sum.Max())];
    rect.Add(new Point(point.X * ratio, point.Y * ratio));

    //# whereas the bottom-left will have the largest difference
    point = pts[Array.IndexOf(diff, diff.Max())];
    rect.Add(new Point(point.X * ratio, point.Y * ratio));

    return rect;
}

void FourPointTransform(Mat inputImage, Mat outputImage, Point[] pts, double ratio)
{
    var rect = OrderPoints(pts, ratio);
    var tl = rect[0];
    var tr = rect[1];
    var br = rect[2];
    var bl = rect[3];
    
    //	# compute the width of the new image, which will be the
    //	# maximum distance between bottom-right and bottom-left
    //	# x-coordiates or the top-right and top-left x-coordinates
    var widthA = Math.Sqrt(Math.Pow(br.X - bl.X, 2) + Math.Pow(br.Y - bl.Y, 2));
    var widthB = Math.Sqrt(Math.Pow(tr.X - tl.X, 2) + Math.Pow(tr.Y - tl.Y, 2));
    var maxWidth = Math.Max((int)widthA, (int)widthB);

    //	# compute the height of the new image, which will be the
    //	# maximum distance between the top-right and bottom-right
    //	# y-coordinates or the top-left and bottom-left y-coordinates
    var heightA = Math.Sqrt(Math.Pow(tr.X - br.Y, 2) + Math.Pow(tr.Y - br.Y, 2));
    var heightB = Math.Sqrt(Math.Pow(tl.X - bl.X, 2) + Math.Pow(tl.Y - bl.Y, 2));
    var maxHeight = Math.Max((int)heightA, (int)heightB);


    //	# now that we have the dimensions of the new image, construct
    //	# the set of destination points to obtain a "birds eye view",
    //	# (i.e. top-down view) of the image, again specifying points
    //	# in the top-left, top-right, bottom-right, and bottom-left
    //	# order
    var dest = new List<Point2f>();
    dest.Add(new Point2f(0, 0));
    dest.Add(new Point2f(maxWidth - 1, 0));
    dest.Add(new Point2f(maxWidth - 1, maxHeight - 1));
    dest.Add(new Point2f(0, maxHeight - 1));

    var trasformationMatrix = Cv2.GetPerspectiveTransform(rect, dest);

    Cv2.WarpPerspective(inputImage, outputImage, trasformationMatrix, new Size(maxWidth, maxHeight));
}