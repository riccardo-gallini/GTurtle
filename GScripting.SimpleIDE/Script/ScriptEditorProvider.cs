using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace GScripting.SimpleIDE
{
    [Export(typeof(IEditorProvider))]
    public class ScriptEditorProvider : IEditorProvider
    {
        public virtual IEnumerable<EditorFileType> FileTypes
        {
            get
            {
                yield return new EditorFileType("Python Script", ".py");
            }
        }

        public virtual bool Handles(string path)
        {
            var extension = Path.GetExtension(path);
            return extension == ".py";
        }

        public virtual IDocument Create()
        {
            return new ScriptViewModel();
        }

        public virtual async Task New(IDocument document, string name)
        {
            await ((ScriptViewModel)document).New(name);
        }

        public virtual async Task Open(IDocument document, string path)
        {
            await ((ScriptViewModel)document).Load(path);
        }
    }
}