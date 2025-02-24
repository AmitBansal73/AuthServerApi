
** Build Docker Image

docker build -t auth-server -f Dockerfile .

docker build -t auth-server .


** check the docker profile port in Properties/launchSettings.json

** Run the Docker Image to deploy on docker

docker run -d -p 32778:8080 -e ASPNETCORE_URL=http://+32778 auth-server 

docker run -d -p 51234:8080 -e ENV=development auth-server 

docker run -dt -p 51234:8080 -e "ASPNETCORE_ENVIRONMENT=Development" auth-server

docker run -dt -p 51234:8080 -e "ASPNETCORE_ENVIRONMENT=Production" auth-server


** Run the application
http://localhost:5233/swagger/index.html

docker inspect -f '{{range.NetworkSettings.Networks}}{{.IPAddress}}{{end}}' 0be85565b558 

172.17.0.2

http://172.17.0.2:51222

docker run -dt -v "C:\Users\Amit Bansal\vsdbg\vs2017u5:/remote_debugger:rw" -v "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Sdks\Microsoft.Docker.Sdk\tools\linux-x64\net8.0:/VSTools:ro" -v "C:\Program Files\Microsoft Visual Studio\2022\Community\Common7\IDE\CommonExtensions\Microsoft\HotReload:/HotReloadAgent:ro" -e "ASPNETCORE_LOGGING__CONSOLE__DISABLECOLORS=true" -e "ASPNETCORE_ENVIRONMENT=Development" -P --name AuthServerApi --entrypoint dotnet authserverapi --roll-forward Major /VSTools/DistrolessHelper/DistrolessHelper.dll --wait 
d5600e97c05ef9297a470f858d93032d52c51f524b4575f9c10c303cf06000f6