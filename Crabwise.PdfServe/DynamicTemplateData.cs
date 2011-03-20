namespace Crabwise.PdfServe
{
    using System.Dynamic;
    using System.Collections.Generic;

    class DynamicTemplateData : DynamicObject
    {
        private readonly IDictionary<string, object> dictionaryData;

        public DynamicTemplateData(IDictionary<string, object> dictionaryData)
        {
            this.dictionaryData = dictionaryData;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return this.dictionaryData.TryGetValue(binder.Name, out result);
        }
    }
}
