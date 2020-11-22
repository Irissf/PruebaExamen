using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PruebaExamen
{
    class Program
    {
        static bool finalCarrera = false;
        static bool liebreDurmiendo = false;

        static readonly object dormirLlave = new object();
        static readonly object llave = new object();

        static int pasosTotalesLiebre = 0;
        static int pasosTotalesTortuga = 0;
        static Random rand = new Random();
        static string ganador;

        static void Main(string[] args)
        {
            //StreamReader sr;
            StreamWriter sw;
            if (File.Exists("ganadores.txt"))
            {
                Process p = Process.Start("Notepad", "ganadores.txt");
                p.WaitForExit();
            }
            else
            {
                Console.WriteLine("El archivo no existe");
            }

            Thread correTortuga = new Thread(CorreTortuga);
            Thread correLiebre = new Thread(CorreLiebre);
            correTortuga.Start();
            correLiebre.Start();
            correTortuga.Join();
            correLiebre.Join();

            using (sw = new StreamWriter("ganadores.txt", true))
            {
                sw.WriteLine("El ganador fue: " + ganador);
            }

            FileInfo f = new FileInfo("ganadores.txt");
            string directorio = f.DirectoryName;
            Console.WriteLine("El archivo fué guardado en en el directorio:{0}", directorio);
            DirectoryInfo direct = new DirectoryInfo(directorio);
            FileInfo[] files = direct.GetFiles();
            foreach (FileInfo item in files)
            {
                Console.WriteLine("Archivo:{0},{1}", item.Name,item.Length);

            }
            Console.ReadLine();
        }

        public static void CorreTortuga()
        {
            while (!finalCarrera)
            {
                lock (llave)
                {
                    if (!finalCarrera)
                    {
                        pasosTotalesTortuga = pasosTotalesTortuga + 1;
                        if (pasosTotalesTortuga >= 25)
                        {
                            pasosTotalesLiebre = 25;
                            ganador = "Tortuga";
                            finalCarrera = true;
                        }
                        if (pasosTotalesLiebre == pasosTotalesTortuga)
                        {
                            int probsesenta = rand.Next(1, 101);
                            if (probsesenta <= 50)
                            {
                                lock (dormirLlave)
                                {
                                    Console.WriteLine("La tortuga hace ruido");
                                    Monitor.Pulse(dormirLlave);
                                    Console.WriteLine("liebre despierta");
                                }
                            }

                        }
                        Console.WriteLine("Tortuga" + pasosTotalesTortuga);
                    }

                }
                Thread.Sleep(300);
            }
        }

        public static void CorreLiebre()
        {
            while (!finalCarrera)
            {
                lock (llave)
                {
                    if (!finalCarrera)
                    {
                        pasosTotalesLiebre = pasosTotalesLiebre + 6;
                        if (pasosTotalesLiebre >= 25)
                        {
                            pasosTotalesLiebre = 25;
                            ganador = "Liebre";
                            finalCarrera = true;
                        }
                        Console.WriteLine("Liebre" + pasosTotalesLiebre);
                    }

                }
                if (!finalCarrera)
                {
                    Thread.Sleep(200);
                    int probsesenta = rand.Next(1, 101);
                    if (probsesenta <= 60)
                    {
                        Thread dormir = new Thread(Dormir);
                        dormir.IsBackground = true;
                        dormir.Start();
                        lock (dormirLlave)
                        {
                            liebreDurmiendo = true;
                            Monitor.Wait(dormirLlave);

                        }
                    }
                }

            }
        }

        public static void Dormir()
        {
            if (!finalCarrera)
            {
                if (liebreDurmiendo)
                {
                    Console.WriteLine("Liebre duerme");
                    Thread.Sleep(rand.Next(1000, 2500));
                    Console.WriteLine("Liebre despierta");
                    lock (dormirLlave)
                    {
                        Monitor.Pulse(dormirLlave);
                        liebreDurmiendo = false;
                    }
                }

            }

        }
    }
}
