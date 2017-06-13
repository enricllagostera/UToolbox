/* Enric Llagostera <http://enric.llagostera.com.br> */

using System.Collections.Generic;
using UnityEngine;

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

        public ConditionedItem PickRandom(List<Condition> query = null)
        {
            // handle null query
            var qry = new List<Condition>();
            if (query != null)
            {
                qry.AddRange(query);
            }
            // get a specific item by id
            var res = Draw(qry);
            if (res == null)
            {
                return null;
            }
            // advance the whole bag timer
            Tick();
            // call its use method
            var stateChanges = res.Use();
            // apply stateChanges
            foreach (var change in stateChanges)
            {
                SetCondition(change);
            }
            // return item
            return res;
        }

        public ConditionedItem Pick(string id, bool forcePick = false)
        {
            // forcePick can be used to get specific items regardless of state
            if (forcePick)
            {
                var res = _items.Find(i => i.Id == id);
                if (res == null)
                {
                    Debug.LogWarning(id + " item not available. The id must be wrong.");
                    return null;
                }
                return res;
            }
            // apply precondition filter based on current state
            var pool = FilterConditions(_state);
            if (pool.Count == 0)
            {
                Debug.LogWarning(id + " item not available in present conditions.");
                return null;
            }
            // get a specific item by id
            var pick = pool.Find(i => i.Id == id && i.IsLocked() == false);
            if (pick == null)
            {
                Debug.LogWarning(id + " item not available. Check its id and locks.");
                return null;
            }
            // advance the whole bag timer
            Tick();
            // call its use method
            var stateChanges = pick.Use();
            // apply stateChanges
            foreach (var change in stateChanges)
            {
                SetCondition(change);
            }
            // return item
            return pick;
        }

        public ConditionedItem Peek(List<Condition> preconditions)
        {
            return Draw(preconditions);
        }

        public List<ConditionedItem> FilterConditions(List<Condition> query)
        {
            // handle null query
            var qry = new List<Condition>();
            if (query != null)
            {
                qry.AddRange(query);
            }
            var res = new List<ConditionedItem>();
            _items.ForEach(i =>
                {
                    if (Condition.CheckAll(i.Preconditions, qry))
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
            if (newState == null)
            {
                Debug.LogWarning("The new State is null, so a new empty State will be created.");
                _state = new List<Condition>();
            }
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
                Debug.LogWarning("The " + id + " Condition does not exist in the bag's state.");
                return null;
            }
        }

        #endregion

        #region Private methods

        private ConditionedItem Draw(List<Condition> preconditions)
        {
            // handle null preconditions
            var conds = new List<Condition>();
            if (preconditions != null)
            {
                conds.AddRange(preconditions);
            }
            var pool = FilterConditions(conds);
            if (pool.Count == 0)
            {
                return null;
            }
            // filter for locked
            var notLocked = pool.FindAll(i => i.IsLocked() == false);
            if (notLocked.Count == 0)
            {
                return null;
            }
            // several options are available
            float[] weights = new float[notLocked.Count];
            float totalWeights = 0;
            for (int i = 0; i < notLocked.Count; i++)
            {
                totalWeights += notLocked[i].Weight;
                weights[i] = totalWeights;
            }
            // Debug.Log("W: " + weights.ToString());
            var roll = Random.Range(0f, totalWeights);
            // Debug.Log("R: " + roll);
            for (int w = 0; w < weights.Length; w++)
            {
                // got to the propoer weighted index
                if (weights[w] >= roll)
                {
                    // Debug.Log(notLocked[w].Id);
                    return notLocked[w];
                }
            }
            return null;
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

        public float Weight
        {
            get { return _weight; }
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

        public ConditionedItem(string id, float weight, int lockedInterval, List<Condition> preconditions = null, List<Condition> effects = null)
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
            if (newCondition == null)
            {
                Debug.LogError("Could not create new Condition.");
            }
            _id = newCondition.Id;
            _status = newCondition.Status;
        }

        #endregion

        #region Static methods

        public static Condition Parse(string description)
        {
            // error cases: too short or has whitespace
            if (description.Length < 2 || description.IndexOf(" ") >= 0)
            {
                Debug.LogError("Description cannot be parsed. Too short or has whitespace.");
                return null;
            }

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
            // cleanup
            if (current == null || query == null)
            {
                Debug.LogError("Cannot compare null Conditions.");
                return false;
            }
            return (current.Id == query.Id) && (current.Status == query.Status);
        }

        public static bool CheckAll(List<Condition> reqs, List<Condition> state)
        {
            // cleanup
            if (reqs == null)
            {
                reqs = new List<Condition>();
            }
            if (state == null)
            {
                state = new List<Condition>();
            }
            // processing
            var matches = new List<Condition>();
            state.ForEach(s =>
                {
                    var satisfiedCond = reqs.Find(r => Check(r, s));
                    if (satisfiedCond != null)
                    {
                        matches.Add(satisfiedCond);
                    }
                });
            if (matches.Count == reqs.Count)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
