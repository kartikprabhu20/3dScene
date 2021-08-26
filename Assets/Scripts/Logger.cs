using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO;

public static class Logger
{
    public static string logFilePath = "/Users/apple/OVGU/Thesis/3dscene.log";

    //public Logger(string logFilePath)
    //{
    //    if (!logFilePath.EndsWith(".log"))
    //        logFilePath += ".log";
    //    LogFilePath = logFilePath;
    //    if (!File.Exists(LogFilePath))
    //        File.Create(LogFilePath).Close();
    //    WriteLine("New Session Started");
    //}

    //public string LogFilePath { get; private set; }

    public static void WriteLine(object message)
    {

        if (!File.Exists(logFilePath)) {
            File.Create(logFilePath).Close();
            WriteLine("New Session Started");
        }
            
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
            writer.WriteLine(DateTime.Now.ToString() + ": " + message.ToString());
    }

}
