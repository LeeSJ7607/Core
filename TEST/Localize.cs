using System.Collections.Generic;
using UnityEngine;

public sealed class Localize : MonoBehaviour
{
    [SerializeField] private string _key;

    public bool TryGetText(out IReadOnlyDictionary<SystemLanguage, string> localizeTableMap_, out string errorMsg_)
    {
        localizeTableMap_ = null;
        errorMsg_ = null;
        
        if (Application.isPlaying == false)
        {
            errorMsg_ = "유니티를 실행했을 경우에만 보여집니다.";
            return false;
        }

        if (_key.IsNullOrWhiteSpace())
        {
            errorMsg_ = "키 값이 비어있습니다.";
            return false;
        }
        
        var table = TableManager.Instance.Get<TblLocalize>();
        if (table.Dic.TryGetValue(_key, out var value) == false)
        {
            errorMsg_ = $"{_key}.. KeyNotFoundException";
            return false;
        }

        localizeTableMap_ = value;
        return true;
    }
}