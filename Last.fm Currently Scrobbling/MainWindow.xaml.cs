using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Last.fm_Currently_Scrobbling
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DispatcherTimer timer = new DispatcherTimer();
        int i = 0;

        public MainWindow()
        {
            InitializeComponent();

            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0,0,5);
            Console.WriteLine("Application Loaded");
            Console.WriteLine("Starting Timer");
            timer.Start();

            Application.Current.MainWindow.Closing += Window_Closing;
        }
        
        JavaScriptSerializer serializer = new JavaScriptSerializer();
        WebClient webClient = new WebClient();

        
        private void Update()
        {
        var settings = serializer.Deserialize<dynamic>(File.ReadAllText("settings.json"));

        string APIKey = settings["APIKey"];
        string SharedSecret = ""; //Might not need this, we'll see later.
        string AlbumImageURI;
        string ArtistName;
        string AlbumName;
        string SongName;
        string User = settings["User"];
            string jsonWebString = webClient.DownloadString($"https://ws.audioscrobbler.com/2.0/?method=user.getrecenttracks&user={User}&api_key={APIKey}&format=json");
            Console.WriteLine("Update Init");
            var deserializeWeb = serializer.Deserialize<dynamic>(jsonWebString);

            if (!jsonWebString.Contains("nowplaying"))
            {
                AlbumImageURI = "https://image.freepik.com/free-icon/musical-note-symbol_318-32225.jpg";
                ArtistName = "None";
                AlbumName = "None";
                SongName = "None";
                UpdateFinal();
                return;
            }

            AlbumImageURI = deserializeWeb["recenttracks"]["track"][0]["image"][3]["#text"];
            ArtistName = deserializeWeb["recenttracks"]["track"][0]["artist"]["#text"];
            AlbumName = deserializeWeb["recenttracks"]["track"][0]["album"]["#text"];
            SongName = deserializeWeb["recenttracks"]["track"][0]["name"];

            UpdateFinal();

            void UpdateFinal()
            {
                AlbumImage.Source = new BitmapImage(new Uri(AlbumImageURI));
                ArtistLabel.Content = ArtistName;
                AlbumLabel.Content = AlbumName;
                SongLabel.Content = SongName;

                Console.WriteLine("Update End");
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Update();
            Console.WriteLine("Loop count: " + i++);
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            Console.WriteLine("Window Closing!");
            try
            {
                Console.WriteLine("Stopping Timer");
                timer.Stop();
                timer.IsEnabled = false;
                Console.WriteLine("Timer Stopped");
            }
            catch(Exception ex)
            {
                Console.WriteLine("ERROR: Could not stop timer");
                Console.WriteLine(ex);
            }
        }
    }
}
