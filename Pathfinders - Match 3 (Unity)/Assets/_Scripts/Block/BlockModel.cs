using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockModel : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private BlockView blockView;

    [Header("Main")]
    [SerializeField] private BLOCK_TYPE blockType;

    public BLOCK_TYPE BlockType
    {
        get { return blockType; }
        set { blockType = value; blockView.SelectSprite(BlockType); }
    }

    public enum BLOCK_TYPE
    {
        Air,
        Earth,
        Fire,
        Water,
        Flash,
        Demon,
        Forest,
        Spirit
    }

    private void Start()
    {
        int elementsAmount = Enum.GetValues(typeof(BLOCK_TYPE)).Length;
        blockType = (BLOCK_TYPE)Random.Range(0, elementsAmount);

        blockView.SelectSprite(blockType);
    }

    public void DestroyBlock(bool isPlayer)
    {
        if (isPlayer)
        {
            GameManager.Instance.AddScore(GameManager.Instance.ComboScore*2);
        }

        GameManager.Instance.AddScore(GameManager.Instance.ComboScore / 4);

        Destroy(gameObject);
    }



}
