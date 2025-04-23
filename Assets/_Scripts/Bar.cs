using System.Collections;
using UnityEngine;

public class Bar : MonoBehaviour {


    public float AnimationSpeed = 10f;
    public int MaxValue = 0;
    
    private int Value = 0;

    [SerializeField]
    private RectTransform TopBar;
    [SerializeField]
    private RectTransform BottomBar;

    private Coroutine AdjustBarWidth;
    
    private float FullWidth;
    private float TargetWidth => Value * FullWidth / MaxValue;

    private Camera mainCamera;
    private Canvas canvas;

    void Awake() {
        canvas = GetComponent<Canvas>();
        mainCamera = Camera.main;
        canvas.worldCamera = mainCamera;
        
    }


    private void Start() {
        FullWidth = TopBar.rect.width;
        Value = MaxValue;

    }


    public void Change(int amount){
        Value = Mathf.Clamp(Value + amount, 0, MaxValue);
        if(AdjustBarWidth != null){
            StopCoroutine(AdjustBarWidth);
        }

        AdjustBarWidth = StartCoroutine(AdjustWidth(amount));

    }

    private IEnumerator AdjustWidth(int amount){
        var suddenBarChange = amount >= 0 ? BottomBar : TopBar;
        var slowBarChange = amount >= 0 ? TopBar: BottomBar;
        suddenBarChange.SetWidth(TargetWidth);

        while(Mathf.Abs(suddenBarChange.rect.width - slowBarChange.rect.width) > 1f){
            slowBarChange.SetWidth(Mathf.Lerp(slowBarChange.rect.width, TargetWidth, Time.deltaTime *  AnimationSpeed));
            yield return null;
        }
        slowBarChange.SetWidth(TargetWidth);
    }

}
