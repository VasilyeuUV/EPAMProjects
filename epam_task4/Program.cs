using epam_task4.Threads;
using System;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;
using System.Configuration.Install;
using System.Management.Automation.Runspaces;
using System.Management.Automation;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace epam_task4
{
    class Program
    {
        static void Main(string[] args)
        {


            // DATABASE
            //InstallDataBase();

            // INSTALL SERVICE
            InstallService(@"E:\_projects\epam\task4\epam_task4\fwService\bin\Debug\", @"fwService.exe");
            //ManagedInstallerClass.InstallHelper(new[] {"FWService.exe"});

            //START SERVICE
            StartService();




            //ServiceController svcController = new ServiceController("FWService");
            //try
            //{
            //    if (svcController.Status != ServiceControllerStatus.Running) 
            //    { 
            //        StartService(svcController); 
            //    }
            //}
            //catch (Exception)
            //{
            //}




            // START EMULATOR
            EmulatorThread eThread = null;
            Form emulator = emulatorWFA.FormMain.StartForm(true);
            if (emulator != null) { eThread = new EmulatorThread(emulator); }



            Console.WriteLine("Press any key to Exit");
            Console.ReadKey();

            eThread?.Close();
            StopService();
        }



        /// <summary>
        /// Install Service
        /// </summary>
        /// <param name="v"></param>
        private static void InstallService(string path, string serviceFile)
        {
            string servicePath = path + serviceFile; 
            if (System.IO.File.Exists(servicePath))
            {
                try
                {
                    ManagedInstallerClass.InstallHelper(new[] { servicePath });
                }
                catch (Exception e)
                {
                }
            }
        }


        /// <summary>
        /// Start Service
        /// </summary>
        private static void StartService()
        {
            using (ServiceController svcController = new ServiceController("FWService"))
            {
                try
                {
                    if (svcController.Status != ServiceControllerStatus.Running)
                    {
                        svcController.Start();
                        WaitStatus(svcController, ServiceControllerStatus.Running);
                    }
                }
                catch (Exception)
                {
                }
                svcController.Close();
            }
        }




        /// <summary>
        /// Stop Service
        /// </summary>
        private static void StopService()
        {
            using (ServiceController svcController = new ServiceController("FWService"))
            {
                try
                {
                    if (svcController.Status != ServiceControllerStatus.Stopped)
                    {
                        svcController.Stop();
                        WaitStatus(svcController, ServiceControllerStatus.Stopped);
                    }
                }
                catch (Exception)
                {
                }
                svcController.Close();
            }
        }


        /// <summary>
        /// Wait for change service status
        /// </summary>
        /// <param name="svcController"></param>
        /// <param name="status"></param>
        private static void WaitStatus(ServiceController svcController, ServiceControllerStatus status)
        {
            while (svcController.Status != status)
            {
                Thread.Sleep(100);
                svcController.Refresh();
            }
        }





        private IEnumerable<string> PowerShellCall(string script, Hashtable args)
        {
        // создаю конфигурацию сессии PowerShell
        InitialSessionState state = InitialSessionState.CreateDefault();

        // меняем политику обработки ошибок в скриптах: 
        // любая ошибка будет приводить к моментальному завершению и выбросу исключения, 
        // которое мы можем легко и не принуждённо поймать и обработать
        state.Variables.Add(new SessionStateVariableEntry(
                "ErrorActionPreference", "Stop", null));

        // передаю параметры моему скрипту как переменная с именем Arguments и типом hashtable:
        // $DeviceMAC = $Arguments["DeviceMAC"]
        state.Variables.Add(new SessionStateVariableEntry(
            "Arguments", args, null));

        // создаю и открываю командную оболочку PowerShell,
        // представленную экземпляром класса System.Management.Automation.Runspaces.Runspace. 
        using (Runspace runspace = RunspaceFactory.CreateRunspace(state))
            {
                runspace.Open();

                // присоединяю новый конвейер команд (pipeline) PowerShell`а:
                using (PowerShell shell = PowerShell.Create())
                {
                    shell.Runspace = runspace;

                    // добавить свою команду к скрипту
                    shell.AddScript("Set-PSDebug -Strict\n" + script);

                    // вызвать скрипт, прочитать результат
                    try
                    {
                        return new List<string>(
                            from PSObject obj in shell.Invoke()
                            where obj != null
                            select obj.ToString());
                    }
                    // обработать ошибки
                    catch (RuntimeException psError)
                    {
                        ErrorRecord error = psError.ErrorRecord;
                        return new List<string>(error.Exception.HResult);
                        //return error.InvocationInfo == null
                        //    ? FormatErrorSimple(error.Exception)
                        //    : FormatError(error.InvocationInfo, error.Exception);
                    }
                }
            }
        }


    }
}
