
namespace ClueGameHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            ClueGameHelper clueGamehelper = new ClueGameHelper();
            clueGamehelper.RegisterPlayers();
            clueGamehelper.RunGame();
        }
    }
}
