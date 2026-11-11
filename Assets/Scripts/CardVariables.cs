using UnityEngine;

public class CardVariables : MonoBehaviour
{
    public int value;
    public Sprite sprite;
    public MeshRenderer meshRenderer;
    public Rigidbody rb;
    public int number;
    public Texture2D cardBack;

    //when card is removed from deck, sets kinematic false to let the card handle its own physics and removes it from deck.
    public void DoWhenRemoveFromDeck()
    {
        SetKinematic(false);
        RemoveFromDeck();
    }

    //sets IsKinematic to given value.
    public void SetKinematic(bool value)
    {
        rb.isKinematic = value;
    }

    //sets the parent to null when removed from deck.
    public void RemoveFromDeck()
    {
        gameObject.transform.parent = null;
    }

    //sets the main texture of the specific copy of the material to the given sprite.
    public void SetMaterial(Sprite newSprite)
    {
        sprite = newSprite;
        var cardTexture = new Texture2D((int) sprite.rect.width, (int) sprite.rect.height);
        var pixels = sprite.texture.GetPixels((int) sprite.rect.x, (int) sprite.rect.y, (int) sprite.rect.width, (int) sprite.rect.height);
        cardTexture.SetPixels(pixels);
        cardTexture.Apply();
        Material m = new Material(meshRenderer.sharedMaterial.shader);
        m.mainTexture = cardTexture;
        m.SetTexture("_Back",cardBack);
        meshRenderer.sharedMaterial = m;
    }
}
