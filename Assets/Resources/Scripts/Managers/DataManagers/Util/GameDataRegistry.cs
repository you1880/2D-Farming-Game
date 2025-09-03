using System;
using System.Collections;
using System.Collections.Generic;
public class GameDataRegistry<TId, TDef>
{
    private Dictionary<TId, TDef> _dataDict = new Dictionary<TId, TDef>();
    public IReadOnlyDictionary<TId, TDef> Dict => _dataDict;
    
    public void LoadData(IEnumerable<TDef> defs, Func<TDef, TId> keySelector)
    {
        _dataDict.Clear();

        foreach (var def in defs)
        {
            var key = keySelector(def);
            _dataDict[key] = def;
        }
    }

    public bool TryGet(TId id, out TDef def)
        => _dataDict.TryGetValue(id, out def);
}
