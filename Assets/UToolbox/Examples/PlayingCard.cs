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

    private ESuit _suit;
    private int _value;

    public PlayingCard(int value, ESuit suit)
        : base(suit.ToString() + value, 1, 51, null, null)
    {
        
    }
}
