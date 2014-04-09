using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GLGUI.Example
{
    public class LineWriter : TextWriter
    {
        public List<string> Lines;
        public bool Changed = false;
        private StringBuilder currentLine;

        public LineWriter()
        {
            this.Lines = new List<string>(1024);
            this.currentLine = new StringBuilder(256);
        }

        public override void Write(char value)
        {
            if (value == '\n')
                Flush();
            else
                currentLine.Append(value);
        }

        public override void Flush()
        {
            Lines.Add(currentLine.ToString());
            if (Lines.Count > 1024)
                Lines.RemoveAt(0);
            currentLine.Clear();
            Changed = true;
        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.UTF8; }
        }

        public void Clear()
        {
            Lines.Clear();
            currentLine.Clear();
            Changed = true;
        }
    }
}
