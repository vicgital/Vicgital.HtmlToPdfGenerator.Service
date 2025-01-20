# USE LATEST Debian 11 IMAGE for amd64
FROM amd64/debian:11
WORKDIR /app
EXPOSE 50051
COPY ["/Vicgital.HtmlToPdfGenerator.Service/", "/app/"]

# INSTALL COMMANDS
RUN apt-get update -y
RUN apt-get install -y sudo
RUN sudo apt-get install -y nano
RUN sudo apt-get install -y wget
# INSTALL .NET RUNTIME
RUN wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN sudo dpkg -i packages-microsoft-prod.deb
RUN rm packages-microsoft-prod.deb
RUN sudo apt-get update && \
    sudo apt-get install -y aspnetcore-runtime-8.0
# INSTALL DEPENDENCIES
RUN sudo apt-get install -y dirmngr ca-certificates gnupg
RUN sudo gpg --homedir /tmp --no-default-keyring --keyring /usr/share/keyrings/mono-official-archive-keyring.gpg --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
RUN echo "deb [signed-by=/usr/share/keyrings/mono-official-archive-keyring.gpg] https://download.mono-project.com/repo/debian stable-buster main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list
RUN echo apt-get update && \
    apt-get autoremove -y && \
    apt-get install -y --no-install-recommends libc6 libgcc1 libgssapi-krb5-2 libicu67 libssl1.1 libstdc++6 zlib1g \
    && chmod +x /app/wkhtmltopdf/linux/*
RUN apt-get update -y && apt-get install -y libgdiplus lsof
RUN ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll
# THIS FIXES libssl1.0-dev NOT BEING SUPPORTED FOR DEBIAN VERSIONS
RUN echo "deb http://archive.debian.org/debian-security stretch/updates main" | sudo tee /etc/apt/sources.list
RUN sudo apt-get update -y && apt-cache policy libssl1.0-dev
RUN sudo apt-get install -y libssl1.0-dev
RUN useradd -ms /bin/bash app

USER app
ENTRYPOINT ["dotnet", "Vicgital.HtmlToPdfGenerator.Service.dll"]