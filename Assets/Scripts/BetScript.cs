using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BetScript : MonoBehaviour
{
    public TextMeshProUGUI betAmountText;
    public float betAmount = 0;
    public bool betting = true;
    public List<GameObject> betChips = new List<GameObject>();

    //sets the bet amount text
    public void SetBetAmountText()
    {
        betAmountText.text = "Betting $" + betAmount;
    }

    //destroyes the chips that have been bet, called when dealing cards starts.
    public void DeleteBetChips()
    {
        foreach (GameObject chip in betChips)
        {
            Destroy(chip);
        }
    }

    //when chip enters trigger and the game is in the betting state, adds the value of the bet chip to the bet amount.
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Chip" && betting)
        {
            betAmount += (float)other.GetComponentInParent<ChipScript>().value;
            SetBetAmountText();
            betChips.Add(other.transform.gameObject);
        }
    }

    //when chip leaves the trigger and the betting is on, removes the value of the bet chip from the bet amount.
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Chip" && betting)
        {
            betAmount -= (float)other.GetComponentInParent<ChipScript>().value;
            SetBetAmountText();
            betChips.Remove(other.transform.gameObject);
        }
    }
}
