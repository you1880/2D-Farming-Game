using System.Collections;
using System.Collections.Generic;

public enum MessageID
{
    GameQuitMsg,
    NameNotFilled,
    FarmNameNotFilled,
    DeleteSaveData,
    CreateSaveData,
    ReCreateSaveData,
    LoadDataError,
    GoNextDay,
    NotForSale,
    RemoveItem,
    NoSaveWarning
}

public static class MessageTable
{
    private static readonly Dictionary<MessageID, string> _messageTable = new Dictionary<MessageID, string>
    {
        {MessageID.GameQuitMsg, "정말 종료하시겠습니까?"},
        {MessageID.NameNotFilled, "이름을 입력해주세요."},
        {MessageID.FarmNameNotFilled, "농장 이름을 입력해주세요."},
        {MessageID.DeleteSaveData, "해당 세이브를 삭제하시겠습니까?"},
        {MessageID.CreateSaveData, "세이브 파일을 생성하시겠습니까?"},
        {MessageID.ReCreateSaveData, "기존 세이브를 삭제하고 재생성하시겠습니까?"},
        {MessageID.LoadDataError, "데이터를 불러오는데 실패하였습니다."},
        {MessageID.GoNextDay, "주무시겠습니까?"},
        {MessageID.NotForSale, "비매품입니다."},
        {MessageID.RemoveItem, "아이템을 버리시겠습니까?"},
        {MessageID.NoSaveWarning, "저장되지 않은 데이터는 사라집니다."} 
    };

    public static string GetMessage(MessageID id)
    {
        if (_messageTable.TryGetValue(id, out string msg))
        {
            return msg;
        }

        return "";
    }
}
