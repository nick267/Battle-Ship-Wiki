using System;
using SwinGameSDK;
using SwinGameSDK.SwinGame; // requires mcs version 4+, 

namespace MyGame
{
    public class GameMain1
    {
        public static void Main()
        {
            //Open the game window

            SwinGame.OpenGraphicsWindow("GameMain", 800, 600);
            ShowSwinGameSplashScreen();
            
            //Run the game loop
            while(false == WindowCloseRequested())
            {
                //Fetch the next batch of UI interaction
                ProcessEvents();
                
                //Clear the screen and draw the framerate
                ClearScreen(Color.White);
                DrawFramerate(0,0);
                
                //Draw onto the screen
                RefreshScreen(60);
            }
        }
    }
}