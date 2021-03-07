using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Sprite dmgSprite;
    public int HP = 4;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    public void DamageWall(int loss) {
        spriteRenderer.sprite = dmgSprite;
        HP -= loss;
        if (HP <= 0)
            gameObject.SetActive(false);
    }
}
