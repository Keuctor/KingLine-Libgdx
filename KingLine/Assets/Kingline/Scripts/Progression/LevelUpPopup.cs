using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LevelUpPopup : MonoBehaviour
{
    [SerializeField]
    private TMP_Text m_titleText;

    [SerializeField]
    private TMP_Text m_levelText;

    [SerializeField]
    private TMP_Text m_unspentSkillPointText;

    [SerializeField]
    private RectTransform m_rectTransform;

    private void Start()
    {
        var m_progressionNetworkController = NetworkManager.Instance.GetController<ProgressionNetworkController>();
        AudioManager.Instance.PlayOnce(SoundType.LEVEL_UP, true, 0.4f);
        m_levelText.text = $"LEVEL {m_progressionNetworkController.Level}";
        m_unspentSkillPointText.text =
            $"You have {m_progressionNetworkController.SkillPoint} unspent skill points";


        StartCoroutine(UpdateUI());
    }

    private IEnumerator UpdateUI()
    {
        yield return new WaitForEndOfFrame();
        m_rectTransform.anchoredPosition = new Vector2(0, m_rectTransform.sizeDelta.y);
        m_rectTransform.DOAnchorPos(new Vector2(0, 0), 0.6f).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(5f);
        m_rectTransform.DOAnchorPos(new Vector2(0, m_rectTransform.sizeDelta.y), 0.5f)
            .SetEase(Ease.InBounce);
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}