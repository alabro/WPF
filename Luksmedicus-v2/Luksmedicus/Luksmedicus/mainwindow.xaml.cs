using System;
using System.Collections.Generic;
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
using System.Configuration;
using System.Data;
using System.Linq;
using System.Data.Entity;

namespace Luksmedicus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        private DocumentCreator creator = new DocumentCreator();

        public MainWindow()
        {
            InitializeComponent();
            InitializeCustomeDatePicker();
            FillLboxFirmi();
            FillCboxFirmi();
            dpDatumPregled.Text = DateTime.Now.ToShortDateString();



            //EmptyTables();

        }

        private static void EmptyTables()
        {
            using (var db = new DatabaseContext())
            {
                var rows = from o in db.Employees
                           select o;
                foreach (var row in rows)
                {
                    db.Employees.Remove(row);
                }
                db.SaveChanges();

            }

            using (var db = new DatabaseContext())
            {
                var rows = from o in db.Reviews
                           select o;
                foreach (var row in rows)
                {
                    db.Reviews.Remove(row);
                }
                db.SaveChanges();

            }

            using (var db = new DatabaseContext())
            {
                var rows = from o in db.Businesss
                           select o;
                foreach (var row in rows)
                {
                    db.Businesss.Remove(row);
                }
                db.SaveChanges();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
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


            using (var db = new DatabaseContext())
            {
                var business = new Business { BusinessName = vnesinaziv.Text.ToString(), BusinessAddress = vnesiadresa.Text.ToString(), BusinessEdb = vnesiedb.Text.ToString(), BusinessEmb = vnesiemb.Text.ToString(), BusinessRemark = vnesizabeleska.Text.ToString() };
                db.Businesss.Add(business);
                db.SaveChanges();
            } 

            //vnesuvanje vo baza

            vnesinaziv.Clear();
            vnesiadresa.Clear();
            vnesiedb.Clear();
            vnesiemb.Clear();
            vnesizabeleska.Clear();

            FillLboxFirmi();
            FillCboxFirmi();

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
            lbPregledivoFirma.Items.Clear();
            ListBox listBox = sender as ListBox;
            var item = listBox.SelectedItem as ListBoxItem;
            if (item != null)
            {

                ClearInfo();

                var BusinessID = int.Parse(item.Tag.ToString());

                FillFirmaInfo(BusinessID);
                FillLboxPreglediFirma(BusinessID);
                GetSumPregledi();    
            }
        }

        private void FillFirmaInfo(int id)
        {
            using (var db = new DatabaseContext())
            {

                var query = from b in db.Businesss
                            where b.BusinessID == id
                            select b;

                Console.WriteLine("All firmi in the database:");
                foreach (var item in query)
                {
                    imefirmainfo.Text = "Име на фирма: " + item.BusinessName;
                    adresafirmainfo.Text = "Адреса: " + item.BusinessAddress;
                    edbfirmainfo.Text = "ЕДБ: " + item.BusinessEdb;
                }

                db.SaveChanges();
            }
        }

        private void FillLboxPreglediFirma(int BusinessID)
        {

            using (var db = new DatabaseContext())
            {
                //var query = from b in db.Employees
                //            orderby b.Employees
                //            select b;

                var query = from employee in db.Employees 
                                     join review in db.Reviews 
                                     on employee.EmployeeID equals review.EmployeeID
                                     where employee.BusinessID == BusinessID
                                     select new {employee,review};

                Console.WriteLine("All firmi in the database:");
                foreach (var item in query)
                {
                    ListBoxItem litem = new ListBoxItem();
                    StackPanel sp = new StackPanel();
                    sp.Orientation = Orientation.Horizontal;

                    Label lb = new Label();
                    lb.Content = item.review.ReviewDate.ToShortDateString();
                    lb.Width = 100;
                    sp.Children.Add(lb);

                    Label ld = new Label();
                    ld.Content = item.employee.EmployeeNameSurname;
                    ld.Width = 250;
                    lb.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    sp.Children.Add(ld);

                    var tip = "";

                    switch (item.review.ReviewType)
                    {
                        case "1":
                            tip = "Систематски";
                            break;
                        case "2":
                            tip = "Дополнителен";
                            break;
                        case "3":
                            tip = "Проширен";
                            break;
                        case "4":
                            tip = "Специфичен";
                            break;
                        case "5":
                            tip = "Насочен";
                            break;
                    }



                    Label td = new Label();
                    td.Content = tip;
                    td.Width = 100;
                    td.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    sp.Children.Add(td);

                    Label cd = new Label();
                    cd.Content = String.Format("{0:#.00} МКД",item.review.ReviewPrice);
                    cd.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    sp.Children.Add(cd);



                    litem.Content = sp;
                    litem.Background = (item.review.ReviewIsPayed ? Brushes.LightGreen : Brushes.Tomato);

                    litem.Tag = item.review.ReviewID;

                    lbPregledivoFirma.Items.Add(litem);
                }
            }

        }

        private void ClearInfo()
        {
            imefirmainfo.Text = "Име на фирма: ";
            adresafirmainfo.Text = "Адреса: ";
            edbfirmainfo.Text = "ЕДБ: ";
            dolziinfo.Text = "Должи: 0,00 MKD";
            platenoinfo.Text = "Платено: 0,00 MKD";
        }

        private void GetSumPregledi()
        {

            //SUM PLATENI
            double payed = 0;
            double notPayed = 0;
            using (var db = new DatabaseContext())
            {

                foreach (var listItem in lbPregledivoFirma.Items)
                {
                    var ReviewID = int.Parse((listItem as ListBoxItem).Tag.ToString());
                    var query = from b in db.Reviews
                                where b.ReviewID == ReviewID
                                select b;

                    foreach (var item in query)
                    {
                        if (item.ReviewIsPayed)
                        {
                            payed += item.ReviewPrice;
                        }
                        else
                        {
                            notPayed += item.ReviewPrice;
                        }
                    }
                    db.SaveChanges();

                    dolziinfo.Text = String.Format("Должи: {0:0.00} МКД",notPayed);

                    platenoinfo.Text = String.Format("Платено: {0:0.00} МКД", payed);

                }

            }
            


            //using (SQLiteConnection con = new SQLiteConnection(ConfigurationManager.ConnectionStrings["Database"].ToString()))
            //{
            //    con.Open();

            //    string command = "SELECT SUM(pregled.cena) AS cena, plateno FROM pregled,vraboten WHERE vraboten.naziv_firma = "
            //            + '"' + nazivFirma + '"' + " AND vraboten.id = pregled.id_vraboten GROUP BY plateno ORDER BY pregled.datum DESC;";


            //    using (SQLiteCommand cmd = new SQLiteCommand(command, con))
            //    {

            //        using (SQLiteDataReader reader = cmd.ExecuteReader())
            //        {
                       
            //            var plateno = "0";
            //            var dolzi = "0";

            //            while (reader.Read())
            //            {
            //                if (reader["plateno"].ToString().Equals("True"))
            //                {
            //                    plateno = reader["cena"].ToString();
            //                }
            //                else
            //                {
            //                    dolzi = reader["cena"].ToString();
            //                }

            //            }

            //            dolziinfo.Text = "Должи: " + dolzi + ",00 MKD";
                       
            //            platenoinfo.Text = "Платено: " + plateno + ",00 MKD";
                        

            //        }

            //    }


            //    con.Close();

            //}



        }

        private void cboxFirmi_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            var item = comboBox.SelectedItem as ComboBoxItem;
            if (item != null && item.IsEnabled)
            {
                FillEmployees(int.Parse(item.Tag.ToString()));
            }
        }

        private void lboxVraboteni_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            var item = listBox.SelectedItem as ListBoxItem;
            if (item != null)
            {
                int EmployeeID;
                if (Int32.TryParse(item.Tag.ToString(), out EmployeeID))
                {
                    Console.WriteLine(EmployeeID);
                    gbNovPregled.IsEnabled = true;
                    FillLboxPregledi(EmployeeID);
                    FillgboxVraboten(EmployeeID);
                }
                else
                {
                    //TODO PROBLEM BATE BATE
                    Console.WriteLine("Error while parsing vraboten_id");
                }


            }
        }

        private void FillLboxPregledi(int EmployeeID)
        {
            lboxPregledi.Items.Clear();
            Console.WriteLine(EmployeeID);
            using (var db = new DatabaseContext())
            {
                var query = from b in db.Reviews
                            where b.EmployeeID == EmployeeID
                            select b;

               

                foreach (var item in query)
                {
                    Console.WriteLine(item.EmployeeID);
                    string tip = "1";
                    switch (item.ReviewType)
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
                    ListBoxItem reviewItem = new ListBoxItem();
                    StackPanel sp = new StackPanel();
                    sp.Orientation = Orientation.Horizontal;
                    Label lb = new Label();
                    lb.Content = tip;
                    lb.Width = lboxPregledi.Width/4 * 3;
                    sp.Children.Add(lb);
                    Label ld = new Label();
                    ld.Content = item.ReviewDate.ToShortDateString();
                    lb.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    sp.Children.Add(ld);
                    reviewItem.Tag = item.ReviewID;
                    reviewItem.Content = sp;
                    reviewItem.Background = (item.ReviewIsPayed ? Brushes.LightGreen : Brushes.Tomato);
                    lboxPregledi.Items.Add(reviewItem);
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
        }

        //Enables-Disables fields in GroupBoxVraboten to toggle value
        private void toggleCustomDatePicker(bool toggle)
        {
            cboxDay.IsEnabled = toggle;
            cboxMonth.IsEnabled = toggle;
            cboxYear.IsEnabled = toggle;
        }

        private void ToggleReadOnlyGboxVraboten(bool toggle)
        {
            
            tbImeVraboten.IsReadOnly = toggle;
            tbMestoRagjanje.IsReadOnly = toggle;
            tbProfesija.IsReadOnly = toggle;
            tbRabotnoMesto.IsReadOnly = toggle;
        }

        private void FillgboxVraboten(int id)
        {
            ToggleReadOnlyGboxVraboten(true);
            ClearGboxVraboteni();
            toggleCustomDatePicker(false);
            btnVnesiVraboten.IsEnabled = false;
            gbVraboten.IsEnabled = true;
            gbPregledi.IsEnabled = true;

            using (var db = new DatabaseContext())
            {

                var query = from b in db.Employees
                            where b.EmployeeID == id
                            select b;

                foreach (var item in query)
                {
                    tbImeVraboten.Text = item.EmployeeNameSurname;
                    tbMestoRagjanje.Text = item.EmployeeAddress;
                    tbProfesija.Text = item.EmployeeProffesion;
                    tbRabotnoMesto.Text = item.EmployeeWorks;

                    //Console.WriteLine(date[0]+" "+date[1]+" "+date[2]);
                    string dtm = Convert.ToDateTime(item.EmployeeBirthDate).ToString("dd/MM/yyyy");
                    SetCustomDate(dtm);
                }

                db.SaveChanges();
            }

        }

        private void SetCustomDate(string dateRaw)
        {
            string[] date = dateRaw.Split(' ')[0].Split('.');
            cboxDay.SelectedItem = date[0];
            cboxMonth.SelectedItem = date[1];
            cboxYear.SelectedItem = date[2];
        }

        private void ClearGboxVraboteni()
        {
            tbImeVraboten.Text = "";
            tbMestoRagjanje.Text = "";
            tbProfesija.Text = "";
            tbRabotnoMesto.Text = "";
            cboxDay.SelectedIndex = 0;
            cboxMonth.SelectedIndex = 0;
            cboxYear.SelectedIndex = 0;
        }

        private void FillCboxFirmi()
        {
            cboxFirmi.Items.Clear();

            ComboBoxItem temporary = new ComboBoxItem();
            temporary.Content = "Избери фирма (Задолжително)";
            temporary.IsEnabled = false;
            cboxFirmi.Items.Add(temporary);
            cboxFirmi.SelectedIndex = 0;

            using (var db = new DatabaseContext())
            {
                var query = from b in db.Businesss
                            orderby b.BusinessName
                            select b;

                foreach (var item in query)
                {
                    if (item.BusinessID != null)
                    {
                        ComboBoxItem b = new ComboBoxItem();
                        b.Content = item.BusinessName;
                        b.Tag = item.BusinessID;
                        cboxFirmi.Items.Add(b);
                    }
                }

            }


            //using (SQLiteConnection con = new SQLiteConnection(ConfigurationManager.ConnectionStrings["Database"].ToString()))
            //{
            //    con.Open();

            //    string stm = "SELECT naziv FROM firmi";

            //    using (SQLiteCommand cmd = new SQLiteCommand(stm, con))
            //    {
            //        using (SQLiteDataReader rdr = cmd.ExecuteReader())
            //        {
            //            while (rdr.Read())
            //            {
            //                ComboBoxItem item = new ComboBoxItem();
            //                item.Content = rdr["naziv"].ToString();
            //                cboxFirmi.Items.Add(item);
            //            }
            //        }
            //    }
            //    con.Close();
            //}
        }

        private void FillLboxFirmi()
        {
            lboxFirmi.Items.Clear();

            using (var db = new DatabaseContext())
            {
                var query = from b in db.Businesss
                            orderby b.BusinessName
                            select b;

                Console.WriteLine("All firmi in the database:");
                foreach (var item in query)
                {
                    if (item.BusinessID != null)
                    {
                        ListBoxItem b = new ListBoxItem();
                        b.Content = item.BusinessName;
                        b.Tag = item.BusinessID;
                        lboxFirmi.Items.Add(b);
                    }
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

        }

        private void FillEmployees(int BussinessID)
        {
            lboxVraboteni.Items.Clear();

            using (var db = new DatabaseContext())
            {
                var query = from b in db.Employees
                            where b.BusinessID == BussinessID
                            select b;

                Console.WriteLine("All firmi in the database:");
                foreach (var item in query)
                {
                    ListBoxItem b = new ListBoxItem();
                    b.Content = item.EmployeeNameSurname;
                    b.Tag = item.EmployeeID;
                    lboxVraboteni.Items.Add(b);
                }

                if (lboxVraboteni.Items.Count == 0)
                {
                    ListBoxItem item = new ListBoxItem();
                    item.Content = "Нема вработени";
                    item.IsEnabled = false;
                    item.Tag = "-1";
                    lboxVraboteni.Items.Add(item);
                }

            }
        }

        protected void addEmployee(object sender, EventArgs e)
        {

            if (tbImeVraboten.Equals("")) { MessageBox.Show("Полето за име на вработениот не смее да биде празно", "Грешка!"); return; }
            //TODO check DATE VALUES and tb fields


            ComboBoxItem b = cboxFirmi.SelectedItem as ComboBoxItem;
            int BusinessID = int.Parse(b.Tag.ToString());

            using (var db = new DatabaseContext())
            {
                var employee = new Employee();
                employee.EmployeeNameSurname = tbImeVraboten.Text.ToString();
                employee.EmployeeWorks = tbRabotnoMesto.Text.ToString();
                employee.EmployeeProffesion = tbProfesija.Text.ToString();
                employee.EmployeeAddress = tbMestoRagjanje.Text.ToString();
                int year = Int32.Parse(cboxYear.SelectedItem.ToString());
                int month = Int32.Parse(cboxMonth.SelectedItem.ToString());
                int day = Int32.Parse(cboxDay.SelectedItem.ToString());
                DateTime dateTime = new DateTime(year,month,day);
                employee.EmployeeBirthDate = dateTime;

                employee.BusinessID = BusinessID;

                db.Employees.Add(employee);
                db.SaveChanges();
            }

            MessageBox.Show(tbImeVraboten.Text + " e внесен во базата.", "Успешно е внесен нов вработен во фирмата " + cboxFirmi.SelectedValue.ToString());

            lboxVraboteni.SelectedIndex = lboxVraboteni.Items.Count;

            FillEmployees(BusinessID);
            RefreshLists();
        }

        protected void addNewEmployee(object sender, EventArgs e)
        {
            if (cboxFirmi.SelectedIndex <= 0)
            {
                MessageBox.Show("Мора да одберете фирма во која ќе го внесете вработениот. ", "Грешка!");
                return;
            }
            lboxVraboteni.SelectedIndex = -1;
            ToggleReadOnlyGboxVraboten(false);
            toggleCustomDatePicker(true);
            ClearGboxVraboteni();
            gbVraboten.IsEnabled = true;
            lboxPregledi.Items.Clear();
            btnVnesiVraboten.IsEnabled = true;
            gbNovPregled.IsEnabled = false;
            gbPregledi.IsEnabled = false;
        }

        protected void addPregled(object sender, EventArgs e)
        {

            if (rbSistematski.IsChecked == false && rbDopolnitelen.IsChecked == false &&
                rbSpecifichen.IsChecked == false && rbNasochen.IsChecked == false && rbProshiren.IsChecked == false)
            {
                string pom1 = rbSistematski.IsChecked.ToString();
                MessageBox.Show("Мора да изберете тип на преглед за пациентот " + pom1, "Грешка!");
                return;
            }

            if (lboxVraboteni.SelectedIndex < 0)
            {
                return;
            }

            ListBoxItem employeeItem = lboxVraboteni.SelectedItem as ListBoxItem;
            var EmployeeID = int.Parse(employeeItem.Tag.ToString());

            using (var db = new DatabaseContext())
            {
                var review = new Review();

                int tip = 0;
                int cena = 0;
                if (rbSistematski.IsChecked == true) { tip = 1; cena = 800; }
                else if (rbDopolnitelen.IsChecked == true) { tip = 2; cena = 1000; }
                else if (rbProshiren.IsChecked == true) { tip = 3; cena = 1200; }
                else if (rbSpecifichen.IsChecked == true) { tip = 4; cena = 1500; }
                else if (rbNasochen.IsChecked == true) { tip = 5; cena = 1000; }

                review.ReviewPrice = cena;
                review.ReviewType = String.Format("{0}",tip);
                review.ReviewIsPayed = false;

                //TODO CHECK STRING FORMAT BEFORE PARSING ! 
                try
                {
                    var shortDate = dpDatumPregled.Text.ToString().Split(' ')[0].Split('.');
                    var year = int.Parse(shortDate[2]);
                    var month = int.Parse(shortDate[1]);
                    var day = int.Parse(shortDate[0]);
                    review.ReviewDate = new DateTime(year, month, day);
                }
                catch (Exception err)
                {
                    MessageBox.Show("Погрешен формат за датум (dd.mm.yyyy)", "Грешка!");
                }
                review.ReviewDate = DateTime.Now;
                
                review.EmployeeID = EmployeeID;

                db.Reviews.Add(review);
                creator.Review = review;
                db.SaveChanges();
                var BusinessID = -1;

                var eQuery = from employee in db.Employees
                            where employee.EmployeeID == EmployeeID
                            select employee;

                foreach (var eItem in eQuery)
                {
                    creator.Employee = eItem;
                    BusinessID = eItem.BusinessID;
                }

                var bQuery = from business in db.Businesss
                            where business.BusinessID == BusinessID
                            select business;

                foreach (var bItem in bQuery)
                {
                    creator.Business = bItem;
                }

            }
            FillLboxPregledi(EmployeeID);


            //creator.ime_vraboten = tbImeVraboten.Text.ToString();
            //creator.mesto_ragjanje = tbMestoRagjanje.Text.ToString();
            //creator.vrab_profesija = tbProfesija.Text.ToString();
            //creator.vrab_rabmesto = tbRabotnoMesto.Text.ToString();
            //creator.datum_rag = cboxDay.SelectedValue.ToString() + "." + cboxMonth.SelectedValue.ToString() +
            //    "." + cboxYear.SelectedValue.ToString();
            //var cbxit = cboxFirmi.SelectedItem as ComboBoxItem;
            //creator.naziv_firma = cbxit.Content.ToString();
            //creator.id_pregled = pregledID;
            //string[] str5 = dpDatumPregled.Text.ToString().Split(' ')[0].Split('-');
            //creator.datum_pregled = str5[2] + "." + str5[1] + "." + str5[0];
            creator.GenerateDocs();

        }

        private void btnNaplati_Click(object sender, RoutedEventArgs e)
        {

            if (lboxFirmi.SelectedIndex != -1)
            {

                using (var db = new DatabaseContext())
                {

                    foreach (var listItem in lbPregledivoFirma.Items)
                    {
                        var ReviewID = int.Parse((listItem as ListBoxItem).Tag.ToString());
                        var query = from b in db.Reviews
                                    where b.ReviewID == ReviewID
                                    select b;
                        
                        foreach (var item in query)
                        {
                            item.ReviewIsPayed = true;
                        }
                        db.SaveChanges();
                    }
                    
                }
                RefreshLists();
            }

        }

        private void RefreshLists()
        {
            //LOL
            var index = lboxFirmi.SelectedIndex;
            lboxFirmi.SelectedIndex = -1;
            lboxFirmi.SelectedIndex = index;

            index = lboxVraboteni.SelectedIndex;
            lboxVraboteni.SelectedIndex = -1;
            lboxVraboteni.SelectedIndex = index;

            index = lboxPregledi.SelectedIndex;
            lboxPregledi.SelectedIndex = -1;
            lboxPregledi.SelectedIndex = index;

            index = lbPregledivoFirma.SelectedIndex;
            lbPregledivoFirma.SelectedIndex = -1;
            lbPregledivoFirma.SelectedIndex = index;



        }
    }

    

    public class Business
    {
        public int BusinessID { get; set; }
        public string BusinessName { get; set; }
        public string BusinessAddress { get; set; }
        public string BusinessEdb { get; set; }
        public string BusinessEmb { get; set; }
        public string BusinessRemark { get; set; }
    }

    public class Employee
    {
        public int EmployeeID { get; set; }
        public string EmployeeNameSurname { get; set; }
        public string EmployeeWorks { get; set; }
        public string EmployeeProffesion { get; set; }
        public string EmployeeAddress { get; set; }
        public DateTime EmployeeBirthDate { get; set; }

        public int BusinessID { get; set; }
        public virtual Business Business { get; set; }
    }

    public class Review
    {
        public int ReviewID { get; set; }
        public DateTime ReviewDate { get; set; }
        public string ReviewType { get; set; }
        public double ReviewPrice { get; set; }
        public bool ReviewIsPayed { get; set; }

        public int EmployeeID { get; set; }
        public virtual Employee Employee { get; set; }
    }

    public class DatabaseContext : DbContext
    {
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Business> Businesss { get; set; }
    }





}