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
using System.IO;
using System.Net;

namespace Spotify_Ad_Killer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
        public string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public bool IsPatchInstalled()
        {
            using (var client = new WebClient())
            using (var readerLocal = new StreamReader(systemPath + @"\drivers\etc\hosts"))
            {
                var localFile = readerLocal.ReadToEnd();
                var stream = client.OpenRead("http://kazesenoue.moe/uploads/hosts.txt");
                var remoteFile = new StreamReader(stream).ReadToEnd();

                if (localFile.Contains(remoteFile) && !File.Exists(appdata + @"\Spotify\Apps\ad.spa"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            if (!IsPatchInstalled())
            {
                button.Content = "Remove ads";
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (IsPatchInstalled())
            {
                using (var client = new WebClient())
                {
                    var stream = client.OpenRead("http://kazesenoue.moe/uploads/hosts.txt");
                    var remoteFile = new StreamReader(stream).ReadToEnd();
                    File.WriteAllText(systemPath + @"\drivers\etc\hosts", File.ReadAllText(systemPath + @"\drivers\etc\hosts").Replace(remoteFile, ""));
                    MessageBox.Show("Ads enabled!");
                    button.Content = "Remove ads";
                }
            }
            else
            {
                if (File.Exists(appdata + @"\Spotify\Apps\ad.spa"))
                    File.Delete(appdata + @"\Spotify\Apps\ad.spa");

                using (var client = new WebClient())
                using (var stream = new StreamWriter(systemPath + @"\drivers\etc\hosts", true))
                {
                    var stream2 = client.OpenRead("http://kazesenoue.moe/uploads/hosts.txt");
                    var reader = new StreamReader(stream2).ReadToEnd();

                    stream.Write(reader);
                    MessageBox.Show("Ads removed!");
                    button.Content = "Enable ads";
                }
            }
        }
    }
}
