/* Enric Llagostera <http://enric.llagostera.com.br> */

using System.Collections.Generic;

namespace UToolbox.SmartBag
{
    /* 
     * 
     */

    public class Condition
    {
        #region Constructor

        public Condition(string id, bool status)
        {
            _id = id;
            _status = status;
        }

        public Condition(string description)
        {
            var newCondition = Parse(description);
            _id = newCondition.Id;
            _status = newCondition.Status;
        }

        #endregion

        #region Public fields

        public string Id
        {
            get { return _id; }
        }

        public bool Status
        {
            get { return _status; }
        }

        #endregion

        #region Class members

        private string _id;
        private bool _status;

        #endregion

        #region Private methods

        #endregion

        #region Static methods

        public static Condition Parse(string description)
        {
            string id = description;
            bool status = true;

            // is a false value
            if (description.IndexOf("!") == 0)
            {
                id = description.Substring(1);
                status = false;
            }

            return new Condition(id, status);
        }

        public static bool Check(Condition current, Condition query)
        {
            return (current.Id == query.Id) && (current.Status == query.Status);
        }

        public static bool CheckAll(List<Condition> state, List<Condition> query)
        {
            for (int i = 0; i < query.Count; i++)
            {
                var res = state.Find(c => c.Id == query[i].Id);
                if (res == null)
                {
                    return false;
                }
                if (!Check(res, query[i]))
                {
                    return false;
                }
            }
            return true;
        }

        #endregion
    }
}
