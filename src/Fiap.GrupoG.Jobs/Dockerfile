FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /build
EXPOSE 80

COPY . /build
RUN dotnet restore
RUN dotnet publish src/Fiap.GrupoG.Jobs/Fiap.GrupoG.Jobs.csproj -c RELEASE --self-contained  -r linux-x64 -o src/out

# build runtime image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1
RUN sed -i 's/CipherString = DEFAULT@SECLEVEL=2/CipherString = DEFAULT@SECLEVEL=1/' /etc/ssl/openssl.cnf
WORKDIR /build
COPY --from=build /build/src/out ./

ARG DOTNET_ENVIRONMENT
ENV DOTNET_ENVIRONMENT=$DOTNET_ENVIRONMENT

ENV TZ=Brazil/East

ENTRYPOINT ["dotnet", "Fiap.GrupoG.Jobs.dll"]
