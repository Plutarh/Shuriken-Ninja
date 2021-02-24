using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIPanel : MonoBehaviour
{
    [Header("Main Panel to tween animations")]
    public RectTransform tween;
    public Image alphaBackground;

    [Header("Animation Type")]
    public AnimationType animationType = AnimationType.noAnimation;
    public Animator animator;
    public RuntimeAnimatorController animatorController;
    public bool unscaledTime;
    public bool onlyFade;

    [Header("Time during which the panel cannot be closed")]
    public float panelShowHoldTime;
    float _panelShowHoldTime;

    [Header("Tween Animation Settings")]
    public UpdateType updateType;

    // Время для шага твин анмаций
    public float timeToTweenAction = 0.15f;

    // Дефолтный размер панели при появление (эффект пружины)
    Vector3 defaultShowSizeVector = new Vector3(1.2f,1.2f,1);

    // Дефолтный размер панели при скрытие (эффект пружины)
    Vector3 defaultHideSizeVector = new Vector3(1.2f, 1.2f, 1);

    // Если нужна кастомная последовательность векторов,которые будут использоваться при появление панели
    public List<Vector3> customShowStepsVector = new List<Vector3>();
    public static List<UIPanel> uiPanelStack = new List<UIPanel>();

    /// <summary>
    /// Ивент при показе
    /// </summary>
    public UnityEvent OnShow;

    /// <summary>
    /// Ивент при скрытие
    /// </summary>
    public UnityEvent OnHide;

    /// <summary>
    /// Ивент при скрытие,для открытия следующей панели
    /// </summary>
    public UnityEvent OnHideWithShowNext;

    /// <summary>
    /// Ивент при вызове показа
    /// </summary>
    public UnityEvent OnCallShow;

    /// <summary>
    /// Ивент при вызвое скрытия
    /// </summary>
    public UnityEvent OnCallHide;

    /// <summary>
    /// Эта панель может открыть след панель,не вызывая свой базовый OnHide ивент
    /// </summary>
    public bool canOpenNextPanel;

    public CanvasGroup canvasGroup;

    public enum AnimationType
    {
        TweenFade,
        TweenSlide,
        Animator,
        noAnimation
    }

    public virtual void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();

        unscaledTime = true;
    }
    public virtual void Start()
    {
        
    }

    public virtual void OnEnable()
    {

        //OnShow?.Invoke();
        if (!canvasGroup)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (!canvasGroup)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }
    }

    public virtual void Update()
    {
        HoldPanelTimer();
    }

    public void OpenNextPanelWithoutHideEvent()
    {
        canOpenNextPanel = true;
    }

    void HoldPanelTimer()
    {
        if (_panelShowHoldTime > 0) _panelShowHoldTime -= Time.deltaTime;
    }

    #region SHOW_REGION

    public void SetActive(bool isActive){
        if (isActive) Show();
        else Hide();
    }

    public void Show(bool instant = false)
    {
        OnCallShow?.Invoke();
        //if (!uiPanelStack.Contains(this)) uiPanelStack.Add(this);
        //if (uiPanelStack.Count > 1) return;

        if (!canvasGroup)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (!canvasGroup)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }


        if (panelShowHoldTime > 0) _panelShowHoldTime = panelShowHoldTime;

        if (instant)
        {
            tween.localScale = Vector3.one;
            canvasGroup.alpha = 1;
            gameObject.SetActive(true);
            OnShow?.Invoke();
        }

        switch (animationType)
        {
            case AnimationType.TweenFade:
                TweenFadeShow();
                break;
            case AnimationType.TweenSlide:
                TweenSlideShow();
                break;
            case AnimationType.Animator:
                AnimatorShow();
                break;
            case AnimationType.noAnimation:
               
                break;
        }
    }

    void TweenFadeShow()
    {
        if (tween == null)
        {
            Debug.LogError($"Tween Ref is NULL in {name}", this);
            return;
        }

        // Убьем все лишние ссылки на этот твин объект
        tween.DOKill(true);
        gameObject.SetActive(true);

        tween.localScale = Vector3.one;
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.3f).SetUpdate(unscaledTime).OnComplete(()=> { if (onlyFade) OnShow?.Invoke(); });

        //Debug.LogError($"Tween  {name} SHOWED", this);

        if (onlyFade) return;

        // Обнулим скейл панели
        tween.localScale = Vector3.zero;
        // Если кастомных настроек нет,просто дефолт анимация
        if (customShowStepsVector.Count == 0)
        {
            tween.DOScale(defaultShowSizeVector, timeToTweenAction).SetUpdate(unscaledTime).OnComplete(() =>
            {
                tween.DOScale(Vector3.one, timeToTweenAction).SetUpdate(unscaledTime).OnComplete(() =>
                {
                    OnShow?.Invoke();
                    canOpenNextPanel = false;
                });
            });
        }
        else
        {
            StartCoroutine(ETweeSizeShow());
        }
    }

    IEnumerator ETweeSizeShow()
    {
        int step = 0;
        while(step < customShowStepsVector.Count)
        {
            tween.DOScale(customShowStepsVector[step], timeToTweenAction).OnComplete(() =>
            {
                step++;
            });
            yield return new WaitForSecondsRealtime(timeToTweenAction);
        }
    }

    void TweenSlideShow()
    {

    }

    void AnimatorShow()
    {
        gameObject.SetActive(true);
        if (animator) animator.SetTrigger("Show");
    }
    #endregion

    #region HIDE_REGION
    public void Hide() 
    {
       
        if (_panelShowHoldTime > 0) return;
        OnCallHide?.Invoke();

        if (!canvasGroup)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        switch (animationType)
        {
            case AnimationType.TweenFade:
                TweenSizeHide();
                break;
            case AnimationType.TweenSlide:
                TweenSlideHide();
                break;
            case AnimationType.Animator:
                AnimatorHide();
                break;
            case AnimationType.noAnimation:
                gameObject.SetActive(false);
                OnHide.Invoke();
                break;
        }
    }

    void TweenSizeHide()
    {
        if (tween == null)
        {
            Debug.LogError("Tween Ref is NULL", this);
            return;
        }

        // Убьем все лишние ссылки на этот твин объект
        tween.DOKill(true);

        canvasGroup.alpha = 1;
        canvasGroup.DOFade(0, 0.3f).SetUpdate(unscaledTime).OnComplete(() =>
        {
            if (onlyFade)
            {
                OnHide?.Invoke();
            }
            gameObject.SetActive(false);
           
        });

        
        if (onlyFade) return;

        tween.DOScale(defaultHideSizeVector, timeToTweenAction).SetUpdate(unscaledTime).OnComplete(() =>
        {
            tween.DOScale(Vector3.zero, timeToTweenAction).SetUpdate(updateType,unscaledTime).OnComplete(() =>
            {
                gameObject.SetActive(false);
                if (!canOpenNextPanel) OnHide?.Invoke();
                else
                {
                    OnHideWithShowNext?.Invoke();
                    canOpenNextPanel = false;
                }
               
            });
        });
    }

    // Если мы накопили кучу панелей для открытия,они буду открываться последовательно,а не разом
    void ShowNextQueuePanel()
    {
        if (uiPanelStack.Count > 0)
        {
            uiPanelStack.RemoveAt(0);
            if (uiPanelStack.Count > 0)
            {
                uiPanelStack[0].Show();
            }
        }
    }

    void TweenSlideHide()
    {

    }

    void AnimatorHide()
    {
        if (animator) animator.SetTrigger("Hide");
    }

    public void HideEvent()
    {
        gameObject.SetActive(false);
    }


    public void SetTweenAnimation()
    {
        animationType = AnimationType.TweenFade;
        animator.runtimeAnimatorController = null;
    }

    public void SetAnimatorAnimation()
    {
        animationType = AnimationType.Animator;
        animator.runtimeAnimatorController = animatorController;
    }

    #endregion
}
