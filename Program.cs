using System;
using System.Drawing;
//Павлов бы мной гордился
namespace ImageProcessingApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Выберите операцию:");
            Console.WriteLine("1. Пиксельное сложение");
            Console.WriteLine("2. Пиксельное умножение");
            Console.WriteLine("3. Среднее арифметическое");
            Console.WriteLine("4. Минимум");
            Console.WriteLine("5. Максимум");
            Console.WriteLine("6. Наложение с маской");

            int choice;
            if (int.TryParse(Console.ReadLine(), out choice))
            {
                switch (choice)
                {
                    case 1:
                        ProcessImages(PixelWiseSum); // Выполнение пиксельного сложения
                        break;
                    case 2:
                        ProcessImages(PixelWiseProduct); // Выполнение пиксельного умножения
                        break;
                    case 3:
                        ProcessImages(Average); // Выполнение среднего арифметического
                        break;
                    case 4:
                        ProcessImages(Minimum); // Выполнение поиска минимума
                        break;
                    case 5:
                        ProcessImages(Maximum); // Выполнение поиска максимума
                        break;
                    case 6:
                        OverlayWithMask(); // Наложение с маской
                        break;
                    default:
                        Console.WriteLine("Неверный выбор.");
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
                var w = img1.Width;
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
                var w = mainImage.Width;
                var h = mainImage.Height;

                using (var imgOut = new Bitmap(w, h))
                {
                    // Перебор всех пикселей изображения
                    for (int i = 0; i < h; ++i)
                    {
                        for (int j = 0; j < w; ++j)
                        {
                            Color maskPixel;

                            // Выбор формы маски и определение цвета пикселя
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

                    Console.WriteLine("Изображение успешно обработано.");

                    Console.WriteLine("Введите полный путь для сохранения результата:");
                    string outputPath = Console.ReadLine();

                    // Сохранение результата
                    imgOut.Save(outputPath);

                    Console.WriteLine("Результат сохранен по пути: " + outputPath);
                }
            }
        }

        // Проверка, находится ли точка внутри круга
        static bool IsInsideCircle(int x, int y, int centerX, int centerY, int radius)
        {
            return Math.Pow(x - centerX, 2) + Math.Pow(y - centerY, 2) <= Math.Pow(radius, 2);
        }

        // Проверка, находится ли точка внутри квадрата
        static bool IsInsideSquare(int x, int y, int centerX, int centerY, int sideLength)
        {
            int halfSide = sideLength / 2;
            return x >= centerX - halfSide && x <= centerX + halfSide && y >= centerY - halfSide && y <= centerY + halfSide;
        }

        // Проверка, находится ли точка внутри прямоугольника
        static bool IsInsideRectangle(int x, int y, int left, int top, int width, int height)
        {
            return x >= left && x <= left + width && y >= top && y <= top + height;
        }

        // Пиксельное сложение цветов
        static Color PixelWiseSum(Color pixel1, Color pixel2)
        {
            return Color.FromArgb(
                (int)Clamp(pixel1.R + pixel2.R, 0, 255),
                (int)Clamp(pixel1.G + pixel2.G, 0, 255),
                (int)Clamp(pixel1.B + pixel2.B, 0, 255));
        }

        // Пиксельное умножение цветов
        static Color PixelWiseProduct(Color pixel1, Color pixel2)
        {
            return Color.FromArgb(
                (int)Clamp((pixel1.R * pixel2.R) / 255, 0, 255),
                (int)Clamp((pixel1.G * pixel2.G) / 255, 0, 255),
                (int)Clamp((pixel1.B * pixel2.B) / 255, 0, 255));
        }

        // Среднее арифметическое цветов
        static Color Average(Color pixel1, Color pixel2)
        {
            return Color.FromArgb(
                (int)Clamp((pixel1.R + pixel2.R) / 2, 0, 255),
                (int)Clamp((pixel1.G + pixel2.G) / 2, 0, 255),
                (int)Clamp((pixel1.B + pixel2.B) / 2, 0, 255));
        }

        // Поиск минимального цвета
        static Color Minimum(Color pixel1, Color pixel2)
        {
            return Color.FromArgb(
                Math.Min(pixel1.R, pixel2.R),
                Math.Min(pixel1.G, pixel2.G),
                Math.Min(pixel1.B, pixel2.B));
        }

        // Поиск максимального цвета
        static Color Maximum(Color pixel1, Color pixel2)
        {
            return Color.FromArgb(
                Math.Max(pixel1.R, pixel2.R),
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
    }
}
