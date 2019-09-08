# README #

This project is an implementation of the classic Battleship game.

Requirements:

* Windows machine or Mono installed
* Unix command line (use something like MSYS2 on Windows)

To get started:

1. Clone project
1. Open command line and cd to project root
1. Build using `build.sh`
1. Run using `run.sh` -- you can also find the executable in the bin folder, but this script makes it simpler to build and run from the command line.

## Game Menu: ##

The menu has four buttons:
1. Play - Start the game against an AI opponent
1. Setup - Choose an AI Difficulty (Easy, Medium or Hard)
1. Scores - View the highscores of this game
1. Quit - Exit the game

## Playing the game ##

Setting up
* You can set the position of your boats on the board by clicking on the on the board.
* You can use click the rotate icon to randomise the boat positions
* You can use the right and up arrow keys select what orientation your boats will be when you place them

Playing
* Click on an empty box on the board. If it turns red, you've hit a boat and get a second consecutive click
* The game ends when all boats of either you or your opponent are destroyed
