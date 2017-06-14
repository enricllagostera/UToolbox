/* Enric Llagostera <http://enric.llagostera.com.br> */

using UnityEngine;
using UToolbox.SmartBagSystem;

public enum ESuit
{
    HEARTS,
    DIAMONDS,
    CLUBS,
    SPADES
}

public class PlayingCard : ConditionedItem
{
    #region Public fields

    public ESuit Suit
    {
        get;
        set;
    }

    public int Value
    {
        get;
        set;
    }

    #endregion

    #region Class members

    private ESuit _suit;
    private int _value;

    #endregion

    public PlayingCard(int value, ESuit suit)
        : base(suit.ToString() + value, 1, 51, null, null)
    {
        
    }

    #region Private methods

    #endregion
}
