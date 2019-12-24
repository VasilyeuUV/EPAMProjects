using System.ComponentModel;
using System.Configuration.Install;

namespace fwService
{
    [RunInstaller(true)]
    public partial class FWServiceInstaller : System.Configuration.Install.Installer
    {
        public FWServiceInstaller()
        {
            InitializeComponent();
        }

        private void serviceInstallerFW_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void serviceProcessInstallerFW_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
