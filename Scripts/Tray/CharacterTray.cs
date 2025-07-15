using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTray : TrayBase, IPlayerSettable
{
    [SerializeField] private GameObject maxTextImage;
    public int _MAX_CHARACTER_TRAY_COUNT = 10;
    private BaseCharacter _baseCharacter;

    protected override void Awake()
    {
        _OnTrayItemTypeChanageEvent -= ChangeTrayItemTypeChangeType;
        _OnTrayItemTypeChanageEvent += ChangeTrayItemTypeChangeType;
        if(maxTextImage) maxTextImage.SetActive(false);
    }

    private void Update()
    {
       if (!_baseCharacter) return;
       _baseCharacter.ChangeStackState(GetTrayObjectCount() > 0 ? CharacterStackType.Stack : CharacterStackType.Idle);

       if (!maxTextImage) return;
       MaxImageHandle();
    }

    private void MaxImageHandle()
    {
        if (GetTrayQueueObjectCount() >= _MAX_CHARACTER_TRAY_COUNT)
        {
            if (!maxTextImage.activeSelf)
            {
                maxTextImage.SetActive(true);
            }
        }
        else if(maxTextImage.activeSelf)
        {
           maxTextImage.SetActive(false);
        }
    }
    
    public override bool CanAddTrayItem()
    {
        return _MAX_CHARACTER_TRAY_COUNT > GetTrayObjectCount();
    }

    public void Connect(BaseCharacter player)
    {
        _baseCharacter = player;
    }

    public void Disconnect()
    {
        _baseCharacter = null;
    }
    
}
