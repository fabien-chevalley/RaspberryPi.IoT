dotnet build 
dotnet publish .\RaspberryPi.IoT -r linux-arm

pscp.exe -r -pw raspberry .\RaspberryPi.IoT\bin\Debug\netcoreapp2.1\linux-arm\publish\RaspberryPi* pi@192.168.1.66:/home/pi/dev/
pscp.exe -r -pw raspberry .\RaspberryPi.IoT\bin\Debug\netcoreapp2.1\linux-arm\publish\MMALSharp* pi@192.168.1.66:/home/pi/dev/
plink.exe -v -ssh -pw raspberry pi@192.168.1.66 chmod u+x,o+x /home/pi/dev/RaspberryPi.IoT