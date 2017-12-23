
dotnet build 
dotnet publish -r linux-arm

pscp.exe -r -pw raspberry .\RaspberryPi.IoT\bin\Debug\netcoreapp2.1\linux-arm\publish\* pi@192.168.1.66:/home/pi/jorat26/garage/
plink.exe -v -ssh -pw raspberry pi@192.168.1.66 chmod u+x,o+x /home/pi/jorat26/garage/RaspberryPi.IoT