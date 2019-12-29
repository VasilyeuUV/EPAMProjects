using System.Configuration.Install;
using System.ServiceProcess;

namespace fwService
{
    partial class FWServiceInstaller
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstallerFW = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstallerFW = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstallerFW
            // 
            this.serviceProcessInstallerFW.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstallerFW.Password = null;
            this.serviceProcessInstallerFW.Username = null;
            // 
            // serviceInstallerFW
            // 
            this.serviceInstallerFW.DelayedAutoStart = true;
            this.serviceInstallerFW.Description = "File Watcher Service";
            this.serviceInstallerFW.DisplayName = "FWService";
            this.serviceInstallerFW.ServiceName = "FWService";
            this.serviceInstallerFW.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // FWServiceInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstallerFW,
            this.serviceInstallerFW});

            this.serviceInstallerFW.AfterInstall += ServiceInstaller_AfterInstall;
            this.serviceInstallerFW.BeforeUninstall += ServiceInstaller_BeforeUninstall;

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstallerFW;
        private System.ServiceProcess.ServiceInstaller serviceInstallerFW;


        private void ServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            try
            {
                ServiceController sc = new ServiceController("FWService");
                sc.Start();
            }
            catch (System.Exception)
            {
            }

        }

        private void ServiceInstaller_BeforeUninstall(object sender, InstallEventArgs e)
        {
            try
            {
                this.serviceInstallerFW.AfterInstall -= ServiceInstaller_AfterInstall;
                this.serviceInstallerFW.BeforeUninstall -= ServiceInstaller_BeforeUninstall;
                ServiceController sc = new ServiceController("FWService");
                sc.Stop();
            }
            catch (System.Exception)
            {
            }

        }


    }
}