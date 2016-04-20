using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace AutomatedSimTemplateTests.Other
{
    [TestClass]
    public class Misc
    {
        public List<string>.Enumerator m_ImageFileNames;
        public IEnumerator<string> ImageFileNames { get; set; }

        [TestMethod]
        public void TestMethod1()
        {
            List<int> list = new List<int>();
            list.Add(1);
            list.Add(5);
            list.Add(9);

            List<int>.Enumerator e = list.GetEnumerator();
            Write(e);
        }

        [TestMethod]
        public void OpenFolderTest()
        {
            OpenFolder();
        }

        public void OpenFolder()
        {
            // Open a folder browsing dialog for the user to select a folder.
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result = fbd.ShowDialog();

            // Obtain the contained files.
            string[] files = Directory.GetFiles(fbd.SelectedPath);

            // Filter for only valid file types
            string[] validFiles = files;
            //IEnumerable<string> validFiles = from file in files
            //                                 where IsValidFile(file)
            //                                 select file;

            ImageFileNames = files.ToList().GetEnumerator();
            LoadFileFromEnumerator();
            LoadFileFromEnumerator();
            LoadFileFromEnumerator();
        }

        /// <summary>
        /// Loads the file into the UI bound property.
        /// </summary>
        /// <param name="filename">The filename.</param>
        private void LoadFile(string filename)
        {
            Console.WriteLine("LoadFile: {0}", filename);
        }

        private void LoadFileFromEnumerator()
        {
            //List<string>.Enumerator it = m_ImageFileNames;
            bool isFile = ImageFileNames.MoveNext();
            if (isFile)
            {
                // If there is a file, load it and await a location input.
                LoadFile(ImageFileNames.Current);
            }
        }

        static void Write(IEnumerator<int> e)
        {
            while (e.MoveNext())
            {
                int value = e.Current;
                Console.WriteLine(value.ToString());
            }
        }
    }
}
