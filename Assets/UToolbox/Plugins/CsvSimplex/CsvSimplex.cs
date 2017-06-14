using Sinbad;
using System.Collections.Generic;

namespace UToolbox.CsvSimplexSystem
{
    public class CsvSimplex<T> where T : new() 
    {
        public CsvSimplex()
        {
        }

        public List<T> ReadList(string path){
            var res = new List<T>();
            res = CsvUtil.LoadObjects<T>(path);
            return res;
        }

        public void WriteList(List<T> list, string path){
            CsvUtil.SaveObjects(list, path);
        }
    }
}
