using TMPro;
using UnityEngine;

public class PlayerVariables : MonoBehaviour
{
    //gain of the money player has accumulated. can be losses as well.
    public float gain;
    public TextMeshProUGUI gainText;
    public float money;
    public TextMeshProUGUI playerMoneyText;
    public ChipCreator chipCreator;

    //sets the gain text to reflect current gain or losses
    public void SetGainText()
    {
        string text = "";
        if(gain>=0)
        {
            text = "Gain:\n";
        }
        else
        {
            text = "Loss:\n";
        }
        gainText.text = text + "$" + gain;
    }

    //sets up removing chips according to the amount
    public void RemoveAmount(float amount)
    {
        gain -= amount;
        float tempAmount = amount;
        for (int i = 0; i < chipCreator.chipValuesArray.Length; i++)
        {
            if(tempAmount>0)
            {
                tempAmount = ShouldDestroyChips(tempAmount,chipCreator.chipValuesArray[i],i);
            }
        }
    }

    //destroy chips of given value and return remaining amount
    private float ShouldDestroyChips(float amount, float value,int index)
    {
        float tempAmount = amount;
        float remainder = tempAmount-value;
        bool destroy = true;
        while(destroy)
        {
            if(tempAmount>=value)
            {
                if(remainder != 0)
                {
                    for (int i = index+1; i < chipCreator.chipValuesArray.Length; i++)
                    {
                        if(remainder-chipCreator.chipValuesArray[i]>=1 || remainder-chipCreator.chipValuesArray[i] == 0)
                        {
                            destroy = true;
                            break;
                        }
                        else
                        {
                            destroy = false;
                        }
                    }
                }
                if(destroy)
                {
                    DestroyChip(value);
                    tempAmount-=value;
                    remainder-=value;
                }
            }
            else
            {
                destroy = false;
            }
        }
        return tempAmount;
    }

    //destroyes a chip of value
    private void DestroyChip(float value)
    {
        //not ideal, but due to lack of time, if player doesn't have the right chip, it just doesn't get destroyed.
        if(chipCreator.chipDictionary[value].Count>0)
        {
            GameObject chip = chipCreator.chipDictionary[value][^1];
            chip.transform.position = new Vector3(0,0,0);
            Destroy(chip);
        }
    }

    //checks if money can be doubled.
    public bool CheckIfCanDouble(float amountBet)
    {
        if(money>=amountBet)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //sets money text to include $
    public void SetMoneyText()
    {
        playerMoneyText.text = "$" + money;
    }

    //adds winnings to the gains and calls to add chips to player
    public void AddWinnings(float winnings)
    {
        chipCreator.AddChips(winnings);
        gain += winnings;
    }

    //when chip enters trigger, adds the value to money
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Chip")
        {
            money += (float)other.GetComponent<ChipScript>().value;
            SetMoneyText();
        }
    }

    //when chip leaves the trigger, removes the chip
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Chip")
        {
            money -= (float)other.GetComponent<ChipScript>().value;
            SetMoneyText();
            //remove chip from stack when it leaves the player chip zone.
            chipCreator.RemoveChip(other.transform.gameObject,(float)other.GetComponent<ChipScript>().value);
            other.transform.parent = null;
        }
    }
}
