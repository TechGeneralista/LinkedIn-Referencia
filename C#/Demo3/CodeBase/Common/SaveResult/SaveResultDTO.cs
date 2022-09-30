using Common.Settings;
using System.Windows.Media;


namespace Common.SaveResult
{
    public class SaveResultDTO
    {
        public string Id { get; }
        public string Result { get; }
        public ImageSource Image { get; }


        public SaveResultDTO(KeyCreator keyCreator, object result, ImageSource image)
        {
            Id = keyCreator.Key;

            if (result.IsNull())
                Result = Common.Result.NotAvailable.ToString();
            else
            {
                int? intResult = result as int?;
                bool? boolResult = result as bool?;

                if (intResult.IsNotNull())
                    Result = intResult.ToString();

                else if (boolResult.IsNotNull())
                {
                    if ((bool)result)
                        Result = Common.Result.Ok.ToString();
                    else
                        Result = Common.Result.Nok.ToString();
                }
            }

            Image = image;
        }
    }
}
