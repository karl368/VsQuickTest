/*
 * History
 * Date        Ver Author        Change Description
 * ----------- --- ------------- ----------------------------------------
 * 13 Mar 2015 001 Karl          How to use function. 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VsQuickTest.basic.function
{
    class HandleMessage
    {
        private String message;

        public HandleMessage(String message)
        {
            this.message = message;
        }

        public void Handle() 
        {
            // (message) is input param, the the part within {...} is func body
            Func<String, int> func = (message) =>
            {
                int times = new System.Random().Next() % 16 + 1;
                for (int i = 0; i < times; i++)
                {
                    System.Console.WriteLine((i + 1) + ": \t[" + message + "]. ");
                }
                return times;
            };
            
            System.Console.WriteLine("\n\n-->");
            int lines = Handle(func);
            System.Console.WriteLine("Total lines : " + lines + ".");
        } 

        private int Handle (Func<String, int> repeatlyHandlingMessage) 
        {
            return repeatlyHandlingMessage(message);
        }


    }

   
}
