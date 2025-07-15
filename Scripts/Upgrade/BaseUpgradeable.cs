using System;
using UnityEngine;

// 업그레이드 가능한 건물
public abstract class BaseUpgradeable : MonoBehaviour
{
    [Header("업그레이드 데이터")]
    [SerializeField] protected UpgradeData _upgradeData;
    [Header("(옵션) 업그레이드 완료 시 제거되는 추가 건물들")]
    [SerializeField] protected GameObject[] _removeObjects;
    
    public UpgradeData GetUpgradeData => _upgradeData;
    protected void Awake()
    {
        _upgradeData.FillArea.RequestMoney = _upgradeData.TotalRequestMoneny;
    }
    
    public abstract void Spawn();
    public abstract void Despawn();
}

[Serializable]
public struct UpgradeData
{
    [Header("레벨")] 
    public int Level;
    [Header("돈 충전 지역")]
    public FillArea FillArea;
    [Header("충전 완료에 필요하는 돈")]
    public int TotalRequestMoneny;
    [Header("충전 완료 시 제거되는 건물")]
    public BaseUpgradeable Legacy;
    [Header("충전 완료 시 업그레이드 되는 건물")]
    public BaseUpgradeable Target;
}
