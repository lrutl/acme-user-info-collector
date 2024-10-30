using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acme.UserInfoCollector.ConsoleApp
{
    internal class Prompter
    {
        internal static T GetFromUser<T>(Func<string?, T> validator, string message, string invalidMessage)
        {
            return GetFromUser<T>(validator, new List<string>() { message }, invalidMessage);
        }

        internal static T GetFromUser<T>(Func<string?, T> validator, IEnumerable<string> messages, string invalidMessage)
        {
            while (true)
            {
                foreach (var message in messages)
                {
                    Console.WriteLine(message);
                }

                var toValidate = Console.ReadLine();
                if (toValidate != null)
                {
                    try
                    {
                        if (validator.Invoke(toValidate) is T toReturn)
                        {
                            return toReturn;
                        }
                    }
                    catch { }
                }
                
                Console.WriteLine(invalidMessage);
            }
        }
    }
}
