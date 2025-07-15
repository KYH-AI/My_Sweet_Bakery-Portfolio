using System;
using System.Collections;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private GameObject _arrowMaker;
    
    public TutorialType CurrentTutorialState { get; private set; } = TutorialType.BreadOven;
    private Coroutine _tutorialCoroutine;
    
    /*
        순차적으로 진행
        - 오븐 마크 표시
        - 빵 진열대 마크 표시
        - 카운터 마크 표시
        - 카운터 돈 Tray 마크 표시
        - 테이블 Unlock 마크 표시
        - 쓰레기 마크 표시
     */
    
    private void Start()
    {
        _arrowMaker = ResourceLoad.Instantiate(_arrowMaker);
        _arrowMaker.SetActive(false);
        _tutorialCoroutine = StartCoroutine(nameof(TutorialProcess));
    }
    
    private IEnumerator TutorialProcess()
    {
        Vector3 arrowPosition;
        
        BreadOvenPipeline bopl = GameManager.Instance.CustomerManager.BreadOven;
        arrowPosition = new Vector3(bopl.transform.position.x, 5f, bopl.transform.position.z);
        _arrowMaker.transform.position = arrowPosition;
        _arrowMaker.SetActive(true);
        PlayerCharacterController.Instance.SetTutorialTarget(bopl.transform);

        yield return new WaitUntil(() => bopl.IsBusy);

        BreadBasketPipeline bbpl = GameManager.Instance.CustomerManager.BreadBasket;
        arrowPosition = new Vector3(bbpl.transform.position.x, 5f, bbpl.transform.position.z);
        _arrowMaker.transform.position = arrowPosition;
        _arrowMaker.SetActive(true);
        PlayerCharacterController.Instance.SetTutorialTarget(bbpl.transform);

        yield return new WaitUntil(() => bbpl.IsBusy);
        
        CounterDeskPipeline cdpl  = GameManager.Instance.CustomerManager.CounterDesk;
        arrowPosition = new Vector3(cdpl.transform.position.x + 1.5f, 5f, cdpl.transform.position.z);
        _arrowMaker.transform.position = arrowPosition;
        _arrowMaker.SetActive(true);
        PlayerCharacterController.Instance.SetTutorialTarget(cdpl.transform);
        
        // 돈이 생기면
        yield return new WaitUntil(() => cdpl.MoneyTray.GetTrayObjectCount() > 0);
        
        arrowPosition = new Vector3(cdpl.MoneyTray.transform.position.x, 5f, cdpl.MoneyTray.transform.position.z);
        _arrowMaker.transform.position = arrowPosition;
        _arrowMaker.SetActive(true);
        PlayerCharacterController.Instance.SetTutorialTarget(cdpl.MoneyTray.transform);
        
        // 돈을 다 획득할 때 까지
        yield return new WaitUntil(() => cdpl.MoneyTray.GetTrayObjectCount() <= 0);

        TablePipeline tpli = GameManager.Instance.CustomerManager.Table;
        arrowPosition = new Vector3(tpli.transform.position.x, 5f, tpli.transform.position.z);
        _arrowMaker.transform.position = arrowPosition;
        _arrowMaker.SetActive(true);
        PlayerCharacterController.Instance.SetTutorialTarget(tpli.transform);
        
        // 테이블이 존재 할 때 까지
        yield return new WaitUntil(() => tpli.FindTable(1));
        
        Table table = tpli.GetTableByIndex(0);
        _arrowMaker.SetActive(false);
        PlayerCharacterController.Instance.SetTutorialTarget(null);
        
        // 테이블에 쓰레기가 존재 할 떄 까지
        yield return new WaitUntil(() => table.TableStateType is TableStateType.Garbage);
        
        arrowPosition = new Vector3(table.transform.position.x, 5f, table.transform.position.z);
        _arrowMaker.transform.position = arrowPosition;
        _arrowMaker.SetActive(true);
        PlayerCharacterController.Instance.SetTutorialTarget(table.transform);
        
        // 테이블에 쓰레기가 없을 때 까지
        yield return new WaitUntil(() => table.TableStateType is TableStateType.None);
        
        _arrowMaker.SetActive(false);
        PlayerCharacterController.Instance.SetTutorialTarget(null);
        // TODO : Unknown...
    }
}
