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
        public string destPath = "";
        //public string appFilePath = "";
        //public string appSource = "";
        
        public Form1()
        {
            InitializeComponent();

            //Get current-user home folder:
            string userPath = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).FullName;
            if (Environment.OSVersion.Version.Major >= 6) userPath = System.IO.Path.Combine(Directory.GetParent(userPath).ToString());
            txtDestination.Text = System.IO.Path.Combine(userPath, tomoFolder);

            DriveInfo[] allDrives = DriveInfo.GetDrives();

          

            //fill txtComboSource with CDRom drives:
            txtComboSource.DataSource = System.IO.DriveInfo.GetDrives()
    .Where(d => d.DriveType == System.IO.DriveType.CDRom).ToList();
txtComboSource.DisplayMember = "Name";

            progressBar.Value = 0;
            progressBar.MarqueeAnimationSpeed = 0;

            
        }
        private void buttonCopiar_Click(object sender, EventArgs e)
        {
            const string msgNoDrive = "Atención: Debe seleccionar un disco válido.";
            const string msgDriveNotReady = "Atención: El disco no está listo.";
            const string msgOverwrite = "Atención: la carpeta ya existe! ¿desea sobreescribirla?";
            const string msgWrongDisc = "Atención: la unidad seleccionada no contiene un disco válido";
            const string msgEmptyPatient = "Atención: el campo paciente debe contener el nombre del paciente";
            const string titleNoDrive = "Disco no válido";
            const string titleDriveNotReady = "Disco no disponible";
            const string titleOverwrite = "¿Sobreescribir carpeta?";
            const string titleWrongDisc = "Disco incorrecto";
            const string titleEmptyPatient = "Campo \'Paciente\' Vacio";

            if (txtComboSource.SelectedItem == null) {
                MessageBox.Show(msgNoDrive, titleNoDrive);
                return;
            }

            //set public vars:
            sourcePath = txtComboSource.SelectedItem.ToString();
            destPath = System.IO.Path.Combine(txtDestination.Text,txtPatient.Text);
            //set local vars:
            string sourceAppFile = System.IO.Path.Combine(sourcePath, appFile);
            
            //check if DVD is ready
            DriveInfo selectedDrive = (DriveInfo) txtComboSource.SelectedItem;
            if (!selectedDrive.IsReady)
            {
                MessageBox.Show(msgDriveNotReady, titleDriveNotReady);
                return;
            }
            //alert if textBoxDesc is empty:
            if (txtPatient.Text.Length <= 0)
            {
                MessageBox.Show(msgEmptyPatient, titleEmptyPatient);
                return;
            }
            //check if source folder contains Roemexis app: (ie: D:\Romexis_Viewer_Win.exe exists)
            if (!File.Exists(sourceAppFile))
            {
                MessageBox.Show(msgWrongDisc, titleWrongDisc);
                return;
            }
            //Check if destination folder already exist
            if (Directory.Exists(destPath))
            {
                var result = MessageBox.Show(msgOverwrite, titleOverwrite,
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);
                // If the no button was pressed ...
                if (result == DialogResult.No) return;
            }
            buttonCopiar.Enabled = false;
            progressBar.Value = 50;
            progressBar.MarqueeAnimationSpeed = 30;
            backgroundCopyWorker.RunWorkerAsync();
        }
        private void runApp()
        {
            string destAppFile = System.IO.Path.Combine(destPath, appFile);
            string msgCopyError = "Ocurrio un error al copiar el contenido desde \'" + sourcePath + "\' a '" + destPath + "\'";
            string titleCopyError = "Error al copiar";
            buttonCopiar.Enabled = true;
            progressBar.Value = 0;
            progressBar.MarqueeAnimationSpeed = 0;

            //To open the app:
            if (File.Exists(destAppFile))
            {
                System.Diagnostics.Process.Start(destAppFile);
            }
            else if (Directory.Exists(destPath))
            {
                // If app doesn't exist, open the folder in file explorer:
                System.Diagnostics.Process.Start("explorer.exe", destPath);
            }
            else
            {
                MessageBox.Show(msgCopyError, titleCopyError);
            }
        }

        private void backgroundCopyWorker_DoWork_1(object sender, DoWorkEventArgs e)
        {
            // To copy a folder's contents to a new location:
            Copy(sourcePath, destPath);
        }
        
        void backgroundCopyWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            //progressBar.Value = (e.ProgressPercentage > 98) ? 100 : e.ProgressPercentage;
        }

        public void Copy(string sPath, string dPath)
        {
            var sourceFolder = new DirectoryInfo(sPath);
            var destFolder = new DirectoryInfo(dPath);
            CopyAll(sourceFolder, destFolder);
            backgroundCopyWorker.ReportProgress(100);
        }

        public void CopyAll(DirectoryInfo sourceFolder, DirectoryInfo destFolder)
        {
            Directory.CreateDirectory(destFolder.FullName);

            backgroundCopyWorker.ReportProgress(0);
            // Copy each file into the new directory.
            foreach (FileInfo fileInfo in sourceFolder.GetFiles())
            {
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
        
        void backgroundCopyWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            runApp();
        }


    }

}
