using Common.Tool;
using IOBoard;


namespace ProcessControlApp
{
    public class MainViewModel
    {
        public IOBoardClient IOBoardClient => ObjectContainer.Get<IOBoardClient>();


        public MainViewModel()
        {
            
        }
    }
}