# Document-Boundary-Detection

I've translated python script in c#, I've used OpenCVSharp.
I've decided to remove NumPy dependency, I've replaced it with native linq.

Original scripts are here:
- https://github.com/adityaguptai/Document-Boundary-Detection
- https://github.com/PyImageSearch/imutils

## 4-point Perspective Transform
A common task in computer vision and image processing is to perform a 4-point perspective transform of a ROI in an image and obtain a top-down, "birds eye view" of the ROI. The `perspective` module takes care of this for you. A real-world example of applying a 4-point perspective transform can be bound in this blog on on [building a kick-ass mobile document scanner](http://www.pyimagesearch.com/2014/09/01/build-kick-ass-mobile-document-scanner-just-5-minutes/).

#### Example
See the contents of `demos/perspective_transform.py`

#### Output:

<img src="https://raw.githubusercontent.com/PyImageSearch/imutils/master/docs/images/perspective_transform.png" alt="Matplotlib example" style="max-width: 500px;">

## Document Boundary &amp; Canny Edge Detection using OpenCV

* Used Simple OpenCV Library for Boundary and Edge Detection
* The Algorithm is quite fast and does not require any training and works quite well with rectangle documents as it was meant for document boundry detection 
* Did not use Deep Learning Techniques as it was most computationally expensive and required good Dataset of images with Boundry label and would had been slow too
* Sometimes simpler models are better than deep learning models

### How to Use

Make Sure you have following libraries installed

* opencv
* imutils

Use `pip install -r requirements.txt` to get all the dependencies

<b>To Run</b>

Use `python scan.py --image images/1.jpg ` to train the model

<b>It opens two windows one with edge detection and one with boundary detection</b>

## Samples
![alt img](https://raw.githubusercontent.com/adityaguptai/Document-Boundary-Detection/master/Samples/1.png)<br>
![alt img](https://raw.githubusercontent.com/adityaguptai/Document-Boundary-Detection/master/Samples/2.png)<br>
![alt img](https://raw.githubusercontent.com/adityaguptai/Document-Boundary-Detection/master/Samples/3.png)<br>
![alt img](https://raw.githubusercontent.com/adityaguptai/Document-Boundary-Detection/master/Samples/4.png)<br>
![alt img](https://raw.githubusercontent.com/adityaguptai/Document-Boundary-Detection/master/Samples/5.png)<br>
![alt img](https://raw.githubusercontent.com/adityaguptai/Document-Boundary-Detection/master/Samples/6.png)<br>

### Credits & Inspired By
(1) https://www.pyimagesearch.com/2014/09/01/build-kick-ass-mobile-document-scanner-just-5-minutes/<br>
