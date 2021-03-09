using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Sprite dmgSprite;

    public AudioClip chopSound1;
    public AudioClip chopSound2;
    public int HP = 4;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    public void DamageWall(int loss) {
        spriteRenderer.sprite = dmgSprite;
        HP -= loss;

        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
        if (HP <= 0)
            gameObject.SetActive(false);
    }
}
