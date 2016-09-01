using System;
using System.IO;
using System.Reflection;

namespace WakeOnLan.exe
{
    public static class Log
    {
        public static void Write(string message, bool success)
        {
            string file = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Log.txt";

            StreamWriter log;

            if (!File.Exists(file))
            {
                log = new StreamWriter(file);
            }
            else
            {
                log = File.AppendText(file);
            }

            log.Write(DateTime.Now);
            log.Write("   ");
            if (success)
                log.Write("SUCCESS: ");
            else
                log.Write("ERROR:   ");
            log.Write(message);
            log.WriteLine();

            log.Close();
        }
    }
}
