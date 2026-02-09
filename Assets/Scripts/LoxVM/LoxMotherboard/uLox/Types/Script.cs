namespace ULox
{
    public class Script
    {
        public readonly string Name;
        public readonly string Ext;
        public readonly string Source;
        public readonly int ScriptHash;

        public Script(string name, string source,string ext)
        {
            Name = name;
            Source = source;
            Ext = ext;
            ScriptHash = source.GetHashCode();  //todo no good, use a stable one
        }
    }
}