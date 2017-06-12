/* Enric Llagostera <http://enric.llagostera.com.br> */

using System.Collections.Generic;

namespace UToolbox.SmartBag
{
    public class SmartBag
    {
        
    }

    public abstract class ConditionedItem
    {
        #region Public fields and properties

        public string Id
        {
            get { return _id; }
        }

        public List<Condition> Preconditions
        {
            get { return _preconditions; }
        }

        public List<Condition> Effects
        {
            get { return _effects; }
        }

        #endregion

        #region Class members

        private string _id;
        private float _weight;
        private int _lockCounter;
        private int _lockedInterval;
        private List<Condition> _preconditions;
        private List<Condition> _effects;

        #endregion

        #region Constructors

        public ConditionedItem(string id, float weight, int lockedInterval, List<Condition> preconditions, List<Condition> effects)
        {
            _id = id;
            _weight = weight;
            _lockCounter = 0;
            _lockedInterval = lockedInterval;
            _preconditions = preconditions;
            _effects = effects;
        }

        #endregion

        #region Public methods

        public List<Condition> Use()
        {
            _lockCounter = _lockedInterval;
            return _effects;   
        }

        public void Tick()
        {
            _lockCounter--;    
            if (_lockCounter < 0)
            {
                _lockCounter = 0;
            }
        }

        public bool IsLocked()
        {
            return (_lockCounter > 0);
        }

        #endregion
    }

    public class Condition
    {
        #region Class members

        private string _id;
        private bool _status;

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
