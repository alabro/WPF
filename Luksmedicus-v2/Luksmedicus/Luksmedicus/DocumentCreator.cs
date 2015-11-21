using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Novacode;
using System.IO;

namespace Luksmedicus
{
    class DocumentCreator
    {
        public string naziv_firma;
        public string ime_vraboten;
        public string datum_rag;
        public string datum_pregled;
        public int id_pregled;
        public string mesto_ragjanje;
        public string tip_pregled;

        public void GenerateDocs() 
        {
            if (!Directory.Exists("Правни лица/" + naziv_firma + "/" + ime_vraboten))
            {
                Console.WriteLine("Правни лица/" + naziv_firma + "/" + ime_vraboten);
                Directory.CreateDirectory("Правни лица/" + naziv_firma + "/" + ime_vraboten);
            }
            CreateDocFizicko();
            CreateDocPravno();
            //TODO DOKUMENTITE TANASIJ
        }

        public void CreateDocFizicko()
        {
            // Modify to suit your machine:
            string fileName = @"Правни лица/"+ naziv_firma +"/"+ ime_vraboten +"/" + ime_vraboten + "_" + id_pregled + "(ZA_VRABOTEN).docx";

            // Create a document in memory:
            var doc = DocX.Create(fileName);
            
            // Insert a paragrpah:
            doc.InsertParagraph("This is my first paragraph for vraboten " + ime_vraboten);

            // Save to the output directory:
            doc.Save();

            
        }
        public void CreateDocPravno()
        {
            // Modify to suit your machine:
            string fileName = @"Правни лица/" + naziv_firma + "/" + ime_vraboten + "/" 
                + ime_vraboten + "_" + id_pregled + "(ZA_FIRMA).docx";

            // Create a document in memory:
            var doc = DocX.Create(fileName);

            // Insert a paragrpah:
            doc.InsertParagraph("This is my first paragraph for firma " + naziv_firma);

            // Save to the output directory:
            doc.Save();

            
        }
    }
}
