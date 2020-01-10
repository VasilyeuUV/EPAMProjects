using System;
using System.Collections.Generic;


public static class Transceiver
{
    private static object locker = new object();
    private static List<string> Storage { get; } = new List<string>();


    public static event EventHandler<string> PropertyChanged;

    public static void AddItem(string item)
    {
        lock (locker)
        {
            Storage.Add(item);
            PropertyChanged?.Invoke(Storage, item);
        }            
    }

    public static void RemoveItem(string item)
    {
        lock (locker)
        {
            Storage.Add(item);
        }
    }
}


