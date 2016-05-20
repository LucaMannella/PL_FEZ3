using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PL_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            bool res;
            String name = "Luca";
            Console.WriteLine("Il programma comincia!");

            Database db = new Database();
            db.OpenConnect();
            res = db.insertName(name);
            db.CloseConnect();

            if(res)
                Console.WriteLine(name+" succesfully inserted!\n");
            else
                Console.WriteLine("Something wrong happens!");

            Console.ReadKey();
        }
    }
}
