/*************************************************************

** Auth: ysd
** Date: 15.9.2
** Desc: 注册/登陆
** Vers: v1.0

*************************************************************/

using UnityEngine;
using System;
using System.Collections;
using KiiCorp.Cloud.Storage;

public class KiiLogin
{

    /// <summary>
    /// handle exception
    /// </summary>
    /// <param name="e"></param>
    private static void HandlerLoginException (Exception e)
    {
        if (e is NetworkException)
        {
            LoginUI.instance.ShowMessage("Invalid network");
        }
        else if (e is BadRequestException)
        {
            BadRequestException bre = (BadRequestException)e;
            LoginUI.instance.ShowMessage("Error : " + bre.reason);
            switch (bre.reason)
            {
                case BadRequestException.Reason.INVALID_ACCOUNT_STATUS:
                    break;
                case BadRequestException.Reason.INVALID_BUCKET:
                    break;
                case BadRequestException.Reason.INVALID_INPUT_DATA:
                    break;
                case BadRequestException.Reason.INVALID_JSON:
                    break;
                case BadRequestException.Reason.PASSWORD_TOO_SHORT:
                    break;
                case BadRequestException.Reason.QUERY_NOT_SUPPORTED:
                    break;
                case BadRequestException.Reason.__UNKNOWN__:
                    break;
                default:
                    break;
            }
        }
        else if (e is ConflictException)
        {
            ConflictException ce = (ConflictException)e;
            LoginUI.instance.ShowMessage("Error : " + ce.reason);
            switch (ce.reason)
            {
                case ConflictException.Reason.__UNKNOWN__:
                    break;
                case ConflictException.Reason.ACL_ALREADY_EXISTS:
                    break;
                case ConflictException.Reason.BUCKET_ALREADY_EXISTS:
                    break;
                case ConflictException.Reason.INVALID_STATUS:
                    break;
                case ConflictException.Reason.OBJECT_ALREADY_EXISTS:
                    break;
                case ConflictException.Reason.OBJECT_VERSION_IS_STALE:
                    break;
                case ConflictException.Reason.USER_ALREADY_EXISTS:
                    break;
                default:
                    break;
            }
        }
        else if (e is ForbiddenException)
        {
            ForbiddenException fbe = (ForbiddenException)e;
            LoginUI.instance.ShowMessage("Error : " + fbe.Status);
        }
        else if (e is CloudException)
        {
            CloudException ce = (CloudException)e;
            LoginUI.instance.ShowMessage("Error : " + ce.Status);
        }
        else
        {
            LoginUI.instance.ShowMessage("Unknow error");
        }
    }

    static public void Signup (string username, string password, string displayName, bool autoLogin)
    {
        //先在客户端检查是否合法
        if (KiiUser.IsValidUserName(username) && KiiUser.IsValidPassword(password) && KiiUser.IsValidDisplayName(displayName))
        {
            KiiUser.Builder userBuilder = KiiUser.BuilderWithName(username);
            KiiUser user = userBuilder.Build();
            user.Displayname = displayName;
            LoginUI.instance.Wait();
            user.Register(password, (KiiUser registeredUser, Exception e) =>
            {
                LoginUI.instance.StopWait();
                if (e == null)
                {
                    PlayerPrefs.SetInt("auto login", autoLogin ? 1 : 0);
                    PlayerPrefs.SetString("access token", KiiUser.AccessToken);
                    LoginUI.instance.OnLoginSuccess();
                }
                //处理异常
                else
                    HandlerLoginException(e);
            });
        }
        else
        {
            LoginUI.instance.ShowMessage("Invalid name or password");
        }
    }

    static public void Login (string username, string password, bool autoLogin)
    {
        if (KiiUser.IsValidUserName(username) && KiiUser.IsValidPassword(password))
        {
            LoginUI.instance.Wait();
            KiiUser.LogIn(username, password, (KiiUser loginedUser, Exception e) =>
            {
                LoginUI.instance.StopWait();
                if (e == null)
                {
                    PlayerPrefs.SetInt("auto login", autoLogin ? 1 : 0);
                    PlayerPrefs.SetString("access token", KiiUser.AccessToken);
                    LoginUI.instance.OnLoginSuccess();
                }
                else
                {
                    HandlerLoginException(e);
                }
            });
        }
        else
        {
            LoginUI.instance.ShowMessage("Invalid username or password");
        }
    }

    static public void AutoLogin ( )
    {
        string token = PlayerPrefs.GetString("access token");
        LoginUI.instance.Wait();
        KiiUser.LoginWithToken(token, (KiiUser loginedUser, Exception e) =>
        {
            LoginUI.instance.StopWait();
            if (e == null)
            {
                LoginUI.instance.OnLoginSuccess();
            }
            else
                HandlerLoginException(e);
        });
    }

}