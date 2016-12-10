using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GTurtle
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Gemini.Framework;
    using Gemini.Framework.Services;
    using GScripting.SimpleIDE;

    namespace GTurtle
    {
        [Export(typeof(IEditorProvider))]
        public class TurtleScriptEditorProvider : ScriptEditorProvider
        {
            public override IEnumerable<EditorFileType> FileTypes
            {
                get
                {
                    yield return new EditorFileType("Turtle Python Script", ".pyt");
                }
            }

            public override bool Handles(string path)
            {
                var extension = Path.GetExtension(path);
                return extension == ".pyt";
            }

            public override IDocument Create()
            {
                var script = new TurtleScriptViewModel();
                return script;
            }
            
        }
    }
}
