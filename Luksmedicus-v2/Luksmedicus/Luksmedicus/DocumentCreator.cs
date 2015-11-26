using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Novacode;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Luksmedicus
{
    class DocumentCreator
    {
        public Employee Employee { get; set; }
        public Business Business { get; set; }
        public Review Review { get; set; }


        public void GenerateDocs()
        {
            if (!Directory.Exists("Правни лица/" + Business.BusinessName + "/" + Employee.EmployeeNameSurname))
            {
                Directory.CreateDirectory("Правни лица/" + Business.BusinessName + "/" + Employee.EmployeeNameSurname);
            }
            CreateDocFizicko();
            CreateDocPravno();
        }

        public void CreateDocFizicko()
        {
            string fileName = @"Правни лица/" + Business.BusinessName + "/" + Employee.EmployeeNameSurname + "/" + Employee.EmployeeNameSurname + "_" + Review.ReviewID + "(ZA_VRABOTEN).docx";
            var doc = DocX.Load("templ/templ(ZAVRABOTEN).docx");
            doc.ReplaceText("<ime na vraboten>", Employee.EmployeeNameSurname);
            doc.ReplaceText("<datum na ragjanje>", Employee.EmployeeBirthDate.ToShortDateString());
            doc.ReplaceText("<професија>", Employee.EmployeeProffesion);
            doc.ReplaceText("<rab mesto>", Employee.EmployeeWorks);
            doc.ReplaceText("<datum pregled>", Review.ReviewDate.ToShortDateString());
            Formatting newf = new Formatting();
            newf.UnderlineStyle = UnderlineStyle.singleLine;
            newf.Bold = true;
            newf.Size = 14;
            var tip_pregled = "";
            switch (Review.ReviewType)
            {
                case "1":
                    tip_pregled = "СИСТЕМАТСКИ";
                    break;
                case "2":
                    tip_pregled = "ПЕРИОДИЧЕН";
                    break;
                case "3":
                    tip_pregled = "ПЕРИОДИЧЕН";
                    break;
                case "4":
                    tip_pregled = "ПЕРИОДИЧЕН";
                    break;
                case "5":
                    tip_pregled = "НАСОЧЕН";
                    break;
            }
            doc.ReplaceText(tip_pregled, tip_pregled, false, System.Text.RegularExpressions.RegexOptions.None, newf);
            doc.SaveAs(fileName);
        }
        public void CreateDocPravno()
        {
            string fileName = @"Правни лица/" + Business.BusinessName + "/" + Employee.EmployeeNameSurname + "/" + Employee.EmployeeNameSurname + "_" + Review.ReviewID + "(ZA_FIRMA).docx";
            var doc = DocX.Load("templ/templ(ZAFIRMA).docx");
            doc.ReplaceText("<ime na vraboten>", Employee.EmployeeNameSurname);
            doc.ReplaceText("<datum na ragjanje>", Employee.EmployeeBirthDate.ToShortDateString());
            doc.ReplaceText("<професија>", Employee.EmployeeProffesion);
            doc.ReplaceText("<rab mesto>", Employee.EmployeeWorks);
            doc.ReplaceText("<datum pregled>", Review.ReviewDate.ToShortDateString());
            Formatting newf = new Formatting();
            newf.UnderlineStyle = UnderlineStyle.singleLine;
            newf.Bold = true;
            newf.Size = 14;

            var tip_pregled = "";
            switch (Review.ReviewType)
            {
                case "1":
                    tip_pregled = "СИСТЕМАТСКИ";
                    break;
                case "2":
                    tip_pregled = "ПЕРИОДИЧЕН";
                    break;
                case "3":
                    tip_pregled = "ПЕРИОДИЧЕН";
                    break;
                case "4":
                    tip_pregled = "ПЕРИОДИЧЕН";
                    break;
                case "5":
                    tip_pregled = "НАСОЧЕН";
                    break;
            }
            doc.ReplaceText(tip_pregled, tip_pregled, false, System.Text.RegularExpressions.RegexOptions.None, newf);
            doc.SaveAs(fileName);

        }

    }
}