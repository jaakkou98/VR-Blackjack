using System.Collections.Generic;
using UnityEngine;

public class DeckScript : MonoBehaviour
{
    public List<GameObject> cards = new List<GameObject>();
    public Sprite[] sprites;
    public GameObject prefab;
    public GameObject deck;

    void Start()
    {
        for(var i = 0; i<52; i++)
        {
            cards.Add(Instantiate
            (
                prefab,
                new Vector3(0,0,0),
                Quaternion.identity
            ));
            SetCardTexture(cards[i],sprites[i]);
            SetCardValue(cards[i],i);
        }
        ShuffleDeck();
    }

    //shuffles deck. Basically sets the transforms at random for all cards, regardless of where the cards are in the game world.
    public void ShuffleDeck()
    {
        float offset = 0;
        for(int i = 0; i<cards.Count; i++)
        {
            System.Random r = new System.Random();
            int randomInt = r.Next(i,cards.Count);
            GameObject temp = cards[i];
            cards[i] = cards[randomInt];
            cards[randomInt] = temp;
            cards[i].transform.parent = deck.transform;
            cards[i].transform.rotation = Quaternion.identity;
            cards[i].GetComponent<CardVariables>().SetKinematic(true);
            cards[i].transform.position = new Vector3(deck.transform.position.x, deck.transform.position.y+offset, deck.transform.position.z);
            cards[i].transform.Rotate(Vector3.left,90,Space.Self);
            offset += prefab.transform.localScale.z;
        }
    }
    
    //sets the value of the card according to the index of the sprite.
    public void SetCardValue(GameObject card, int index)
    {
        int value = 0;
        int number = 0;
        if(index == 0 || index % 13 == 0)
        {
            value = 11;
            number = 1;
        }
        else if(index == 1 || index % 13 == 1)
        {
            value = 2;
            number = 2;
        }
        else if(index == 2 || index % 13 == 2)
        {
            value = 3;
            number = 3;
        }
        else if(index == 3 || index % 13 == 3)
        {
            value = 4;
            number = 4;
        }
        else if(index == 4 || index % 13 == 4)
        {
            value = 5;
            number = 5;
        }
        else if(index == 5 || index % 13 == 5)
        {
            value = 6;
            number = 6;
        }
        else if(index == 6 || index % 13 == 6)
        {
            value = 7;
            number = 7;
        }
        else if(index == 7 || index % 13 == 7)
        {
            value = 8;
            number = 8;
        }
        else if(index == 8 || index % 13 == 8)
        {
            value = 9;
            number = 9;
        }
        else if(index == 9  || index % 13 == 9)
        {
            value = 10;
            number = 10;
        }
        else if(index == 10  || index % 13 == 10)
        {
            value = 10;
            number = 11;
        }
        else if(index == 11  || index % 13 == 11)
        {
            value = 10;
            number = 12;
        }
        else if(index == 12  || index % 13 == 12)
        {
            value = 10;
            number = 13;
        }
        card.GetComponent<CardVariables>().value = value;
        card.GetComponent<CardVariables>().number = number;
    }

    //calls the cardvariables script to set the right sprite for the material.
    public void SetCardTexture(GameObject card, Sprite sprite)
    {
        card.GetComponent<CardVariables>().SetMaterial(sprite);
    }
}
