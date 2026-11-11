using TMPro;
using UnityEngine;

public class ChipScript : MonoBehaviour
{
    public double value;
    public MeshRenderer meshRenderer;
    public Rigidbody rb;
    public TextMeshPro valueText;
    public TextMeshPro valueText2;

    //when player lets go of chip, makes the chip handle its own physics
    public void DoWhenThrown()
    {
        rb.isKinematic = false;
    }
    
    //sets the color and money value of the chip accordingly. The colors are some common colors for blackjack chips
    public void SetColorAndValue()
    {
        Material mat = new Material(meshRenderer.sharedMaterial);
        valueText.text = value.ToString();
        valueText2.text = valueText.text;
        Color newColor = mat.color;
        if(value == 1)
        {
            //white
            newColor = new Color(0.778f,0.9f,1);
        }
        if(value == 2.5)
        {
            valueText.text = valueText.text + "0";
            valueText2.text = valueText.text;
            //pink
            newColor = new Color(1,0.307f,0.872f);
        }
        if(value == 5)
        {
            //red
            newColor = new Color(1,0,0.139f);
        }
        if(value == 10)
        {
            //blue
            newColor = new Color(0.286f,0.532f,1);
        }
        if(value == 20)
        {
            //gray
            newColor = new Color(0.557f,0.557f,0.557f);
        }
        if(value == 25)
        {
            //green
            newColor = new Color(0.29f,0.868f,0);
        }
        if(value == 50)
        {
            //orange
            newColor = new Color(1,0.502f,0);
        }
        if(value == 100)
        {
            //black
            newColor = Color.black;
        }
        if(value == 500)
        {
            //purple
            newColor = new Color(0.651f,0,0.635f);
        }
        if(value == 1000)
        {
            //yellow
            newColor = new Color(1,0.853f,0);
        }
        if(value == 5000)
        {
            //brown
            newColor = new Color(0.585f,0.092f,0);
        }
        mat.SetColor("_EmissionColor", newColor);
        meshRenderer.material = mat;
    }
}
