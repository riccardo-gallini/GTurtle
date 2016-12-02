using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle
{
    public class TurtleScript
    {
        public static string NewTurtleScript()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "GTurtle.Turtle.newscript.py";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
                else
                {
                    return null;
                }

            }
        }


    }
}
