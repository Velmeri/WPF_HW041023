using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_HW041023
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			LoadAndSplitImage();
		}

		private void LoadAndSplitImage()
		{
			var originalImageSource = new BitmapImage(new Uri("pack://application:,,,/abstract.png", UriKind.Absolute));

			var scaledImageSource = new TransformedBitmap(originalImageSource, new ScaleTransform(450.0 / originalImageSource.PixelWidth, 450.0 / originalImageSource.PixelHeight));

			var pieces = SplitImage(scaledImageSource, 3, 3);

			for (int row = 0; row < 3; row++) {
				for (int col = 0; col < 3; col++) {
					var piece = pieces[row * 3 + col];
					var image = new Image { Source = piece, Width = 150, Height = 150 };
					image.MouseLeftButtonDown += Image_MouseLeftButtonDown;

					Canvas.SetLeft(image, col * 150);
					Canvas.SetTop(image, row * 150);

					PuzzleCanvas.Children.Add(image);
				}
			}
		}

		private List<ImageSource> SplitImage(BitmapSource sourceImage, int rows, int cols)
		{
			List<ImageSource> pieces = new List<ImageSource>();

			int pieceWidth = sourceImage.PixelWidth / cols;
			int pieceHeight = sourceImage.PixelHeight / rows;

			for (int x = 0; x < cols; x++) {
				for (int y = 0; y < rows; y++) {
					var piece = new CroppedBitmap(sourceImage, new Int32Rect(x * pieceWidth, y * pieceHeight, pieceWidth, pieceHeight));
					pieces.Add(piece);
				}
			}

			return pieces;
		}

		private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var image = (Image)sender;
			DataObject data = new DataObject(typeof(ImageSource), image.Source);
			DragDrop.DoDragDrop(image, data, DragDropEffects.Move);
		}

		private void Canvas_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(ImageSource))) {
				var sourceImageSource = (ImageSource)e.Data.GetData(typeof(ImageSource));
				var target = e.Source as Image;

				if (target != null && sourceImageSource != null) {
					var targetImageSource = target.Source;

					target.Source = sourceImageSource;

					UpdateDraggedImageSource(sourceImageSource, targetImageSource);
				}
			}
		}

		private void UpdateDraggedImageSource(ImageSource draggedSource, ImageSource newSource)
		{
			foreach (var child in PuzzleCanvas.Children) {
				var image = child as Image;
				if (image != null && image.Source == draggedSource) {
					image.Source = newSource;
					break;
				}
			}
		}
	}
}
