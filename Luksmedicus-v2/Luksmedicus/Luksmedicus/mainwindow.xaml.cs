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
using System.Data.SQLite;
//using Finisar.SQLite;
using System.Configuration;

namespace Luksmedicus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SQLiteConnection sqlite_conn;
        SQLiteCommand sqlite_cmd;

        

        public MainWindow()
        {
            InitializeComponent();
            InitializeCustomeDatePicker();
            FillLboxFirmi();
            FillCboxFirmi();

        }

        private void InitializeCustomeDatePicker()
        {

            for (int i = 0; i < 31; i++)
            {
                cboxDay.Items.Add(String.Format("{0:00}",(i+1)));
            }
            for (int i = 0; i < 12; i++)
            {
                cboxMonth.Items.Add(String.Format("{0:00}", (i + 1)));
            }
            for (int i = 1900; i < DateTime.Today.Year; i++)
            {
                cboxYear.Items.Add(String.Format("{0:0000}", i));
            }
        }

        protected void KreirajDir(object sender, EventArgs e)
        {
            TextBox TextBox1 = (TextBox)vnesinaziv;
            String str = TextBox1.Text;
            if (str.Equals("")) { MessageBox.Show("Називот на фирмата не смее да биде празен!", "Грешка"); return; }
            System.IO.Directory.CreateDirectory("Правни лица/" + str);
            String path = "Правни лица/" + str + "/Инфо.txt";

            File.Create(path).Dispose();

            MessageBox.Show("Успешно е креиран директориум за фирма со назив '" + str + "'.", "Внесување фирма");

            String[] linii = new String[] { vnesinaziv.Text, vnesiadresa.Text, vnesiedb.Text, vnesiemb.Text, vnesizabeleska.Text };
            File.WriteAllLines(path, linii);



            sqlite_conn = new SQLiteConnection("Data Source=database.db;Version=3;New=false;Compress=True;");
            sqlite_conn.Open();

            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO firmi (naziv, adresa, edb, emb, zabeleshka) VALUES (" + "'" + vnesinaziv.Text.ToString() + "', '"
            + vnesiadresa.Text.ToString() + "', '" + vnesiedb.Text.ToString() + "', '" + vnesiemb.Text.ToString() + "', '" + vnesizabeleska.Text.ToString() + "');";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_conn.Close();
            //vnesuvanje vo baza

            vnesinaziv.Clear();
            vnesiadresa.Clear();
            vnesiedb.Clear();
            vnesiemb.Clear();
            vnesizabeleska.Clear();

            FillLboxFirmi();
            FillCboxFirmi();


            //sqlite test end

        }

        protected void PonistiVnesuvanjeFirma(object sender, EventArgs e)
        {
            vnesinaziv.Clear();
            vnesiadresa.Clear();
            vnesiedb.Clear();
            vnesiemb.Clear();
            vnesizabeleska.Clear();
        }

        private void lboxFirmi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            var item = listBox.SelectedItem as ListBoxItem;
            if (item != null)
            {
                //TODO FILL VRABOTENI
                Console.WriteLine(item.Content.ToString());
            }

        }

        private void cboxFirmi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            var item = comboBox.SelectedItem as ComboBoxItem;
            if (item != null && item.IsEnabled)
            {
                FillEmployees(item.Content.ToString());
                Console.WriteLine(item.Content.ToString());
            }
        }

        private void lboxVraboteni_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            var item = listBox.SelectedItem as ListBoxItem;
            if (item != null)
            {
                int result;
                if (Int32.TryParse(item.Tag.ToString(), out result))
                {
                    //TODO FILL lbox PREGLEDI
                    gbNovPregled.IsEnabled = true;
                    FilllboxPregledi(result);
                    FillgboxVraboten(result);
                }
                else
                {
                    //TODO PROBLEM BATE BATE
                    Console.WriteLine("Error while parsing vraboten_id");
                }


            }
        }

        private void FilllboxPregledi(int result)
        {
            lboxPregledi.Items.Clear();

            using (SQLiteConnection con = new SQLiteConnection(ConfigurationManager.ConnectionStrings["Database"].ToString()))
            {
                con.Open();

                string stm = "SELECT * FROM pregled WHERE id_vraboten='" + result + "';";

                using (SQLiteCommand cmd = new SQLiteCommand(stm, con))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            ListBoxItem item = new ListBoxItem();
                            string tip = "1";
                            switch (rdr["tip"].ToString())
                            {
                                case "1":
                                    tip = "Систематски";
                                    break;
                                case "2":
                                    tip = "Периодичен дополнителен";
                                    break;
                                case "3":
                                    tip = "Периодичен проширен";
                                    break;
                                case "4":
                                    tip = "Периодичен специфичен";
                                    break;
                                case "5":
                                    tip = "Насочен";
                                    break;
                            }
                            //TODO среди формат и договор како да изгледат
                            item.Content = String.Format("{0,-50}{1,-10}", tip, rdr["datum"].ToString().Split(' ')[0]);
                            item.Background = (rdr["plateno"].ToString() == "True" ? Brushes.LightGreen : Brushes.Tomato);
                            lboxPregledi.Items.Add(item);
                        }
                    }
                }
                con.Close();


            }

            if (lboxPregledi.Items.Count == 0)
            {
                ListBoxItem item = new ListBoxItem();
                //TODO смисли подобра фраза
                item.Content = "Нема прегледи";
                item.IsEnabled = false;
                lboxPregledi.Items.Add(item);
            }
        }

        //Enables-Disables fields in GroupBoxVraboten to toggle value
        private void toggleGBoxVraboten(bool toggle)
        {
            tbImeVraboten.IsReadOnly = toggle;
            tbMestoRagjanje.IsReadOnly = toggle;
            tbProfesija.IsReadOnly = toggle;
            tbRabotnoMesto.IsReadOnly = toggle;
            cboxDay.IsEnabled = toggle;
            cboxMonth.IsEnabled = toggle;
            cboxYear.IsEnabled = toggle;
        }

        private void FillgboxVraboten(int id)
        {
            gbVraboten.IsEnabled = true;
            tbImeVraboten.Text = "";
            tbMestoRagjanje.Text = "";
            tbProfesija.Text = "";
            tbRabotnoMesto.Text = "";
            toggleGBoxVraboten(true);
            cboxDay.SelectedIndex = 0;
            cboxMonth.SelectedIndex = 0;
            cboxYear.SelectedIndex = 0;
            btnVnesiVraboten.IsEnabled = false;

            using (SQLiteConnection con = new SQLiteConnection(ConfigurationManager.ConnectionStrings["Database"].ToString()))
            {
                con.Open();

                string stm = "SELECT * FROM vraboten WHERE id='" + id + "';";

                using (SQLiteCommand cmd = new SQLiteCommand(stm, con))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            tbImeVraboten.Text = rdr["ime_prezime"].ToString();
                            tbMestoRagjanje.Text = rdr["mesto_rag"].ToString();
                            tbProfesija.Text = rdr["profesija"].ToString();
                            tbRabotnoMesto.Text = rdr["rab_mesto"].ToString();

                            // TODO fix
                            // dpDatumRagjanje.SelectedDate = Convert.ToDateTime(rdr["datum_rag"].ToString() + " 00:00:00");
                        }
                    }
                }

                con.Close();



            }


        }

        private void FillCboxFirmi()
        {
            cboxFirmi.Items.Clear();

            ComboBoxItem temporary = new ComboBoxItem();
            //TODO смисли подобра фраза
            temporary.Content = "Избери фирма (Задолжително)";
            temporary.IsEnabled = false;
            cboxFirmi.Items.Add(temporary);
            cboxFirmi.SelectedIndex = 0;

            using (SQLiteConnection con = new SQLiteConnection(ConfigurationManager.ConnectionStrings["Database"].ToString()))
            {
                con.Open();

                string stm = "SELECT naziv FROM firmi";

                using (SQLiteCommand cmd = new SQLiteCommand(stm, con))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            ComboBoxItem item = new ComboBoxItem();
                            item.Content = rdr["naziv"].ToString();
                            cboxFirmi.Items.Add(item);
                        }
                    }
                }
                con.Close();
            }
        }

        private void FillLboxFirmi()
        {
            lboxFirmi.Items.Clear();

            using (SQLiteConnection con = new SQLiteConnection(ConfigurationManager.ConnectionStrings["Database"].ToString()))
            {
                con.Open();

                string stm = "SELECT naziv FROM firmi";

                using (SQLiteCommand cmd = new SQLiteCommand(stm, con))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            ListBoxItem item = new ListBoxItem();
                            item.Content = rdr["naziv"].ToString();
                            lboxFirmi.Items.Add(item);
                        }
                    }
                }
                con.Close();


            }

            if (lboxFirmi.Items.Count == 0)
            {
                ListBoxItem item = new ListBoxItem();
                //TODO смисли подобра фраза
                item.Content = "Нема фирми";
                item.IsEnabled = false;
                lboxFirmi.Items.Add(item);
            }
        }

        private void FillEmployees(String naziv_firma)
        {
            lboxVraboteni.Items.Clear();

            using (SQLiteConnection con = new SQLiteConnection(ConfigurationManager.ConnectionStrings["Database"].ToString()))
            {
                con.Open();

                string stm = "SELECT ime_prezime,rab_mesto,id FROM vraboten WHERE naziv_firma='" + naziv_firma + "';";

                using (SQLiteCommand cmd = new SQLiteCommand(stm, con))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            ListBoxItem item = new ListBoxItem();
                            item.Content = rdr["ime_prezime"].ToString();
                            item.Tag = rdr["id"].ToString();
                            lboxVraboteni.Items.Add(item);
                        }
                    }
                }

                con.Close();

                dpDatumPregled.Text = DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss");

            }

            if (lboxVraboteni.Items.Count == 0)
            {
                ListBoxItem item = new ListBoxItem();
                //TODO смисли подобра фраза
                item.Content = "Нема вработени";
                item.IsEnabled = false;
                lboxVraboteni.Items.Add(item);
            }

        }

        protected void addEmployee(object sender, EventArgs e)
        {

            if (tbImeVraboten.Equals("")) { MessageBox.Show("Полето за име на вработениот не смее да биде празно", "Грешка!"); return; }

            string date = cboxDay.SelectedItem.ToString() + "-" + cboxMonth.SelectedItem.ToString() +"-"+cboxYear.SelectedItem.ToString();

            string stm = "INSERT INTO vraboten(ime_prezime, rab_mesto, profesija, naziv_firma, mesto_rag, datum_rag) VALUES('" +
               tbImeVraboten.Text.ToString() + "', '" +
               tbRabotnoMesto.Text.ToString() + "', '" +
               tbProfesija.Text.ToString() + "', '" +
               cboxFirmi.SelectedValue.ToString() + "', '" +
               tbMestoRagjanje.Text.ToString() + "', '" +
               date + "');";


            sqlite_conn = new SQLiteConnection("Data Source=database.db;Version=3;New=false;Compress=True;");
            sqlite_conn.Open();

            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = stm;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_conn.Close();

            MessageBox.Show(tbImeVraboten.Text + " e внесен во базата.", "Успешно е внесен нов вработен во фирмата " + cboxFirmi.SelectedValue.ToString());

            gbVraboten.IsEnabled = false;

            FillEmployees(cboxFirmi.SelectedValue.ToString());
        }

        protected void addNewEmployee(object sender, EventArgs e)
        {
            if (cboxFirmi.SelectedIndex <= 0)
            {
                MessageBox.Show("Мора да одберете фирма во која ќе го внесете вработениот. ", "Грешка!");
                return;
            }

            gbVraboten.IsEnabled = true;
        }

        protected void addPregled(object sender, EventArgs e)
        {

            if (rbSistematski.IsChecked == false && rbDopolnitelen.IsChecked == false &&
                rbSpecifichen.IsChecked == false && rbNasochen.IsChecked == false)
            {
                string pom1 = rbSistematski.IsChecked.ToString();
                MessageBox.Show("Мора да изберете тип на преглед за пациентот " + pom1, "Грешка!");
                return;
            }

            string vrabIme = "";
            int vrabID = 0;
            string strSQL1 = "SELECT id FROM vraboten WHERE ime_prezime = " + '"' + tbImeVraboten.Text.ToString() + '"' + ";";
            sqlite_conn = new SQLiteConnection("Data Source=database.db;Version=3;New=false;Compress=True;");
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();

            sqlite_cmd.CommandText = strSQL1;
            SQLiteDataReader rdr = sqlite_cmd.ExecuteReader();
            rdr.Read();
            vrabID = Int32.Parse(rdr["id"].ToString());
            rdr.Close();


            int tip = 0;
            int cena = 0;
            if (rbSistematski.IsChecked == true) { tip = 1; cena = 800; }
            else if (rbDopolnitelen.IsChecked == true) { tip = 2; cena = 1000; }
            else if (rbProshiren.IsChecked == true) { tip = 3; cena = 1200; }
            else if (rbSpecifichen.IsChecked == true) { tip = 4; cena = 1500; }
            else if (rbNasochen.IsChecked == true) { tip = 5; cena = 1000; }


            string str = "INSERT INTO pregled (datum, tip, cena, id_vraboten) VALUES ('" +
                dpDatumPregled.Text.ToString() + "', '" +
                tip.ToString() + "', '" +
                cena.ToString() + "', '" +
                vrabID.ToString() + "');";


            sqlite_cmd.CommandText = str;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_conn.Close();

            FilllboxPregledi(vrabID);


        }


    }
}