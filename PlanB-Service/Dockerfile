FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR pdf-filler

EXPOSE 80
EXPOSE 3000

#COPY PROJECT FILES
COPY ./*.csproj ./
RUN dotnet restore

#COPY EVERYTHING ELSE
COPY . .
RUN dotnet publish -c Release -o out .

#Build image
FROM mcr.microsoft.com/dotnet/sdk:6.0
WORKDIR /pdf-filler
COPY --from=build /pdf-filler/out .
ENTRYPOINT ["dotnet", "PlanB-Service.dll"]