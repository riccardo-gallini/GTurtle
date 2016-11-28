using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;

namespace GTurtle
{
    [Export(typeof(IEditorProvider))]
    public class ScriptEditorProvider : IEditorProvider
    {
        public IEnumerable<EditorFileType> FileTypes
        {
            get
            {
                yield return new EditorFileType("GTurtle Python Script", ".py");
            }
        }

        public bool Handles(string path)
        {
            var extension = Path.GetExtension(path);
            return extension == ".py";
        }

        public IDocument Create()
        {
            return new ScriptEditorViewModel();
        }

        public async Task New(IDocument document, string name)
        {
            await ((ScriptEditorViewModel)document).New(name);
        }

        public async Task Open(IDocument document, string path)
        {
            await ((ScriptEditorViewModel)document).Load(path);
        }
    }
}