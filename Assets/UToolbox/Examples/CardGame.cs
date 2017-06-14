/* Enric Llagostera <http://enric.llagostera.com.br> */

using UnityEngine;
using System.Collections.Generic;
using UToolbox.SmartBagSystem;

public class CardGame : MonoBehaviour
{
    private SmartBag<PlayingCard> _deck;
    private int _cardCount = 0;

    void Awake()
    {
        var cards = new List<PlayingCard>();
        for (int v = 0; v < 13; v++)
        {
            for (int s = 0; s < 4; s++)
            {
                cards.Add(new PlayingCard(v, (ESuit)s));
            }
        }
        _deck = new SmartBag<PlayingCard>(cards, null);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _cardCount++;
            print(_deck.UseRandom(null).Id + " : " + _cardCount);
            if (_cardCount == 52)
            {
                _deck.UnlockAll();
            }
        }
    }
}
