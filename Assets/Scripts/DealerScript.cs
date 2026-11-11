using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DealerScript : MonoBehaviour
{
    public DeckScript dc;
    //dealer and player card positions are empties that pinpoint the transform for the cards that are dealt
    public GameObject dealerCardPosition;
    public GameObject playerCardPosition;
    //pinpoints the position where played cards are stacked
    public GameObject playedCardsPile;
    //offset between spawn cards
    public float offset;
    //values of the cards of each dealer and player
    public TextMeshProUGUI dealerValueText;
    public TextMeshProUGUI playerValueText;
    public TextMeshProUGUI hitButtonText;
    //used to determine which card is currently on top of the deck.
    private static int deckTopIndex;
    //dealer and player values are lists of the values of the cards the player and dealer have on their hands.
    private List<int> dealerValues = new List<int>();
    private List<int> playerValues = new List<int>();
    //checks if the first deal of the game is done.
    private bool firstDealDone = false;
    //instance of the dealer card that is face down.
    private GameObject hiddenDealerCard;
    //list of the card gameobjects that have already been played.
    private List<GameObject> playedCards = new List<GameObject>();
    //used to determine if the game has ended.
    private bool gameEnded;
    //deck has been shuffled at the start of the game.
    private bool shuffled = true;
    public TextMeshProUGUI playButtonText;
    public TextMeshProUGUI betText;
    //the bet isn't done at the start of the game.
    private bool betDone = false;
    public Button playButton;
    public Button hitButton;
    public Button standButton;
    public PlayerVariables playerVariables;
    public BetScript betScript;
    private float bet;
    //at the start of the game, it's the start of the game.
    private bool startOfGame = true;
    //dealer animator instance.
    public Animator animator;
    public Button splitButton;
    //the player numbers are separate from the player values because of the way blackjack works. Could have made them the same thing, but since I made the blackjack check later, decided to just have them separate.
    private List<int> playerNumbers = new List<int>();
    public GameObject splitCardPosition;
    public TextMeshProUGUI splitValueText;
    //values of the split hand's cards.
    private List<int> splitValues = new List<int>();
    //the player cards are listed because of the splitting function transforms, and so are the split cards.
    private List<GameObject> playerCards = new List<GameObject>();
    private List<GameObject> splitCards = new List<GameObject>();
    //are the cards split or not? By default they are not.
    private bool cardsSplit = false;
    //have the split hands been swapped or not?
    private bool splitCardsSwapped = false;
    //in case of double blackjack, sets tempBlackjack true
    private bool tempBlackjack = false;
    public Button doubleButton;
    //is the player doubling the bet or not?
    private bool doubling = false;
    public Button insuranceButton;
    //is the dealer paying insurance or not?
    private bool payingInsurance = false;

    //called by the play button to start the game
    public void Play()
    {
        if(startOfGame)
        {
            playerVariables.chipCreator.CreateChips();
            startOfGame = false;
        }
        if(!betDone)
        {
            betDone = true;
            playButtonText.text = "Deal";
            betText.text = "Place Your Bets!";
            betScript.SetBetAmountText();
            animator.SetTrigger("DoGesture");
        }
        else
        {
            playButton.interactable = false;
            playButton.image.enabled = false;
            playButtonText.text = "";
            playButton.gameObject.SetActive(false);
            bet = betScript.betAmount;
            betScript.betting = false;
            hitButtonText.text = "Hit";
            betScript.DeleteBetChips();
            betText.text = betScript.betAmountText.text;
            betScript.betAmountText.text = "";
            playerVariables.gain -= betScript.betAmount;
            animator.SetTrigger("DealCards");
            Deal();
        }
    }

    //sets up the betting functionalities
    public void ActivateBetting()
    {
        betDone = false;
        playButton.interactable = true;
        playButton.image.enabled = true;
        playButton.gameObject.SetActive(true);
        betScript.betting = true;
        hitButton.interactable = false;
        betScript.betAmount = 0;
        Play();
    }

    //called when insurance button pressed and handles the insurance
    public void Insure()
    {
        standButton.interactable = false;
        hitButton.interactable = false;
        splitButton.interactable = false;
        doubleButton.interactable = false;
        insuranceButton.interactable = false;
        insuranceButton.gameObject.SetActive(false);
        playerVariables.RemoveAmount(Mathf.Floor(bet*2)/2);
        StartCoroutine(CheckInsurance());
    }

    //checks if dealer has blackjack
    private IEnumerator CheckInsurance()
    {
        if(dealerValues[0] != 10)
        {
            betText.text = "Nobody home!";
            yield return new WaitForSeconds(1);
            betText.text = "Betting $" + bet;
            hitButton.interactable = true;
            standButton.interactable = true;
            doubleButton.interactable = true;
            splitButton.interactable = true;
        }
        else
        {
            payingInsurance = true;
            DealerTurn(false,false);
        }
    }

    //called by double button
    public void Double()
    {
        doubling = true;
        playerVariables.RemoveAmount(bet);
        bet *= 2;
        betText.text = "Betting $" + bet;
        Hit();
    }
    
    //called by split button
    public void Split()
    {
        cardsSplit = true;
        splitValues.Add(playerValues[1]);
        playerValues.RemoveAt(1);
        playerNumbers.RemoveAt(1);
        splitCards.Add(playerCards[1]);
        playerCards.RemoveAt(1);
        splitCards[0].transform.position = splitCardPosition.transform.position;
        DisplayCardValues(false);
        //because of the way winnings are calculated with split, the bet is split as well, so the text is changed to the new bet, but the variable isn't changed
        betText.text = "Betting $" + bet*2;
        playerVariables.RemoveAmount(bet);
        StartCoroutine(DealSplitCards());
    }

    //when split, deals the split cards accordingly. These deal functions are done this way with coroutines because of the animator triggers.
    private IEnumerator DealSplitCards()
    {
        animator.SetTrigger("DealCards");
        hitButton.interactable = false;
        standButton.interactable = false;
        splitButton.interactable = false;
        splitButton.gameObject.SetActive(false);
        doubleButton.interactable = false;
        doubleButton.gameObject.SetActive(false);
        yield return new WaitForSeconds(1.7f);
        DealCard(new Vector3(playerCardPosition.transform.position.x-offset, playerCardPosition.transform.position.y, playerCardPosition.transform.position.z),true,true,false);
        DisplayCardValues(false);
        animator.SetTrigger("DealCards");
        yield return new WaitForSeconds(0.7f);
        DealCard(new Vector3(splitCardPosition.transform.position.x-offset, splitCardPosition.transform.position.y, splitCardPosition.transform.position.z),true,true,true);
        DisplayCardValues(false);
        if(int.Parse(playerValueText.text)==21)
        {
            tempBlackjack = true;
            SwapWithSplitCards(true);
        }
        else
        {
            hitButton.interactable = true;
            standButton.interactable = true;
        }
    }

    //handles the functionalities for swapping the split hand with the other hand after one hand has been stood.
    private void SwapWithSplitCards(bool blackjack)
    {
        if(splitCardsSwapped)
        {
            DealerTurn(blackjack,tempBlackjack);
        }
        else
        {
            float localOffset = 0;
            foreach (GameObject card in splitCards)
            {
                card.transform.position = new Vector3(playerCardPosition.transform.position.x-localOffset, playerCardPosition.transform.position.y, playerCardPosition.transform.position.z);
                localOffset += offset;
            }
            localOffset = 0;
            foreach (GameObject card in playerCards)
            {
                card.transform.position = new Vector3(splitCardPosition.transform.position.x-localOffset, splitCardPosition.transform.position.y, splitCardPosition.transform.position.z);
                localOffset += offset;
            }
            List<int> tempList = playerValues;
            playerValues = splitValues;
            splitValues = tempList;
            splitCardsSwapped = true;
            DisplayCardValues(false);
            if(int.Parse(playerValueText.text)==21)
            {
                SwapWithSplitCards(true);
            }
            else
            {
                hitButton.interactable = true;
                standButton.interactable = true;
            }
        }
    }

    //adds the winnings to player
    private void AddWinningsToPlayerMoney(float winnings)
    {
        //rounds down so that in case of decimal blackjack win, the money stays realistic.
        playerVariables.AddWinnings(Mathf.Floor(winnings*2)/2);
    }

    //calculates the winnings based on the outcome and displays text of the outcome.
    public void CalculateWinnings(bool playerBlackjack,bool dealerBlackjack,bool splitBlackjack)
    {
        string outcome = "";
        //if paying insurance, get bet back.
        if(payingInsurance)
        {
            outcome = "INSURANCE PAID";
            AddWinningsToPlayerMoney(bet);
        }
        //if player wins by blackjack, gets 2.5* bet amount of money
        else if(playerBlackjack && !dealerBlackjack)
        {
            outcome = "BLACKJACK";
            AddWinningsToPlayerMoney(bet*2.5f);
        }
        //if dealer has blackjack and player doesn't, lose
        else if(!playerBlackjack && dealerBlackjack)
        {
            outcome = "DEALER BLACKJACK";
        }
        //if both have blackjack, push (tie). Player gets money back.
        else if(playerBlackjack && dealerBlackjack)
        {
            outcome = "PUSH";
            AddWinningsToPlayerMoney(bet);
        }
        //if player goes bust, loses
        else if(int.Parse(playerValueText.text)>21)
        {
            outcome = "BUST";
        }
        //if player doesn't go bust and dealer does, gets 2* bet amount of money
        else if(int.Parse(dealerValueText.text)>21)
        {
            outcome = "VICTORY";
            AddWinningsToPlayerMoney(bet*2);
        }
        //if neither goes bust and player has higher value of cards than dealer, gets 2* bet amount of money
        else if(int.Parse(playerValueText.text)>int.Parse(dealerValueText.text))
        {
            outcome = "VICTORY";
            AddWinningsToPlayerMoney(bet*2);
        }
        //if neither goes bust and player and dealer have equal value of cards, it's a push (tie). Player gets bet money back.
        else if(int.Parse(playerValueText.text)==int.Parse(dealerValueText.text))
        {
            outcome = "PUSH";
            AddWinningsToPlayerMoney(bet);
        }
        //otherwise player loses
        else
        {
            outcome = "LOSS";
        }
        //if cards are split, there's another check to see if player beats dealer
        if(cardsSplit)
        {
            if(splitBlackjack && !dealerBlackjack)
            {
                if(outcome == "BLACKJACK")
                {
                    outcome = "BLACKJACK x2";
                }
                else
                {
                    outcome += " and BLACKJACK";
                }
                AddWinningsToPlayerMoney(bet*2.5f);
            }
            //if dealer has blackjack and player doesn't, lose
            else if(!splitBlackjack && dealerBlackjack)
            {
                outcome = "DEALER BLACKJACK";
            }
            //if both have blackjack, push (tie). Player gets money back.
            else if(playerBlackjack && dealerBlackjack)
            {
                if(outcome == "PUSH")
                {
                    outcome = "PUSH x2";
                }
                else
                {
                    outcome += " and PUSH";
                }
                AddWinningsToPlayerMoney(bet);
            }
            //if player goes bust, loses
            else if(int.Parse(splitValueText.text)>21)
            {
                if(outcome == "BUST")
                {
                    outcome = "BUST x2";
                }
                else
                {
                    outcome += " and BUST";
                }
            }
            //if player doesn't go bust and dealer does, gets 2* bet amount of money
            else if(int.Parse(dealerValueText.text)>21)
            {
                if(outcome == "VICTORY")
                {
                    outcome = "VICTORY x2";
                }
                else
                {
                    outcome += " and VICTORY";
                }
                AddWinningsToPlayerMoney(bet*2);
            }
            //if neither goes bust and player has higher value of cards than dealer, gets 2* bet amount of money
            else if(int.Parse(splitValueText.text)>int.Parse(dealerValueText.text))
            {
                if(outcome == "VICTORY")
                {
                    outcome = "VICTORY x2";
                }
                else
                {
                    outcome += " and VICTORY";
                }
                AddWinningsToPlayerMoney(bet*2);
            }
            //if neither goes bust and player and dealer have equal value of cards, it's a push (tie). Player gets bet money back.
            else if(int.Parse(splitValueText.text)==int.Parse(dealerValueText.text))
            {
                if(outcome == "PUSH")
                {
                    outcome = "PUSH x2";
                }
                else
                {
                    outcome += " and PUSH";
                }
                AddWinningsToPlayerMoney(bet);
            }
            //otherwise player loses
            else
            {
                if(outcome == "LOSS")
                {
                    outcome = "LOSS x2";
                }
                else
                {
                    outcome += " and LOSS";
                }
            }
        }
        betText.text = outcome + "!";
    }

    //called by the hit button. Handles game ending and dealing the first cards, and also calls hit function. Used to be bigger before other functions were separated from this, but now more of a residue from the beginning.
    public void Deal()
    {
        if(gameEnded)
        {
            EndShoe();
            ActivateBetting();
        }
        else if(firstDealDone)
        {
            Hit();
        }
        else
        {
            //if deck has been shuffled, reset deckTopIndex to accurately represent top of deck index.
            if(shuffled)
            {
                deckTopIndex = dc.cards.Count-1;
            }
            StartCoroutine(DealFirstCards());
        }
    }

    //deal first four cards with the first dealer card not revealed
    private IEnumerator DealFirstCards()
    {
        //first player card
        yield return new WaitForSeconds(1.9f);
        DealCard(playerCardPosition.transform.position,true,true,false);
        DisplayCardValues(false);
        //first dealer card, face down
        yield return new WaitForSeconds(0.8f);
        DealCard(dealerCardPosition.transform.position,false,false,false);
        //second player card
        yield return new WaitForSeconds(0.8f);
        DealCard(new Vector3(playerCardPosition.transform.position.x-offset, playerCardPosition.transform.position.y, playerCardPosition.transform.position.z),true,true,false);
        DisplayCardValues(false);
        //second dealer card, face up
        yield return new WaitForSeconds(0.8f);
        DealCard(new Vector3(dealerCardPosition.transform.position.x+offset, dealerCardPosition.transform.position.y, dealerCardPosition.transform.position.z),true,false,false);
        DisplayCardValues(false);
        firstDealDone = true;
        shuffled = false;
        //if player gets blackjack, move on to dealer turn automatically.
        if(int.Parse(playerValueText.text)==21)
        {
            DealerTurn(true,false);
        }
        else
        {
            standButton.interactable = true;
            hitButton.interactable = true;
            if(playerVariables.CheckIfCanDouble(bet))
            {
                doubleButton.gameObject.SetActive(true);
                doubleButton.interactable = true;
                if(playerNumbers[0] == playerNumbers[1])
                {
                    splitButton.gameObject.SetActive(true);
                    splitButton.interactable = true;
                }
            }
            if(dealerValues[1] == 11)
            {
                insuranceButton.gameObject.SetActive(true);
                insuranceButton.interactable = true;
            }
        }
    }

    //deals cards with the animation trigger wait times.
    private IEnumerator DealCardWithDelay(Vector3 position, bool showCard, bool isPlayer)
    {
        insuranceButton.interactable = false;
        insuranceButton.gameObject.SetActive(false);
        splitButton.interactable = false;
        splitButton.gameObject.SetActive(false);
        doubleButton.interactable = false;
        doubleButton.gameObject.SetActive(false);
        hitButton.interactable = false;
        standButton.interactable = false;
        yield return new WaitForSeconds(1.8f);
        DealCard(position,showCard,isPlayer,false);
        DisplayCardValues(false);
        if(int.Parse(playerValueText.text)>=21)
        {
            if(cardsSplit)
            {
                SwapWithSplitCards(false);
                hitButton.interactable = true;
                standButton.interactable = true;
            }
            else
            {
                DealerTurn(false,false);
            }
        }
        else
        {
            if(doubling)
            {
                DealerTurn(false,false);
            }
            else
            {
                hitButton.interactable = true;
                standButton.interactable = true;
            }
        }
    }

    //dealer turn with the animation triggers for the dealer.
    private IEnumerator DealerTurnWithDelay(bool playerBlackjack,bool splitBlackjack)
    {
        insuranceButton.interactable = false;
        insuranceButton.gameObject.SetActive(false);
        splitButton.interactable = false;
        splitButton.gameObject.SetActive(false);
        doubleButton.interactable = false;
        doubleButton.gameObject.SetActive(false);
        hitButton.interactable = false;
        standButton.interactable = false;
        animator.SetTrigger("FlipDealerCard");
        yield return new WaitForSeconds(0.4f);
        RevealDealerCard();
        DisplayCardValues(true);
        bool dealerBlackjack = false;
        //check if dealer has blackjack.
        if(int.Parse(dealerValueText.text) == 21)
        {
            dealerBlackjack = true;
        }
        else
        {   
            //dealer continues to draw cards until the value of dealer cards is at least 17
            while(int.Parse(dealerValueText.text) < 17)
            {
                animator.SetTrigger("DealCards");
                yield return new WaitForSeconds(0.7f);
                float localOffset = offset*dealerValues.Count;
                DealCard(new Vector3(dealerCardPosition.transform.position.x+localOffset, dealerCardPosition.transform.position.y, dealerCardPosition.transform.position.z),true,false,false);
                DisplayCardValues(true);
            }
        }
        yield return new WaitForSeconds(1);
        animator.SetTrigger("GameEnded");
        gameEnded = true;
        hitButton.interactable = true;
        hitButtonText.text = "Reset";
        CalculateWinnings(playerBlackjack,dealerBlackjack,splitBlackjack);
    }

    //starts coroutine for dealer turn for the animations. Used to handle the whole dealer turn before the animations became a thing.
    private void DealerTurn(bool playerBlackjack,bool splitBlackjack)
    {
        StartCoroutine(DealerTurnWithDelay(playerBlackjack,splitBlackjack));
    }

    //calls the functions for when player takes the hit action and starts the dealer dealing one card.
    public void Hit()
    {
        float localOffset = offset*playerValues.Count;
        animator.SetTrigger("DealCards");
        StartCoroutine(DealCardWithDelay(new Vector3(playerCardPosition.transform.position.x-localOffset, playerCardPosition.transform.position.y, playerCardPosition.transform.position.z),true,true));
    }

    //called by the stand button.
    public void Stand()
    {
        if(cardsSplit)
        {
            SwapWithSplitCards(false);
        }
        else
        {
            DealerTurn(false,false);
        }
    }

    //ends the shou, which basically means it ends the hand. Sets up things for the next shoe.
    private void EndShoe()
    {
        hitButtonText.text = "Deal";
        float localOffset = playedCards[0].transform.localScale.z*playedCards.Count;
        foreach (GameObject card in playedCards)
        {
            card.GetComponent<CardVariables>().SetKinematic(true);
            card.transform.position = new Vector3(playedCardsPile.transform.position.x, playedCardsPile.transform.position.y+localOffset, playedCardsPile.transform.position.z);
            localOffset += card.transform.localScale.z;
        }
        firstDealDone = false;
        if(deckTopIndex<14)
        {
            playedCards.Clear();
            dc.ShuffleDeck();
            shuffled = true;
        }
        gameEnded = false;
        playerValueText.text = "";
        dealerValueText.text = "";
        playerValues.Clear();
        dealerValues.Clear();
        playerNumbers.Clear();
        splitCards.Clear();
        playerCards.Clear();
        splitValues.Clear();
        splitValueText.text = "";
        cardsSplit = false;
        splitCardsSwapped = false;
        tempBlackjack = false;
        doubling = false;
        payingInsurance = false;
        playerVariables.SetGainText();
    }

    //rotates the dealer card that is face down to be shown.
    private void RevealDealerCard()
    {
        hiddenDealerCard.transform.Rotate(Vector3.left, 180);
        DisplayCardValues(true);
    }

    //handles the transform of the cards that are dealt from the deck.
    private void DealCard(Vector3 pos,bool showCard,bool isPlayer,bool isSplit)
    {
        GameObject card = dc.cards[deckTopIndex];
        card.transform.position = pos;
        deckTopIndex--;
        card.GetComponent<CardVariables>().DoWhenRemoveFromDeck();
        playedCards.Add(card);
        if(showCard)
        {
            card.transform.Rotate(Vector3.left, 180);
        }
        else
        {
            hiddenDealerCard = card;
        }
        if(isPlayer && !isSplit)
        {
            playerValues.Add(card.GetComponent<CardVariables>().value);
            playerNumbers.Add(card.GetComponent<CardVariables>().number);
            playerCards.Add(card);
        }
        else if(!isPlayer)
        {
            dealerValues.Add(card.GetComponent<CardVariables>().value);
        }
        else if(isSplit)
        {
            splitValues.Add(card.GetComponent<CardVariables>().value);
            splitCards.Add(card);
        }
    }

    //displays card values on the player, dealer and split value texts.
    private void DisplayCardValues(bool dealerCardsRevealed)
    {
        playerValueText.text = CountCardValues(playerValues).ToString();
        //if dealer has the cards revealed, calculate card values the same way the player cards are calculated
        if(dealerCardsRevealed)
        {
            dealerValueText.text = CountCardValues(dealerValues).ToString();
        }
        else if(dealerValues.Count>1)
        {
            //since dealerValue list's index 1 card is always the one revealed, it is the only value displayed if all cards aren't revealed
            dealerValueText.text = dealerValues[1].ToString();
        }
        if(cardsSplit)
        {
            splitValueText.text = CountCardValues(splitValues).ToString();
        }
    }

    //counts the card values.
    private int CountCardValues(List<int> cardValues)
    {
        int cardValue = 0;
        foreach (int value in cardValues)
        {
            cardValue += value;
        }
        //in Blackjack, the value of Ace is 11, unless the hand goes over 21, in which case the first ace is valued 1 and the others 11.
        //checks to make sure that 10 is substracted from total value if there is any aces in the hand, making the total value equal to 1 ace being valued 1.
        if(cardValue > 21 && cardValues.Contains(11))
        {
            cardValue -= 10;
        }
        return cardValue;
    }
}
