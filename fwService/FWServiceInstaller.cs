using System.ComponentModel;

namespace fwService
{
    [RunInstaller(true)]
    public partial class FWServiceInstaller : System.Configuration.Install.Installer
    {
        public FWServiceInstaller()
        {
            InitializeComponent();
        }
    }
}
