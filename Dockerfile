FROM mcr.microsoft.com/dotnet/sdk:8.0 as restore
WORKDIR /src
COPY Corretora.Identidade.sln ./
COPY nuget.config ./
COPY Corretora.Identidade.API/Corretora.Identidade.API.csproj ./Corretora.Identidade.API/
COPY Corretora.Identidade.Core/Corretora.Identidade.Core.csproj ./Corretora.Identidade.Core/
COPY Corretora.Identidade.CrossCutting.IoC/Corretora.Identidade.CrossCutting.IoC.csproj ./Corretora.Identidade.CrossCutting.IoC/
COPY Corretora.Identidade.Infra/Corretora.Identidade.Infra.csproj ./Corretora.Identidade.Infra/
COPY Corretora.Identidade.API.Tests/Corretora.Identidade.API.Tests.csproj ./Corretora.Identidade.API.Tests/

RUN dotnet restore "Corretora.Identidade.sln"
FROM restore AS build
COPY . .
RUN dotnet build "Corretora.Identidade.API/Corretora.Identidade.API.csproj" -c Release -o /app
RUN dotnet publish "Corretora.Identidade.API/Corretora.Identidade.API.csproj" -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet as base
COPY --from=build /publish /app
WORKDIR /app
EXPOSE 80
CMD ["./Corretora.Identidade"]