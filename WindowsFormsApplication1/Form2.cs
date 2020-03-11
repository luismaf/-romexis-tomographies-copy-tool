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
        public const string tomoFolder = "Tomografías";
        public const string appFile = "Romexis_Viewer_Win.exe";
        public string sourcePath = "";
        public string sourceSize = "";
        public string destPath = "";
        //public string appFilePath = "";
        //public string appSource = "";
        
        public Form1()
        {
            InitializeComponent();

            //Get current-user home folder:
            string userPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6) userPath = System.IO.Path.Combine(Directory.GetParent(userPath).ToString(), tomographyFolderName);
            txtDestination.Text = System.IO.Path.Combine(userPath, tomoFolder);

            //fill txtComboSource with DVD drives:
            txtComboSource.DataSource = System.IO.DriveInfo.GetDrives();
            //.Where(d => d.DriveType == System.IO.DriveType.CDRom);
            txtComboSource.DisplayMember = "Name";
            
        }
        private void buttonCopiar_Click(object sender, EventArgs e)
        {
            
            const string msgOverwrite = "Atención: la carpeta ya existe! ¿desea sobreescribirla?";
            const string msgWrongDisc = "Atención: la unidad seleccionada no contiene un disco válido"";
            const string titleOverwrite = "¿Sobreescribir carpeta?";
            const string titleWrongDisc = "Disco Incorrecto";
            
            //set public vars:
            sourcePath = txtComboSource.SelectedItem.ToString();
            destPath = System.IO.Path.Combine(txtDestination.Text,txtPatient.Text);
            //set local vars:
            string sourceAppFile = System.IO.Path.Combine(sourcePath, appFile);

            
            buttonCopiar.Enabled = false;

            //alert if textBoxDesc is empty:
            if (txtPatient.Text.Length <= 0)
            {
                MessageBox.Show("Debe ingresar una descripción");
                return;
            }
            
            //Check if folder exist
            if (Directory.Exists(destPath))
            {
                var result = MessageBox.Show(msgOverwrite, titleOverwrite,
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);
                // If the no button was pressed ...
                if (result == DialogResult.No) return;
            }
            //check if source folder contains Roemexis app: (ie: D:\Romexis_Viewer_Win.exe exists)
            if (!File.Exists(sourceAppFile)) {
                MessageBox.Show(msgWrongDisc, titleWrongDisc);
                return;
            }

            backgroundCopyWorker.RunWorkerAsync();
        }
        private void runApp()
        {
            string destFolder = 
       
            // Use Path class to manipulate file and directory paths.
            string destFolder = System.IO.Path.Combine(destination, , description);
            string path = System.IO.Path.Combine(destFolder, appExeFilename);


            buttonCopiar.Enabled = true;
            //progressBar.Value = 100;
            MessageBox.Show("Done");

            string appLocalPath = appFilename = 

            //To open the app:
            if (File.Exists(appPath))
            {
                System.Diagnostics.Process.Start(appPath);
            }
            else if (Directory.Exists(destPath))
            {
                // If app doesn't exist, open the folder in file explorer:
                System.Diagnostics.Process.Start("explorer.exe", destPath);
            }
            else
            {
                MessageBox.Show("Ocurrio un error al copiar el contenido desde \'" + sourcePath + "\' a '" + destPath + "\'");
            }
        }

        private void myBGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // To copy a folder's contents to a new location:
            Copy(sourcePath, destPath);
        }
        
        void myBGWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        public void Copy(string sPath, string dPath)
        {
            backgroundCopyWorker.ReportProgress(0);
            var sourceFolder = new DirectoryInfo(sPath);
            var destFolder = new DirectoryInfo(dPath);

            CopyAll(sourceFolder, destFolder);
            backgroundCopyWorker.ReportProgress(100);
        }

        public void CopyAll(DirectoryInfo sourceFolder, DirectoryInfo destFolder)
        {
            Directory.CreateDirectory(destFolder.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fileInfo in sourceFolder.GetFiles())
            {
                //fileInfo.Length //in bytes:
                fileInfo.CopyTo(System.IO.Path.Combine(destFolder.FullName, fileInfo.Name), true);
                backgroundCopyWorker.ReportProgress(99);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in sourceFolder.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    destFolder.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        
        void myBGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            runApp();
        }

    }

}
