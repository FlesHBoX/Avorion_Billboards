using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Xml.Linq;

namespace Avorion_Billboards
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string imgPath;
        XDocument blueprint = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
        XElement ship = new XElement("ship",
                    new XAttribute("accumulatehealth","true"),
                    new XAttribute("convex","false"));
        int parent = -1;
        int index = 0;
        int rowParent;
        double Lx;
        double Ly;
        double Lz = -0.05;
        double Ux;
        double Uy;
        double Uz = 0.05;
        bool newRow = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Browse_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)),
                Filter = "Image Files(*.BMP;*.JPG;*.GIF)|*.BMP;*.JPG;*.GIF|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            if (openFileDialog.ShowDialog() == true)
            {
                imgPath = openFileDialog.FileName;                
                input_Image.Text = imgPath;
            }
        }

        private void ConvertImgToBlocks(string imgPath)
        {
            Bitmap img = new Bitmap(imgPath);
            Ly = -0.05;
            Uy = 0.05;
            for (int y = 0; y < img.Height; y++)
            {
                Lx = -0.05;
                Ux = 0.05;
                //rowParent = index;
                for (int x = 0; x < img.Width; x++)
                {
                    if(newRow == true)
                    {
                        parent = rowParent;
                        rowParent = index +1;
                        newRow = false;
                    }
                    System.Drawing.Color pixel = img.GetPixel(y, x);

                    XElement item = new XElement("item",
                        new XAttribute("parent",parent),
                        new XAttribute("index",index),
                        new XElement("block",
                        new XAttribute("lx", Math.Round(Lx,digits:3)),
                        new XAttribute("ly", Math.Round(Ly, digits:3)),
                        new XAttribute("lz", Lz),
                        new XAttribute("ux", Math.Round(Ux, digits:3)),
                        new XAttribute("uy", Math.Round(Uy, digits:3)),
                        new XAttribute("uz", Uz),
                        new XAttribute("index", "190"),
                        new XAttribute("material", "0"),
                        new XAttribute("look", "1"),
                        new XAttribute("up", "3"),
                        new XAttribute("color", $"ff{ColorToHexString(pixel)}")
                        ));
                    ship.Add(item);
                    Lx += .1;
                    Ux += .1;
                    parent++;
                    index++;
                }
                Ly -= .1;
                Uy -= .1;
                newRow = true;
            }
            blueprint.Add(ship);
        }

        public static string ColorToHexString(System.Drawing.Color oColor)
        {
            return string.Format("{0}{1}{2}", oColor.R.ToString("x2"),
            oColor.G.ToString("x2"), oColor.B.ToString("x2"));
        }

        private void Create_Ship_Click(object sender, RoutedEventArgs e)
        {
            ConvertImgToBlocks(imgPath);
            SaveFileDialog saveFile = new SaveFileDialog
            {
                InitialDirectory = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"Avorion\\ships\\"),
                RestoreDirectory = true,
                Title = "Create Ship",
                DefaultExt = "xml",
                Filter = "xml files (*.xml)|*.xml",
                FilterIndex = 1,
                FileName = "billboard.xml"
            };
            saveFile.ShowDialog();
            blueprint.Save(saveFile.FileName);
        }
    }
}
