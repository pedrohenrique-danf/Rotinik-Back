# Guia de Instalação e Configuração

Instalaremos e configuraremos:

- [.NET 10.0 SDK](https://dotnet.microsoft.com)
- [PostgreSQL](https://www.postgresql.org)
- [Docker](https://docs.docker.com/)
- [DBeaver](https://dbeaver.io/)

## Instalação

```sh
sudo apt update
```

### .NET 10.0 SDK

```sh
sudo apt install -y dotnet-sdk-10.0
dotnet tool install --global dotnet-ef
```

### Postgresql

```sh
sudo apt install -y postgresql postgresql-contrib
```

configura a senha do postgres como `postgres`
```sh
sudo systemctl start postgres
sudo -u postgres psql -c "ALTER USER postgres PASSWORD 'postgres';"
```

### Docker

```bash
sudo apt install -y docker.io
sudo usermod -aG docker $USER
sudo systemctl start docker
sudo chmod 666 /var/run/docker.sock
```

### DBeaver

```bash
wget [https://dbeaver.io/files/dbeaver-ce_latest_amd64.deb](https://dbeaver.io/files/dbeaver-ce_latest_amd64.deb)
sudo apt install ./dbeaver-ce_latest_amd64.deb
rm dbeaver-ce_latest_amd64.deb
```

## Configuração

### .NET e BD

1. Abra `src/appsettings.Development.json`.
2. Ponha o conteúdo abaixo no arquivo.

```json
   {
       "ConnectionStrings": {
           "DefaultConnection": "Host=localhost;Port=5432;Database=rotinik_db;Username=postgres;Password=postgres"
       },
       "JwtSettings": {
           "Secret": "RotinikSuperSecretKeyThatNeedsToBeAtLeast32CharactersLong!",
           "Issuer": "RotinikAPI",
           "Audience": "RotinikClients"
       },
       "Logging": {
           "LogLevel": {
               "Default": "Information",
               "Microsoft.AspNetCore": "Warning"
           }
       },
       "AllowedHosts": "*"
   }
```

```sh
dotnet ef database update --project src/Rotinik-Backend.csproj
```

### DBeaver

1. Para visualizar seus usuários e tabelas, siga estes passos para configurar a conexão:
2. Abra o DBeaver.
3. Clique no botão "Nova Conexão" (ícone de tomada com um plugue azul).
4. Selecione PostgreSQL e clique em Next.
5. Preencha as configurações conforme abaixo:
    - Host: localhost
    - Port: 5432
    - Database: rotinik_db
    - Username: postgres
    - Password: postgres

6. Clique em "Test Connection".
7. Se o DBeaver pedir para baixar os drivers do PostgreSQL, clique em Download.
8. Se o teste passar, clique em Finish.
