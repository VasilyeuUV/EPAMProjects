using System.Collections;
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


        //protected override void OnBeforeInstall(IDictionary savedState)
        //{
        //    string parameter = "MySource1\" \"MyLogFile1";

        //    base.OnBeforeInstall(savedState);
        //}


    }
}
