start redis_stop.cmd

taskkill /f /im Valuator.exe
taskkill /f /im RankCalculator.exe
taskkill /f /im EventsLogger.exe
taskkill /f /im nats-server.exe
taskkill /f /im cmd.exe
cd "../nginx" & nginx.exe -s quit
