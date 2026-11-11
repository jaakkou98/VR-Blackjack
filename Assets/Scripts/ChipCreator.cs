using System.Collections.Generic;
using UnityEngine;

public class ChipCreator : MonoBehaviour
{
    //dictionary that keeps track of all chips player has, with the keys being the values the chips can have.
    public Dictionary<float,List<GameObject>> chipDictionary = new Dictionary<float, List<GameObject>>();
    //chip positions is an array of empties that are used to determine the transform of the chips when they spawn
    public GameObject[] chipPositions;
    public GameObject prefab;
    public float[] chipValuesArray = {5000,1000,500,100,50,25,20,10,5,2.5f,1};

    void Start()
    {
        foreach (float value in chipValuesArray)
        {
            chipDictionary[value] = new List<GameObject>();
        }
    }
    
    //removes chip from dicionary
    public void RemoveChip(GameObject chip, float value)
    {
        //check if chip is in dictionary and if it is, remove it from it.
        if(chipDictionary[value].Contains(chip))
        {
            chipDictionary[value].Remove(chip);
        }
    }

    //calls to create the chips
    public void CreateChips()
    {
        foreach (float value in chipValuesArray)
        {  
            SpawnChip(value,6);
        }
    }

    //adds chips when player wins money
    public void AddChips(float money)
    {
        float moneyLeft = money;
        for (int i = 0; i < chipValuesArray.Length; i++)
        {
            moneyLeft = ShouldSpawnChips(moneyLeft,chipValuesArray[i],i);
        }
    }

    //checks if chips should be spawned
    private float ShouldSpawnChips(float money, float value, int index)
    {
        float moneyLeft = money;
        float remainder = moneyLeft-value;
        bool spawn = true;
        while(spawn)
        {
            if(moneyLeft>=value)
            {
                if(remainder != 0)
                {
                    for (int i = index+1; i < chipValuesArray.Length; i++)
                    {
                        if(remainder-chipValuesArray[i]>=1 || remainder-chipValuesArray[i] == 0)
                        {
                            spawn = true;
                            break;
                        }
                        else
                        {
                            spawn = false;
                        }
                    }
                }
                if(spawn)
                {
                    SpawnChip(value,1);
                    moneyLeft-=value;
                    remainder-=value;
                }
            }
            else
            {
                spawn = false;
            }
        }
        return moneyLeft;
    }

    //spawns chips
    public void SpawnChip(float value, int amountToSpawn)
    {
        int index = 0;
        switch (value)
        {
            case 1:
                index = 0;
                break;
            case 2.5f:
                index = 1;
                break;
            case 5:
                index = 2;
                break;
            case 10:
                index = 3;
                break;
            case 20:
                index = 4;
                break;
            case 25:
                index = 5;
                break;
            case 50:
                index = 6;
                break;
            case 100:
                index = 7;
                break;
            case 500:
                index = 8;
                break;
            case 1000:
                index = 9;
                break;
            case 5000:
                index = 10;
                break;
        }
        int totalAmount = amountToSpawn+chipDictionary[value].Count;
        int totalIndex = chipDictionary[value].Count;
        float offset = 0.003f*totalIndex;
        for(int i = totalIndex; i<totalAmount; i++)
        {
            chipDictionary[value].Add(Instantiate
            (
                prefab,
                new Vector3(0,0,0),
                Quaternion.identity
            ));
            chipDictionary[value][i].transform.Rotate(Vector3.left,90);
            chipDictionary[value][i].transform.position = new Vector3(chipPositions[index].transform.position.x,chipPositions[index].transform.position.y+offset,chipPositions[index].transform.position.z);
            chipDictionary[value][i].GetComponent<ChipScript>().value = value;
            chipDictionary[value][i].GetComponent<ChipScript>().SetColorAndValue();
            offset += 0.003f;
        }
    }
}
