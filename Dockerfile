# Use the official .NET 8 ASP.NET runtime image as the base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 50051
COPY ["/Vicgital.HtmlToPdfGenerator.Service/", "/app/"]

# Install required packages
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
        sudo \
        nano \
        wget \
        dirmngr \
        ca-certificates \
        gnupg \
        libc6 \
        libgcc1 \
        libgssapi-krb5-2 \
        libicu72 \
        libssl3 \
        libstdc++6 \
        zlib1g \
        libgdiplus \
        lsof && \
    ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll && \
    rm -rf /var/lib/apt/lists/*

# Mono repository and key (for compatibility, as in your original)
RUN apt-get update && \
    apt-get install -y --no-install-recommends dirmngr gnupg ca-certificates && \
    gpg --homedir /tmp --no-default-keyring --keyring /usr/share/keyrings/mono-official-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF && \
    echo "deb [signed-by=/usr/share/keyrings/mono-official-archive-keyring.gpg] https://download.mono-project.com/repo/debian stable-buster main" > /etc/apt/sources.list.d/mono-official-stable.list

# Install legacy libssl1.0-dev (if needed)
RUN echo "deb http://archive.debian.org/debian-security stretch/updates main" > /etc/apt/sources.list && \
    apt-get update && \
    apt-get install -y --no-install-recommends libssl1.0-dev || true

# Add the certificate to the trusted store
RUN update-ca-certificates

RUN chmod +x /app/wkhtmltopdf/linux/*

USER app
ENTRYPOINT ["dotnet", "Vicgital.HtmlToPdfGenerator.Service.dll"]