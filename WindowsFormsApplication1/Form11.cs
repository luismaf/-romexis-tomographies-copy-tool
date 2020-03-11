using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public const string appFilename = "Romexis_Viewer_Win.exe";
        public string sourceFolder = "";
        public string sourceSize = "";
        public string destFolder = "";
        //public string appFilePath = "";
        //public string appSource = "";
        
        public Form1()
        {
            InitializeComponent();

            //fill txtComboSource with DVD drives:
            txtComboSource.DataSource = System.IO.DriveInfo.GetDrives();
            //.Where(d => d.DriveType == System.IO.DriveType.CDRom);
            txtComboSource.DisplayMember = "Name";
          
            
            //Get current-user home folder:
            string userPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6)
            {
                 txtDestination.Text = Directory.GetParent(userPath).ToString();
            }
        }
        private void buttonCopiar_Click(object sender, EventArgs e)
        {
            buttonCopiar.Enabled = false;

            //MessageBox.Show(destFolder); return;
            //alert if textBoxDesc is empty
            if (txtPatient.Text.Length <= 0)
            {
                MessageBox.Show("Debe ingresar una descripción");
                return;
            }

            
            //Check if folder exist
            if (Directory.Exists(destFolder))
            {
                const string message = "Atención: la carpeta ya existe! ¿desea sobreescribirla?";
                const string caption = "¿Sobreescribir carpeta?";
                var result = MessageBox.Show(message, caption,
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);
                // If the no button was pressed ...
                if (result == DialogResult.No) return;
            }
            //check if source folder is a Roemxis DVD by checking its autorun filename ie: D:\Romexis_Viewer_Win.exe
            if (!File.Exists(System.IO.Path.Combine(sourceFolder, appFilename) {
                MessageBox.Show("La unidad seleccionada no contiene tomografías Romexis");
                return;
            }
            MessageBox.Show(txtComboSource.SelectedItem.ToString());
            myBGWorker.RunWorkerAsync();
        }
        // This worker will copy DVD content to a new patient folder, inside tomograhy folder:
        private void myBGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var source = new DirectoryInfo(sourceFolder);
            var destination = new DirectoryInfo(destFolder);

            
            Copy(sourceFolder, destFolder);

            Directory.CreateDirectory(destination.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.Length //in bytes:
                //fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
                myBGWorker.ReportProgress(99);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    destination.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }

        }
        void myBGWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        public void Copy(string sourceDirectory, string targetDirectory)
        {
            myBGWorker.ReportProgress(0);
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
            myBGWorker.ReportProgress(100);
        }

        public void CopyAll()
        {

        }




        void myBGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonCopiar.Enabled = true;
            //progressBar.Value = 100;
            MessageBox.Show("Done");

            //To open the app:
            if (File.Exists(appPath))
            {
                System.Diagnostics.Process.Start(appPath);
            }
            else if (Directory.Exists(destFolder))
            {
                // If app doesn't exist, open the folder in file explorer:
                System.Diagnostics.Process.Start("explorer.exe", destFolder);
            }
            else
            {
                MessageBox.Show("Ocurrio un error al copiar el contenido desde \'" + sourceFolder + "\' a '" + destFolder + "\'");
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void textBoxDesc_TextChanged(object sender, EventArgs e)
        {

        }

    }
    static class Path
    {
        public static string sourceFolder = "";
        public static string destFolder = "";
        public static string appExeFilePath = "";
        
        public void setFolders(string source, string destination) 
        {
            string tomographyFolderName = "Tomografías";
            
            string destFolder = 
       
            // Use Path class to manipulate file and directory paths.
            string destFolder = System.IO.Path.Combine(destination, tomographyFolderName, description);
            string path = System.IO.Path.Combine(destFolder, appExeFilename);

        }


    }

}
