using System;
using System.IO;
using OpenCvSharp;

class Program {
    static void replace(string imagePath, string beansPath, string outputPath) {
        Mat image = Cv2.ImRead(imagePath);
        Mat beans = Cv2.ImRead(beansPath, ImreadModes.Unchanged);

        if (beans.Empty()) {
            return;
        }

        bool hasAlpha = beans.Channels() == 4;

        string cascadeFile = @"C:\Users\Fabio\source\repos\BeansVirus\BeansVirus\haarcascade_frontalface_default.xml";
        var faceCascade = new CascadeClassifier(cascadeFile);

        Mat gray = new Mat();
        Cv2.CvtColor(image, gray, ColorConversionCodes.BGR2GRAY);

        var faces = faceCascade.DetectMultiScale(gray, 1.1, 5, HaarDetectionTypes.ScaleImage, new OpenCvSharp.Size(30, 30));
        foreach (var face in faces) {

            Mat resizedBeans = new Mat();
            Cv2.Resize(beans, resizedBeans, new OpenCvSharp.Size(face.Width, face.Height));

            for (int i = 0; i < face.Height; i++) {
                for (int j = 0; j < face.Width; j++) {
                    if (hasAlpha) {

                        Vec4b beanPixel = resizedBeans.At<Vec4b>(i, j);

                        if (beanPixel[3] > 0) {
                            image.At<Vec3b>(face.Y + i, face.X + j) = new Vec3b(beanPixel[0], beanPixel[1], beanPixel[2]);
                        }

                    } else {
                        image.At<Vec3b>(face.Y + i, face.X + j) = resizedBeans.At<Vec3b>(i, j);
                    }
                }
            }
        }

        Cv2.ImWrite(outputPath, image);
    }

    static void processImages(string directory, string beansPath) {
        var files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
        int count = 0;

        foreach (var file in files) {
            if (file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                file.EndsWith(".png", StringComparison.OrdinalIgnoreCase)) {

                string outputPath = Path.Combine(Path.GetDirectoryName(file), $"BEANED_{count}_" + Path.GetFileName(file));
                replace(file, beansPath, outputPath);
                count++;
            }
        }
    }

    static void Main(string[] args) {
        string userName = Environment.UserName;
        string directoryPath = $"C:\\Users\\{userName}\\OneDrive\\Documents\\SavedPictures";
        string beansPath = @"C:\Users\Fabio\source\repos\BeansVirus\BeansVirus\beans.png";

        processImages(directoryPath, beansPath);
    }
}
