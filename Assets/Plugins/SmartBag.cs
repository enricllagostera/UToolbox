/* Enric Llagostera <http://enric.llagostera.com.br> */

using System.Collections.Generic;

namespace UToolbox.SmartBagSystem
{
    public class SmartBag
    {
        #region Class members

        private List<ConditionedItem> _items;
        private List<Condition> _state;

        #endregion

        #region Public fields and properties

        public List<ConditionedItem> Items
        {
            get { return _items; }
        }

        public List<Condition> State
        {
            get { return _state; }
        }

        #endregion

        #region Constructor

        public SmartBag(List<ConditionedItem> startingItems = null, List<Condition> initialState = null)
        {
            _items = new List<ConditionedItem>();
            if (startingItems != null)
            {
                _items.AddRange(startingItems);
            }
            _state = new List<Condition>();
            if (initialState != null)
            {
                _state.AddRange(initialState);
            }
        }

        #endregion

        #region Public methods

        public List<ConditionedItem> Filter(List<Condition> query)
        {
            var res = new List<ConditionedItem>();
            _items.ForEach(i =>
                {
                    if (Condition.CheckAll(i.Preconditions, query))
                    {
                        res.Add(i);
                    }
                });
            return res;
        }

        public void Tick()
        {
            _items.ForEach(i => i.Tick());
        }

        public void SetState(List<Condition> newState)
        {
            _state = newState;
        }

        public List<Condition> GetState()
        {
            return _state;
        }

        public void SetCondition(Condition newCondition)
        {
            var cond = _state.Find(c => c.Id == newCondition.Id);
            if (cond == null)
            {
                _state.Add(newCondition);
            }
            else
            {
                cond.Status = newCondition.Status;
            }
        }

        public Condition GetCondition(string id)
        {
            var cond = _state.Find(c => c.Id == id);
            if (cond != null)
            {
                return cond;
            }
            else
            {
                return null;
            }
        }

        #endregion
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
            if (preconditions == null)
            {
                _preconditions = new List<Condition>();
            }
            else
            {
                _preconditions = preconditions;    
            }
            if (effects == null)
            {
                _effects = new List<Condition>();
            }
            else
            {
                _effects = effects;    
            }
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
            set { _status = value; }
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
            if (state == null)
            {
                return false;
            }
            if (query == null)
            {
                return true;
            }
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
