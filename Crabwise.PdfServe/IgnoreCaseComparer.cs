namespace Crabwise.PdfServe
{
    using System.Collections.Generic;

    internal class IgnoreCaseComparer : Comparer<string>
    {
        public override int Compare(string x, string y)
        {
            return string.Compare(x, y, true);
        }
    }
}
