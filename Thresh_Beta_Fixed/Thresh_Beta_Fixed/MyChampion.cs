using EloBuddy;

namespace Thresh_Beta_Fixed
{
    public static class MyChampion
    {
        public static AIHeroClient MyThresh => Player.Instance;
        //Stack Enemys
        public static AIHeroClient Enemys
        {
            get { return ObjectManager.Player; }
        }
        public static AIHeroClient _Player
        {
            get { return ObjectManager.Player; }
        }
    }
}
