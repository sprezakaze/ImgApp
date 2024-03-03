using System;
using System.Drawing;

namespace ImageProcessingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //вывод меню на экран
            Console.WriteLine("Выберите операцию:");
            Console.WriteLine("1. Пиксельное сложение");
            Console.WriteLine("2. Пиксельное умножение");
            Console.WriteLine("3. Среднее арифметическое");
            Console.WriteLine("4. Минимум");
            Console.WriteLine("5. Максимум");
            Console.WriteLine("6. Наложение с маской");
            Console.WriteLine("7. Градационные преобразования");
            //реализация навигационного меню через конструкцию свитч кейс
            int choice;
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                switch (choice)
                {
                    case 1:
                        ProcessImages(PixelWiseSum); // выполнение попиксельного сложения
                        break;
                    case 2:
                        ProcessImages(PixelWiseProduct); // выполнение пиксельного умножения
                        break;
                    case 3:
                        ProcessImages(Average); // выполнение среднего арифметического
                        break;
                    case 4:
                        ProcessImages(Minimum); // выполнение поиска минимума
                        break;
                    case 5:
                        ProcessImages(Maximum); // выполнение поиска максимума
                        break;
                    case 6:
                        OverlayWithMask(); // наложение с маской
                        break;
                    case 7:
                        GradationTransformations();
                        break;
                    default:
                        Console.WriteLine("Неверный выбор."); //уведомление о неверном выборе
                        break;
                }
            }
            else
            {
                Console.WriteLine("Неверный ввод.");
            }
        }

        // Обработка изображений с использованием переданной операции
        static void ProcessImages(Func<Color, Color, Color> operation)
        {
            Console.WriteLine("Введите полный путь для первого изображения:");
            string imagePath1 = Console.ReadLine();

            Console.WriteLine("Введите полный путь для второго изображения:");
            string imagePath2 = Console.ReadLine();

            Console.WriteLine("Обработка изображений...");

            // Используем блок using для автоматического закрытия ресурсов
            using (var img1 = new Bitmap(imagePath1))
            using (var img2 = new Bitmap(imagePath2))
            {
                var w = img1.Width; //установка длины и ширины изображения
                var h = img1.Height;

                using (var imgOut = new Bitmap(w, h))
                {
                    // Перебор всех пикселей изображений
                    for (int i = 0; i < h; ++i)
                    {
                        for (int j = 0; j < w; ++j)
                        {
                            var pix1 = img1.GetPixel(j, i);
                            var pix2 = img2.GetPixel(j, i);

                            // Применение операции к каждому пикселю
                            var resultPix = operation(pix1, pix2);

                            imgOut.SetPixel(j, i, resultPix);
                        }
                    }

                    Console.WriteLine("Изображение успешно обработано.");

                    Console.WriteLine("Введите полный путь для сохранения результата:");
                    string outputPath = Console.ReadLine();

                    // Сохранение результата
                    imgOut.Save(outputPath);

                    Console.WriteLine("Результат сохранен по пути: " + outputPath);
                }
            }
        }

        // Наложение маски на основное изображение
        static void OverlayWithMask()
        {
            Console.WriteLine("Введите полный путь для основного изображения:");
            string mainImagePath = Console.ReadLine();

            Console.WriteLine("Введите форму маски (circle, square, rectangle):");
            string maskShape = Console.ReadLine();

            Console.WriteLine("Обработка изображений...");

            // Используем блок using для автоматического закрытия ресурсов
            using (var mainImage = new Bitmap(mainImagePath))
            {
                var w = mainImage.Width; //установка длины и ширины изображения
                var h = mainImage.Height;

                using (var imgOut = new Bitmap(w, h))
                {
                    // Перебор всех пикселей изображения
                    for (int i = 0; i < h; ++i)
                    {
                        for (int j = 0; j < w; ++j)
                        {
                            Color maskPixel;

                            // Выбор формы маски и определение цвета пикселя (реализация через свитч кейс)
                            switch (maskShape.ToLower())
                            {
                                case "circle":
                                    maskPixel = IsInsideCircle(j, i, w / 2, h / 2, Math.Min(w, h) / 4) ? Color.White : Color.Black;
                                    break;
                                case "square":
                                    maskPixel = IsInsideSquare(j, i, w / 2, h / 2, Math.Min(w, h) / 2) ? Color.White : Color.Black;
                                    break;
                                case "rectangle":
                                    maskPixel = IsInsideRectangle(j, i, w / 4, h / 4, w / 2, h / 2) ? Color.White : Color.Black;
                                    break;
                                default:
                                    Console.WriteLine("Неверная форма маски. Поддерживаемые формы: circle, square, rectangle");
                                    return;
                            }

                            var mainPixel = mainImage.GetPixel(j, i);

                            // Наложение маски
                            if (maskPixel.R == 255 && maskPixel.G == 255 && maskPixel.B == 255)
                            {
                                imgOut.SetPixel(j, i, mainPixel);
                            }
                            else
                            {
                                imgOut.SetPixel(j, i, maskPixel);
                            }
                        }
                    }
                    //сообщение об успешности обработки
                    Console.WriteLine("Изображение успешно обработано.");
                    //приглашение к воду ссылки на результат
                    Console.WriteLine("Введите полный путь для сохранения результата:");
                    string outputPath = Console.ReadLine();

                    // Сохранение результата
                    imgOut.Save(outputPath);
                    //сообщение об успешности сохранения
                    Console.WriteLine("Результат сохранен по пути: " + outputPath);
                }
            }
        }

        static void GradationTransformations()
        {
            int histogramheight = 512;
            Console.WriteLine("Введите полный путь для фото:");
            string imagePath = Console.ReadLine();
            Console.WriteLine("Обработка изображения...");

            using (var image = new Bitmap(imagePath))
            {
                Console.WriteLine("Введите полный путь для гистограммы исходного изображения:");
                string histogram1path = Console.ReadLine();
                CreateHistogramImage(image, histogramheight, histogram1path);
                var w = image.Width;
                var h = image.Height;

                using (var transformedImage = new Bitmap(w, h))
                {
                    for (int i = 0; i < h; ++i)
                    {
                        for (int j = 0; j < w; ++j)
                        {
                            Color originalPixel = image.GetPixel(j, i);
                            // Применяем градационное преобразование S = r^2
                            int r = originalPixel.R;
                            int g = originalPixel.G;
                            int b = originalPixel.B;
                            int newR = (r * r) / 255;
                            int newG = (g * g) / 255;
                            int newB = (b * b) / 255;

                            Color newPixel = Color.FromArgb(newR, newG, newB);
                            transformedImage.SetPixel(j, i, newPixel);
                        }
                    }

                    Console.WriteLine("Изображение успешно обработано.");
                    Console.WriteLine("Введите полный путь для сохранения результата:");
                    string outputPath = Console.ReadLine();

                    transformedImage.Save(outputPath);
                    Console.WriteLine("Результат сохранен по пути: " + outputPath);
                    Console.WriteLine("Введите полный путь для гистограммы измененного изображения:");
                    string histogram2path = Console.ReadLine();
                    CreateHistogramImage(transformedImage, histogramheight, histogram2path);
                }
            }
        }

        // проверка, находится ли точка внутри круга
        static bool IsInsideCircle(int x, int y, int centerX, int centerY, int radius)
        {
            return Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2) <= Math.Pow(radius, 2);
        }

        // проверка, находится ли точка внутри квадрата
        static bool IsInsideSquare(int x, int y, int centerX, int centerY, int sideLength)
        {
            int halfSide = sideLength / 2;
            return x >= centerX - halfSide && x <= centerX + halfSide && y >= centerY - halfSide && y <= centerY + halfSide;
        }

        // проверка, находится ли точка внутри прямоугольника
        static bool IsInsideRectangle(int x, int y, int left, int top, int width, int height)
        {
            return x >= left && x <= left + width && y >= top && y <= top + height;
        }

        // попиксельное сложение цветов
        static Color PixelWiseSum(Color pixel1, Color pixel2)
        {
            return Color.FromArgb(
                (int)Clamp(pixel1.R + pixel2.R, 0, 255), //сумма цветов
                (int)Clamp(pixel1.G + pixel2.G, 0, 255),
                (int)Clamp(pixel1.B + pixel2.B, 0, 255));
        }

        // попиксельное умножение цветов
        static Color PixelWiseProduct(Color pixel1, Color pixel2)
        {
            return Color.FromArgb(
                (int)Clamp((pixel1.R * pixel2.R) / 255, 0, 255), //произведение цветов
                (int)Clamp((pixel1.G * pixel2.G) / 255, 0, 255),
                (int)Clamp((pixel1.B * pixel2.B) / 255, 0, 255));
        }

        // среднее арифметическое цветов
        static Color Average(Color pixel1, Color pixel2)
        {
            return Color.FromArgb(
                (int)Clamp((pixel1.R + pixel2.R) / 2, 0, 255), //сумма цветов делится надвое
                (int)Clamp((pixel1.G + pixel2.G) / 2, 0, 255),
                (int)Clamp((pixel1.B + pixel2.B) / 2, 0, 255));
        }

        // поиск минимального цвета
        static Color Minimum(Color pixel1, Color pixel2)
        {
            return Color.FromArgb(
                Math.Min(pixel1.R, pixel2.R), //использование функции Math.Min() для поиска минимального из двух элементов
                Math.Min(pixel1.G, pixel2.G),
                Math.Min(pixel1.B, pixel2.B));
        }

        // поиск максимального цвета
        static Color Maximum(Color pixel1, Color pixel2)
        {
            return Color.FromArgb(
                Math.Max(pixel1.R, pixel2.R), //использование функции Math.Max() для поиска минимального из двух элементов
                Math.Max(pixel1.G, pixel2.G),
                Math.Max(pixel1.B, pixel2.B));
        }

        // Ограничение значения в заданных пределах
        static T Clamp<T>(T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if (val.CompareTo(max) > 0) return max;
            else return val;
        }
        static void CreateHistogramImage(Bitmap inputImage, int histogramHeight, string outputFilePath)
        {
            int[] histogram = new int[256];
            int maxHeight = 0;

            for (int y = 0; y < inputImage.Height; y++)
            {
                for (int x = 0; x < inputImage.Width; x++)
                {
                    Color pixel = inputImage.GetPixel(x, y);
                    int brightness = (pixel.R + pixel.G + pixel.B) / 3;
                    histogram[brightness]++;
                    if (histogram[brightness] > maxHeight)
                    {
                        maxHeight = histogram[brightness];
                    }
                }
            }

            Bitmap outputImage = new Bitmap(256, histogramHeight);

            double scaleFactor = (double)histogramHeight / maxHeight;

            for (int i = 0; i < 255; i++)
            {
                int lineStartY = histogramHeight - 1;
                int lineEndY = (int)(histogramHeight - 1 - histogram[i] * scaleFactor);

                for (int y = lineStartY; y >= lineEndY; y--)
                {
                    outputImage.SetPixel(i, y, Color.Black);
                }
            }

            outputImage.Save(outputFilePath); // Сохранение изображения

            outputImage.Dispose();
        }
    }
}