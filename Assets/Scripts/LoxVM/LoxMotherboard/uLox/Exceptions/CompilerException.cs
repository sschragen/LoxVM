
namespace ULox
{
    public class CompilerException : UloxException
    {
        private readonly string extension;
        public string Extension { get { return extension; } }

        public CompilerException(string msg, string extension)
            : base(msg)
        {
            this.extension = extension;
        }
    }
}
