using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManagerEx
{
    private const float WAIT_TIME = 1.0f;
    private const float FADE_DELAY_TIME = 1.0f;
    private const float ALPHA_OPAQUE = 1.0f;
    private const float ALPHA_TRANSPARENT = 0.0f;
    private GameObject _blocker;
    private UI_Blocker _blockerUI;
    private Image _blockerImage;
    public event Action<Define.SceneType> OnSceneChanged;
    public BaseScene CurrentScene { get { return GameObject.Find("@Scene").GetComponent<BaseScene>(); } }
    public Define.SceneType CurrentSceneType { get { return CurrentScene == null ? Define.SceneType.Unknown : CurrentScene.CurrentScene; }}

    public void LoadNextScene(Define.SceneType sceneType, PlayerController playerController = null)
    {
        if (CurrentSceneType == sceneType)
        {
            return;
        }

        string sceneName = GetSceneName(sceneType);
        Managers.RunCoroutine(LoadSceneAsync(sceneName, playerController));
    }

    public IEnumerator MoveSceneArea(PlayerController controller = null, Action action = null)
    {
        if (controller != null)
        {
            controller.ChangePlayerState(Define.PlayerState.Idle);
            controller.enabled = false;
        }

        yield return Fade(ALPHA_TRANSPARENT, ALPHA_OPAQUE);

        action?.Invoke();

        yield return Fade(ALPHA_OPAQUE, ALPHA_TRANSPARENT);
        Managers.UI.CloseUI(_blocker);

        if (controller != null)
        {
            controller.enabled = true;
        }

        yield break;
    }

    private void CreateBlockerUI()
    {
        _blockerUI = Managers.UI.ShowUI<UI_Blocker>();
        _blocker = _blockerUI.gameObject;
        _blockerImage = _blocker.GetComponent<Image>();
    }

    private string GetSceneName(Define.SceneType type)
        => Enum.GetName(typeof(Define.SceneType), type);
    
    private IEnumerator LoadSceneAsync(string sceneName, PlayerController playerController = null)
    {
        if (playerController != null)
        {
            playerController.ChangePlayerState(Define.PlayerState.Idle);
            playerController.enabled = false;
        }

        yield return new WaitForSeconds(0.1f);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float startTime = Time.time;

        yield return Managers.RunCoroutine(FadeOut());

        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        float elapsed = Time.time - startTime;

        if (elapsed < WAIT_TIME)
        {
            yield return new WaitForSeconds(WAIT_TIME - elapsed);
        }

        yield return new WaitForSeconds(WAIT_TIME / 2.0f);

        operation.allowSceneActivation = true;
        Managers.UI.ClearUIDictionary();

        yield return null;

        OnSceneChanged?.Invoke(CurrentSceneType);

        yield return Managers.RunCoroutine(FadeIn());
        
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }

    private IEnumerator Fade(float start, float end)
    {
        CreateBlockerUI();

        float elapsed = 0.0f;

        Color color = _blockerImage.color;
        _blockerImage.color = new Color(color.r, color.g, color.b, start);

        while (elapsed < FADE_DELAY_TIME)
        {
            if (_blockerImage == null)
            {
                yield break;
            }
            
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / FADE_DELAY_TIME);
            float alpha = Mathf.Lerp(start, end, t);

            _blockerImage.color = new Color(color.r, color.g, color.b, alpha);

            yield return null;
        }

        yield return null;
    }

    private IEnumerator FadeIn()
    {
        yield return Managers.RunCoroutine(Fade(ALPHA_OPAQUE, ALPHA_TRANSPARENT));

        Managers.UI.CloseUI(_blocker);
    }

    private IEnumerator FadeOut()
    {
        yield return Managers.RunCoroutine(Fade(ALPHA_TRANSPARENT, ALPHA_OPAQUE));
    }
}
