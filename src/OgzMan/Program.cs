using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgzMan
{
    class Program
    {
        static void Main(string[] args)
        {
            char selectChar = 'a';
            
            do
            {
                Console.WriteLine("\n------------------------------------------------------------------");
                Console.WriteLine("What do yo want?");
                Console.WriteLine("1: OgzContext");
                Console.WriteLine("e: Exit");
                Console.Write("\nPlease select a char: ");
                selectChar = Console.ReadKey().KeyChar;
                switch (selectChar)
                {
                    //OgzContext
                    case '1':
                        Do_OgzContext();
                        break;
                    case 'e':
                    case 'E':
                        Console.WriteLine("\nExiting...");
                        break;
                }
            } while (selectChar != 'e' && selectChar != 'E');

            Console.ReadLine();
        }

        static void Do_OgzContext()
        {

            char selectChar = 'a';
            do
            {
                Console.WriteLine("\n============================OgzContext============================");

                Console.WriteLine("What do yo want?");
                Console.WriteLine("1: Install & Register Server for x86");
                Console.WriteLine("2: Install & Register Server for x64");
                Console.WriteLine("3: Uninstall & Register Server for x86");
                Console.WriteLine("4: Uninstall & Register Server for x64");

                Console.WriteLine("e: Cancel");
                Console.Write("\nPlease select a char: ");
                selectChar = Console.ReadKey().KeyChar;
                switch (selectChar)
                {
                    //OgzContext
                    case '1':
                        OgzContext_InstRegServerX86();
                        break;
                    case '2':
                        OgzContext_InstRegServerX64();
                        break;
                    case 'e':
                    case 'E':
                        Console.WriteLine("\nCancel OgzContext");
                        break;
                }
            } while (selectChar != 'e' && selectChar != 'E');
        }

        static void OgzContext_InstRegServerX86()
        {
            Console.Write("\n");
            Console.WriteLine("RegServer X86");
            Console.ReadLine();
        }

        static void OgzContext_UnInstRegServerX86()
        {
            Console.Write("\n");
            Console.WriteLine("UnRegServer X86");
            Console.ReadLine();
        }

        static void OgzContext_InstRegServerX64()
        {
            Console.Write("\n");
            Console.WriteLine("RegServer X86");
            Console.ReadLine();
        }

        static void OgzContext_UnInstRegServerX64()
        {
            Console.Write("\n");
            Console.WriteLine("UnRegServer X64");
            Console.ReadLine();
        }
    }
}
