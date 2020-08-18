# TextAdventureGame

Software used to run Furventure games https://furrytextadventures.com/

These programs communicate via either Discord, Telegram, Kik, or on a Website.
The program looks in Google Drive for exported draw.io XML files, in a format defined in this document: https://docs.google.com/document/d/1HjVx7QFzzNt5z3yenMGs99I-j7ZPuEGAGpztOQ-dwhc/
It will parse the xml file and create a game that can be played on any of the above platforms. Slack logging or logging to file is supported.

The AdminSite project runs an asp.net MVC web app to display reports in daily usage, what games are been played, and helps with debugging errors that may be reported from a player.

The public site uses asp.net blazor WebAssembly to display the games output on the browser.

The DrawGameTester rapidly executes a game, and returns any end states or other errors that it finds. It can also use azure spell checking to check for basic grammer and spelling mistakes.

This software currently runs on .net core 3.1
