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
using System.IO;
using System.Globalization;
using System.Timers;
using System.Collections;
using System.Windows.Controls.Primitives;

namespace lecteurmp3_dotnet
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<Fichier> Files;
        List<string> mp3 = new List<string>();
        MediaPlayer mp;
        Random rnd = new Random();
        // int pos = 10;
        string currentMP3 = "";
        int mp3Index = 0;
        Fichier selectedItem=new Fichier();
        string userPath = Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
        private static System.Timers.Timer aTimer;

      

        public MainWindow()
        {
            InitializeComponent();
     
            //this.creerListe();
            initMP3Files();
            foreach(Fichier fi in this.Files)
            {
                System.Diagnostics.Debug.WriteLine(fi.Path);
            }

        }

        void initMP3Files(string filename = null)
        {

        
            RecursiveFileSearch filesearch = new RecursiveFileSearch();
            this.Files = filesearch.Main();

            foreach (Fichier mp3 in this.Files)
             {

                // string name = mp3.Name.Substring(mp3.Name.LastIndexOf(@"\")+1);
                 // MessageBox.Show(name);
                 this.mp3.Add(mp3.Name);
                 listeMP3.Items.Add(mp3.Name);
                         System.Diagnostics.Debug.WriteLine("--------------------------------------------------------------------");
        System.Diagnostics.Debug.WriteLine(mp3.Name);
            System.Diagnostics.Debug.WriteLine("--------------------------------------------------------------------");
                Label text = new Label();
                text.Height = 10;
                text.Content = mp3.Name;
              //  DataMP3.ItemsSource= text;
                 //MessageBox.Show(this.pos+"");

             }
           
            
        }

        private void listeMP3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                if (listeMP3.SelectedIndex > 0)
                {
                    int item = listeMP3.SelectedIndex;
                    System.Diagnostics.Debug.WriteLine(item);
                    this.currentMP3 = this.Files[item].Path+"\\"+ this.Files[item].Name;
                    this.setMP3Infos();
                    this.mp = new MediaPlayer();
                    this.mp.Open(new Uri(this.currentMP3));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("erreru");
            }

        }

        public void setTimer()
        {
            var infos = TagLib.File.Create(this.currentMP3);


            double duration = infos.Properties.Duration.TotalSeconds;
            MP3Time.Maximum = duration;
            aTimer = new System.Timers.Timer();
            aTimer.Interval = 1000;
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            var infos = TagLib.File.Create(this.currentMP3);


            double duration = infos.Properties.Duration.TotalSeconds;
            this.Dispatcher.Invoke(() =>
            {


                TimeSpan time = this.mp.Position;
                MP3Time.Value = time.TotalSeconds;
                MP3Timer.Content = time.ToString(@"mm\:ss");
                System.Diagnostics.Debug.WriteLine(time);

            });
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            this.mp = new MediaPlayer();

            this.mp.Open(new Uri(this.currentMP3));
            //  
            this.mp.Play();
            setTimer();
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            this.mp.Stop();

        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.mp3Index > 0 && this.currentMP3 != "")
            {
                this.mp3Index -= 1;
                this.currentMP3 = this.mp3[this.mp3Index];


                this.setMP3Infos();


                this.mp.Open(new Uri(this.currentMP3));
                this.mp.Play();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.mp3Index < this.Files.Count && this.currentMP3 != "")
            {
                this.mp3Index += 1;
                this.currentMP3 = this.mp3[this.mp3Index];


                this.setMP3Infos();

                this.mp.Open(new Uri(this.currentMP3));
                this.mp.Play();
            }
        }

        private void RandomButton_Click(object sender, RoutedEventArgs e)
        {
            this.mp3Index = rnd.Next(this.Files.Count);
            this.currentMP3 = this.mp3[this.mp3Index];

            this.setMP3Infos();


            this.mp.Open(new Uri(this.currentMP3));
            this.mp.Play();
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.currentMP3 != "")
            {
                this.mp.Stop();
                this.mp.Open(new Uri(this.currentMP3));
                this.mp.Play();
            }
        }

        public void setMP3Infos()
        {
            try
            {

                System.Diagnostics.Debug.WriteLine(this.Files[5].Path);


                var file = Path.GetFileNameWithoutExtension(this.currentMP3);

            var infos = TagLib.File.Create(this.currentMP3);


            TimeSpan duration = infos.Properties.Duration;

            MP3Title.Content = file + " " + duration.ToString(@"mm\:ss");
            MPEData.Content = infos.Tag.Album;


            MPEData.Content = infos.ToString();
   }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine(this.currentMP3);
                MessageBox.Show("fichier invalide");
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var test = SearchInput.Text;
            this.mp3.Clear();
            listeMP3.Items.Clear();
            initMP3Files(test);

        }

        private void AlbumGroup_Click(object sender, RoutedEventArgs e)
        {
            //  TagLib.File infos = TagLib.File.Create(this.currentMP3);
            listeMP3.Items.Clear();
            List<Dossier> dataAlbums = new List<Dossier>();
            List songs;
            TagLib.File infos;
            this.Files.ForEach(mp3 =>
           {
               infos = TagLib.File.Create(mp3.Path+"\\"+mp3.Name);

               List<Dossier> name = (List<Dossier>)dataAlbums.Where(x => x.Name == infos.Tag.Album).ToList();

               if (name.ToList().Count()>0)
               {
                   songs = new List();
                   Fichier file = new Fichier();
                   file.Name = mp3.Name;
                   name[0].dossier.Add(file);
               }
               else
               {
                   Dossier dossier = new Dossier();
                   dossier.Name = infos.Tag.Album;
                   dataAlbums.Add(dossier);
                   Fichier file = new Fichier();
                   file.Name = mp3.Name;
                   dossier.dossier.Add(file);

               }
           });

            this.createGroupList(dataAlbums);
            //listeMP3.ItemsSource = dataAlbums;
  

        }
        public void createGroupList(List<Dossier> dataAlbums)
        {
          foreach (var item in dataAlbums)
            {
                Expander exp = new Expander();
                exp.Header = item.Name;
                exp.IsExpanded = false;
             
                List<Fichier> listsongs = item.dossier;
                ListView lv = new ListView();
                lv.Name = "AlbumList";
              //  lv.Style = "{StaticResource Header1}";
             //   lv.Style = "{StaticResource Header1}";
              //  lv.Foreground = Color.FromRgb(255,192,122);
                lv.SelectionChanged += new SelectionChangedEventHandler(AlbumList_SelectedItem);
                lv.SelectedItem = "{Binding selectedItem}";
                exp.Content = lv;
               
               // lv.ItemsSource = listsongs;

                foreach(Fichier mp3 in listsongs)
                {

                    string name = mp3.Name.Substring(mp3.Name.LastIndexOf(@"\") + 1);
                    // MessageBox.Show(name);
                    lv.Items.Add(name);
                }
                listeMP3.Items.Add(exp);

            }
        }

        private void AlbumList_SelectedItem(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            ListView myview = sender as ListView;
            string file = (string)myview.SelectedItems[0];
      
           List<Fichier> tmp = this.Files.Where((m) => m.Name==file ).ToList();
            this.currentMP3 =tmp[0].Path+"\\"+ tmp[0].Name;
            this.setMP3Infos();
        
               
            System.Diagnostics.Debug.WriteLine(this.currentMP3);
                 }

        private void ArtistsGroup_Click(object sender, RoutedEventArgs e)
        {
            //  TagLib.File infos = TagLib.File.Create(this.currentMP3);
            listeMP3.Items.Clear();
            List<Dossier> dataAlbums = new List<Dossier>();
            //List songs;
            TagLib.File infos;
            this.Files.ForEach(mp3 =>
            {
                infos = TagLib.File.Create(mp3.Path+"\\"+mp3.Name);
               // System.Diagnostics.Debug.WriteLine(string.Join(",", infos.Tag));

              
                  //  System.Diagnostics.Debug.WriteLine(infos.Tag.Artists);
           
                /*  List<Dossier> name = (List<Dossier>)dataAlbums.Where(x => x.Name == infos.Tag.AlbumArtists).ToList();

                  if (name.ToList().Count() > 0)
                  {
                      songs = new List();
                      Fichier file = new Fichier();
                      file.Name = mp3;
                      name[0].dossier.Add(file);
                  }
                  else
                  {
                      Dossier dossier = new Dossier();
                      dossier.Name = infos.Tag.Album;
                      dataAlbums.Add(dossier);
                      Fichier file = new Fichier();
                      file.Name = mp3;
                      dossier.dossier.Add(file);

                  }*/
            });

            this.createGroupList(dataAlbums);
        }
        /*  public void creerListe()g.A.
{
System.Console.WriteLine("searching");

foreach (string currentFile in this.Files)
{
string fileName = Path.GetFileName(currentFile);
this.mp3.Add(fileName);
System.Diagnostics.Debug.WriteLine(fileName);
}
}

public void button1_Click(object sender, System.EventArgs e)
{
Label clickedButton = (Label)sender;
int index = this.mp3.IndexOf(clickedButton.Text);
this.mp3Index = index;
this.afficherPlayer(index);
// MessageBox.Show(index+"");

}

public void afficherPlayer(int index)
{
var currentSong = this.Files[index];
var fi1 = new FileInfo(currentSong);

FileAttributes attributes = File.GetAttributes(currentSong);
WMPLib.WindowsMediaPlayer wplayer = new WMPLib.WindowsMediaPlayer();

wplayer.URL = "My MP3 file.mp3";
wplayer.controls.play(); ;
// Read and Write:



}*/
    }

}