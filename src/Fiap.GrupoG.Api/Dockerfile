FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /build
EXPOSE 80

COPY . /build
RUN dotnet restore
RUN dotnet publish src/Fiap.GrupoG.Api/Fiap.GrupoG.Api.csproj -c RELEASE --self-contained  -r linux-x64 -o src/out

# build runtime image
FROM mcr.microsoft.com/dotnet/runtime:5.0
RUN sed -i 's/CipherString = DEFAULT@SECLEVEL=2/CipherString = DEFAULT@SECLEVEL=1/' /etc/ssl/openssl.cnf
WORKDIR /build
COPY --from=build /build/src/out ./

# ARG DOTNET_ENVIRONMENT
# ENV DOTNET_ENVIRONMENT=$DOTNET_ENVIRONMENT
# ARG BUCKETNAME
# ENV BUCKETNAME=BUCKETNAME

ENV TZ=Brazil/East

EXPOSE 80
ENTRYPOINT ["dotnet", "Fiap.GrupoG.Api.dll"]
