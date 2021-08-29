#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["api_gateway/api_gateway.csproj", "api_gateway/"]
COPY ["hotel_base/hotel_base.csproj", "hotel_base/"]
COPY ["common.libs/common.libs.csproj", "common.libs/"]
COPY ["ordering/ordering.csproj", "ordering/"]
COPY ["member_center/member_center.csproj", "member_center/"]
RUN dotnet restore "api_gateway/api_gateway.csproj"
COPY . .
WORKDIR "/src/api_gateway"
RUN dotnet build "api_gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "api_gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "api_gateway.dll"]