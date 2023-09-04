using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
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
using static System.Net.WebRequestMethods;

namespace LogicielNettoyagePC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string version = "1.0.0";
        public DirectoryInfo winTemp;
        public DirectoryInfo appTemp;

        public MainWindow()
        {
            InitializeComponent();
            // Récuperer le chemin des dossiers temporaires
            winTemp = new DirectoryInfo(@"C:\Windows\Temp");
            appTemp = new DirectoryInfo(System.IO.Path.GetTempPath());
            CheckActu();
            GetDate();
        }

        /// <summary>
        /// Gérer le bandeau d'actualité
        /// </summary>
        public void CheckActu()
        {
            string url = "http://localhost/monSite/actu.txt";
            using (WebClient client = new WebClient())
            {
                // Fonction qui appel le serveur à l'url, recup le txt et le mettre ds la variable
                string actu = client.DownloadString(url);
                if(actu != String.Empty)
                {
                    actuTXT.Content = actu;
                    actuTXT.Visibility = Visibility.Visible;
                    bandeauActu.Visibility = Visibility.Visible;
                }
                
            }
        }

        // Verif version du logiciel
        public void CheckVersion()
        {
            string url = "http://localhost/monSite/version.txt";
            using (WebClient client = new WebClient())
            {
                // Fonction qui appel le serveur à l'url, recup le txt et le mettre ds la variable
                string v = client.DownloadString(url);
                // Si logiciel pas à jour:
                if(version != v)
                {
                    MessageBox.Show("Une mise à jour est dispo !", "Mise à jour", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Votre logiciel est à jour !", "Mise à jour", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
        }

        /// <summary>
        /// Calcul de la taile d'un dossier
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public long DirSize(DirectoryInfo dir)
        {
            return dir.GetFiles().Sum(fi => fi.Length) + dir.GetDirectories().Sum(di => DirSize(di));
        }

        // Vider un dossier
        public void ClearTempData(DirectoryInfo di)
        {
            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                    Console.WriteLine(file.FullName);
                }
                catch (Exception ex)
                {
                    continue;
                }

            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                try
                {
                    // dir.delete pour delete le dossier
                    dir.Delete(true);
                    Console.WriteLine(dir.FullName);
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }

        private void Button_MAJ_Click(object sender, RoutedEventArgs e)
        {
            CheckVersion();
        }

        private void Button_Histo_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("TODO: créer page histo ", "Mon historique", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void Button_Web_Click(object sender, RoutedEventArgs e)
        {
            // try catch pour éviter que ça plante 
            try
            {
                Process.Start(new ProcessStartInfo("https://google.fr")
                {
                    UseShellExecute = true
                });
            } catch (Exception ex)
            {
                Console.Write("Erreur : " + ex.Message);
            }

        }

        private void Button_Analyser_Click(object sender, RoutedEventArgs e)
        {
            AnalyseFolders();
        }

        public void AnalyseFolders()
        {
            Console.WriteLine("Début de l'analyse...");
            long totalSize = 0;

            try
            {

                totalSize += DirSize(winTemp) / 1000000; // On divise par 1M pour avoir la taille en mega
                totalSize += DirSize(appTemp) / 1000000;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Analyse impossible : " + ex.Message);
            }


            espace.Content = totalSize + " Mb";
            titre.Content = "Analyse effectué";
            date.Content = DateTime.Today;
            SaveDate();
        }

        private void Button_Nettoyer_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Nettoyage en cours...");
            btnClean.Content = "NETTOYAGE EN COURS";

            // Nettoyage presse papier
            Clipboard.Clear();

            // Nettoyer dossier temp
            try
            {
                ClearTempData(winTemp);
            } catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }

            try
            {
                ClearTempData(appTemp);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }

            btnClean.Content = "NETTOYAGE TERMINE !";
            titre.Content = "Nettoyage effectué !";
            espace.Content = "0 Mb";

        }

        public void SaveDate()
        {
            string date = DateTime.Today.ToString();
            System.IO.File.WriteAllText("date.txt", date);
        }

        public void GetDate()
        {
            try
            {
                string dateFichier = System.IO.File.ReadAllText("date.txt");
                if (dateFichier != String.Empty)
                {
                    date.Content = dateFichier;
                }
            } catch (Exception ex)
            {
                Console.WriteLine("Fichier introuvable" + ex.Message);
                FileStream fs = System.IO.File.Create("date.txt");
            }
            
        }
    }
}
