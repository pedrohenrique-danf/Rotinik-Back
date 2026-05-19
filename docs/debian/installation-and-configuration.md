# Guia de Instalação e Configuração

Instalaremos e configuraremos:

- [.NET 10.0 SDK](https://dotnet.microsoft.com)
- [PostgreSQL](https://www.postgresql.org)
- [Docker](https://docs.docker.com/)
- [DBeaver](https://dbeaver.io/)

## Instalação

```sh
sudo apt update
sudo apt install -y fish eza

# .NET 10
sudo apt install -y dotnet-sdk-10.0
dotnet tool install --global dotnet-ef

# PostgreSQL
sudo apt install -y postgresql postgresql-contrib

# Docker
sudo apt install -y docker.io

# DBeaver
wget [https://dbeaver.io/files/dbeaver-ce_latest_amd64.deb](https://dbeaver.io/files/dbeaver-ce_latest_amd64.deb)
sudo apt install ./dbeaver-ce_latest_amd64.deb
rm dbeaver-ce_latest_amd64.deb

# Repositório
git clone https://github.com/pedrohenrique-danf/Rotinik-Back.git
```

## Configuração


### PostgreSQL & Docker

```sh
# PostgreSQL
sudo systemctl start postgresql
sudo -u postgres psql -c "ALTER USER postgres PASSWORD 'postgres';"

# Docker
sudo usermod -aG docker $USER
sudo systemctl start docker
```

### DBeaver

1. Abra o DBeaver.
2. Clique no botão "Nova Conexão" (ícone de tomada com um plugue azul).
3. Selecione PostgreSQL e clique em Next.
4. Preencha as configurações conforme abaixo:
    - Host: localhost
    - Port: 5432
    - Database: rotinik_db
    - Username: postgres
    - Password: postgres

5. Clique em "Test Connection".
6. Se o DBeaver pedir para baixar os drivers do PostgreSQL, clique em Download.
7. Se o teste passar, clique em Finish.

### Repositório

1. Abra `src/appsettings.Development.json`.
2. Ponha o conteúdo abaixo no arquivo.

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "ConnectionStrings": {
        "RotinikConnection": "Host=localhost;Port=5432;Database=rotinik_db;Username=postgres;Password=postgres"
    },
    "JwtSettings": {
        "Secret": "RotinikSuperSecretKeyQueDeveTerPeloMenos32Caracteres!", 
        "Issuer": "RotinikApi",
        "Audience": "RotinikApp",
        "ExpiresInHours": 8
    }
}
```

rode:
```sh
dotnet ef migrations add new_models --project src/Rotinik.csproj
dotnet ef database update --project src/Rotinik.csproj
```

> Saia e entre no usuário, Log Out -> Log in, para o Docker funcionar (ou reinicie o computador).
