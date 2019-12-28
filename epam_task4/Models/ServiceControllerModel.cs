using System.ServiceProcess;

namespace epam_task4.Models
{
    internal class ServiceControllerModel
    {
        private readonly ServiceController _controller;
        internal ServiceController Controller => _controller;



        /// <summary>
        /// CTOR
        /// </summary>
        internal ServiceControllerModel(ServiceController controller)
        {
            this._controller = controller;
        }

        
    }
}
