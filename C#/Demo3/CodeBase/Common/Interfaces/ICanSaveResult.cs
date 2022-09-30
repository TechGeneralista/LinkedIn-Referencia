using Common.SaveResult;
using Common.Settings;
using System.Collections.Generic;

namespace Common.Interfaces
{
    public interface ICanSaveResult
    {
        void SaveResult(KeyCreator keyCreator, List<SaveResultDTO> list);
    }
}
