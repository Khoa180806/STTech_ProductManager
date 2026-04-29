FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

RUN dotnet tool install -g Microsoft.Web.LibraryManager.Cli
RUN ln -s ~/.dotnet/tools/libman /usr/local/bin/libman

WORKDIR /src
COPY ["src/ProductManager.Web.Mvc/ProductManager.Web.Mvc.csproj", "src/ProductManager.Web.Mvc/"]
COPY ["src/ProductManager.Web.Core/ProductManager.Web.Core.csproj", "src/ProductManager.Web.Core/"]
COPY ["src/ProductManager.Application/ProductManager.Application.csproj", "src/ProductManager.Application/"]
COPY ["src/ProductManager.Core/ProductManager.Core.csproj", "src/ProductManager.Core/"]
COPY ["src/ProductManager.EntityFrameworkCore/ProductManager.EntityFrameworkCore.csproj", "src/ProductManager.EntityFrameworkCore/"]
WORKDIR "/src/src/ProductManager.Web.Mvc"
RUN dotnet restore 

WORKDIR /src
COPY ["src/ProductManager.Web.Mvc", "src/ProductManager.Web.Mvc"]
COPY ["src/ProductManager.Web.Core", "src/ProductManager.Web.Core"]
COPY ["src/ProductManager.Application", "src/ProductManager.Application"]
COPY ["src/ProductManager.Core", "src/ProductManager.Core"]
COPY ["src/ProductManager.EntityFrameworkCore", "src/ProductManager.EntityFrameworkCore"]
WORKDIR "/src/src/ProductManager.Web.Mvc"
RUN libman restore
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "ProductManager.Web.Mvc.dll"]
