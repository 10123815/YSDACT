/*************************************************************

** Auth: ysd
** Date: 15.9.2
** Desc: 注册/登陆界面
** Vers: v1.0

*************************************************************/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class LoginUI : MonoBehaviour
{

    static public LoginUI instance;

    public InputField usernameInput;
    public InputField passwordInput;
    public InputField displayNameInput;
    public Toggle autoLoginToggle;
    public Button changeFuncButton;
    public Button goButton;

    private CanvasGroup m_canvasGroup;

    void Start ( )
    {
        instance = this;

        m_canvasGroup = GetComponent<CanvasGroup>();

        goButton.onClick.AddListener(( ) =>
        {
            Signup();
        });
        GameObject nickName = displayNameInput.transform.parent.gameObject;
        Text text = changeFuncButton.GetComponentInChildren<Text>();
        GridLayoutGroup grid = GetComponentInChildren<GridLayoutGroup>();
        changeFuncButton.onClick.AddListener(( ) =>
        {
            if (text.text == "NO ACCOUNT?")
            {
                grid.padding.top = 0;
                grid.spacing = Vector2.zero;
                nickName.SetActive(true);
                text.text = "HAVE AN ACCOUNT";
                goButton.onClick.RemoveAllListeners();
                goButton.onClick.AddListener(( ) =>
                {
                    Signup();
                });
            }
            else
            {
                grid.padding.top = 30;
                grid.spacing = new Vector2(0, 30);
                nickName.SetActive(false);
                text.text = "NO ACCOUNT?";
                goButton.onClick.RemoveAllListeners();
                goButton.onClick.AddListener(( ) =>
                {
                    Login();
                });
            }
        });

        //自动登录
        if (PlayerPrefs.HasKey("auto login") && PlayerPrefs.GetInt("auto login") == 1)
            KiiLogin.AutoLogin();
    }

    public void Signup ( )
    {
        KiiLogin.Signup(usernameInput.text, passwordInput.text, displayNameInput.text, autoLoginToggle.isOn);
    }

    public void Login ( )
    {
        KiiLogin.Login(usernameInput.text, passwordInput.text, autoLoginToggle.isOn);
    }

    public void OnLoginSuccess ( )
    {
        CommonCanvasManager.GetInstance().ShowMessage("Login succees", null, 0.5f);
        GameObject matchUICanvas = GameObject.Find("match ui");
        Sequence seq = DOTween.Sequence();
        CanvasGroup loginCanvasGroup = GetComponent<CanvasGroup>();
        CanvasGroup matchCanvasGroup = matchUICanvas.GetComponent<CanvasGroup>();
        seq.Append(transform.DOScale(0.5f, 0.2f)).Join(loginCanvasGroup.DOFade(0, 0.2f)).Join(matchCanvasGroup.DOFade(1, 0.2f)).AppendCallback(( ) =>
        {
            matchCanvasGroup.interactable = true;
            Destroy(transform.parent.gameObject);
        });
    }


    internal void ShowMessage (string p, float seconds = 2)
    {
        CanvasGroup[] cg = new CanvasGroup[1];
        cg[0] = m_canvasGroup;
        CommonCanvasManager.GetInstance().ShowMessage(p, cg, seconds);
    }

    public void Wait ( )
    {
        //message UI
        GameObject canvas = CommonCanvasManager.GetInstance().ShowMessage("Wait", new CanvasGroup[1] { m_canvasGroup });
    }

    public void StopWait ( )
    {
        CommonCanvasManager.GetInstance().CloseMessagePanel(new CanvasGroup[1] { m_canvasGroup });
    }

}