using Acme.UserInfoCollector.Middleware;
using System.ComponentModel.DataAnnotations;

namespace Acme.UserInfoCollector.ConsoleApp
{
    internal static class UserExtensions
    {
        internal static T GetFromConsole<T>(this PersonVM user, string property, Func<string?, T> validator, string message, string invalidMessage)
        {
            return user.GetFromConsole<T>(property, validator, new List<string>() { message }, invalidMessage);
        }

        internal static T GetFromConsole<T>(this PersonVM user, string property, Func<string?, T> validator, IEnumerable<string> messages, string invalidMessage)
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
                            user.GetType().GetProperty(property)?.SetValue(user, toReturn);
                            var ctx = new ValidationContext(user, null, null);
                            Validator.ValidateObject(user, ctx, true);
                            Console.WriteLine();
                            return toReturn;
                        }
                    }
                    catch (ValidationException vex)
                    {
                        Console.WriteLine(vex.Message);
                        Console.WriteLine();
                        continue;
                    }
                }
                
                Console.WriteLine(invalidMessage);
                Console.WriteLine();
            }
        }
    }
}
