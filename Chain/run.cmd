dotnet build
start "Node 1" dotnet run 6000 localhost 6001 --no-build
start "Node 2" dotnet run 6001 localhost 6002 --no-build
start "Node 3" dotnet run 6002 localhost 6003 --no-build
start "Node 4" dotnet run 6003 localhost 6004 --no-build
start "Node 5" dotnet run 6004 localhost 6005 --no-build
start "Node 6" dotnet run 6005 localhost 6006 --no-build