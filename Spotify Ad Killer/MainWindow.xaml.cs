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
using Newtonsoft.Json;

namespace Spotify_Ad_Killer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
        public string appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public string version = "v1.0";

        public void Log(string info)
        {
            using (var writer = new StreamWriter("log.txt", true))
            {
                var time = DateTime.Now.ToShortTimeString();

                writer.WriteLine("[" + time + "] " + info);
            }
        }

        public void CheckForUpdates()
        {
            Log("Checking for updates...");
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", "Spotify-Ad-Killer");
                var json = client.DownloadString("https://api.github.com/repos/KazeSenoue/Spotify-Ad-Killer/releases/latest");
                dynamic data = JsonConvert.DeserializeObject(json);

                if (data.tag_name.ToString() != version)
                {
                    Log("Update found.");
                    var dialogResult = MessageBox.Show("An update has been found. Do you wish to update?", "Update warning", MessageBoxButton.YesNo);
                    if (dialogResult == MessageBoxResult.Yes)
                    {
                        Log("Opening update link.");
                        System.Diagnostics.Process.Start(data.html_url.ToString());
                        Application.Current.Shutdown();
                    }
                    else
                    {
                        Log("Update declined.");
                    }
                }
            }
        }

        public bool IsPatchInstalled()
        {
            using (var client = new WebClient())
            using (var readerLocal = new StreamReader(systemPath + @"\drivers\etc\hosts"))
            {
                var localFile = readerLocal.ReadToEnd();
                var stream = client.OpenRead("http://kazesenoue.moe/uploads/hosts");
                var remoteFile = new StreamReader(stream).ReadToEnd();

                if (localFile.Contains(remoteFile) && !File.Exists(appdata + @"\Spotify\Apps\ad.spa"))
                {
                    Log("Patch has already been applied.");
                    return true;
                }
                else
                {
                    Log("No patch applied.");
                    return false;
                }
            }
        }

        public MainWindow()
        {
            CheckForUpdates();
            InitializeComponent();
            if (!IsPatchInstalled())
            {
                button.Content = "Remove ads";
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Made by: http://kazesenoue.moe \nGithub: http://github.com/KazeSenoue/Spotify-Ad-Killer", "About");
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IsPatchInstalled())
                {
                    using (var client = new WebClient())
                    {
                        var stream = client.OpenRead("http://kazesenoue.moe/uploads/hosts");
                        var remoteFile = new StreamReader(stream).ReadToEnd();
                        File.WriteAllText(systemPath + @"\drivers\etc\hosts", File.ReadAllText(systemPath + @"\drivers\etc\hosts").Replace(remoteFile, ""));
                        MessageBox.Show("Ads enabled!");
                        button.Content = "Remove ads";
                        Log("Ads enabled.");
                    }
                }
                else
                {
                    if (File.Exists(appdata + @"\Spotify\Apps\ad.spa"))
                        File.Delete(appdata + @"\Spotify\Apps\ad.spa");

                    using (var client = new WebClient())
                    using (var stream = new StreamWriter(systemPath + @"\drivers\etc\hosts", true))
                    {
                        var stream2 = client.OpenRead("http://kazesenoue.moe/uploads/hosts");
                        var reader = new StreamReader(stream2).ReadToEnd();

                        stream.Write(reader);
                        MessageBox.Show("Ads removed!");
                        button.Content = "Enable ads";
                        Log("Ads removed.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                Log("---------------------------------\n");
            }
        }
    }
}
