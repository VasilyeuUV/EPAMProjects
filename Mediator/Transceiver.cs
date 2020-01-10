public static class Transceiver
{
    private static object locker = new object();

    //public static event EventHandler<string> PropertyChanged;

    public static void Transmit(string fileName)
    {
        lock (locker)
        {
            epam_task4.Program.MediatorStartProcess(fileName);
            //MediatorStartProcess(fileName);
            //PropertyChanged?.Invoke(typeof(Transceiver), fileName);
        }
    }
}


