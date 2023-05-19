namespace CRL.EmailService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.CRLMailServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.CRLMailServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // CRLMailServiceProcessInstaller
            // 
            this.CRLMailServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.CRLMailServiceProcessInstaller.Password = null;
            this.CRLMailServiceProcessInstaller.Username = null;
            this.CRLMailServiceProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.CRLMailServiceProcessInstaller_AfterInstall);
            // 
            // CRLMailServiceInstaller
            // 
            this.CRLMailServiceInstaller.ServiceName = "CRLMailService";
            this.CRLMailServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.CRLMailServiceProcessInstaller,
            this.CRLMailServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller CRLMailServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller CRLMailServiceInstaller;
    }
}