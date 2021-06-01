using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockView : MonoBehaviour
{
    [SerializeField] private List<Sprite> elementsImage;
    [SerializeField] private Image image;

    /*
     * Methods
     */
    public void SelectSprite(BlockModel.BLOCK_TYPE blockType)
    {
        switch (blockType)
        {
            case BlockModel.BLOCK_TYPE.Air:
                image.sprite = elementsImage[0];
                break;

            case BlockModel.BLOCK_TYPE.Earth:
                image.sprite = elementsImage[1];
                break;

            case BlockModel.BLOCK_TYPE.Fire:
                image.sprite = elementsImage[2];
                break;

            case BlockModel.BLOCK_TYPE.Water:
                image.sprite = elementsImage[3];
                break;

            case BlockModel.BLOCK_TYPE.Flash:
                image.sprite = elementsImage[4];
                break;

            case BlockModel.BLOCK_TYPE.Demon:
                image.sprite = elementsImage[5];
                break;

            case BlockModel.BLOCK_TYPE.Forest:
                image.sprite = elementsImage[6];
                break;

            case BlockModel.BLOCK_TYPE.Spirit:
                image.sprite = elementsImage[7];
                break;

            default:
                break;
        }
    }
}
