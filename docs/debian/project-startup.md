# Guia de Inicialização do Projeto

## Ligar os Serviços do Sistema

```sh
sudo systemctl start postgresql
sudo systemctl start docker
```

## Usar .NET

Rodar API:
```sh
dotnet run --project src/Rotinik-Backend.csproj
```

Rodar Testes:
```sh
dotnet test
```

Atualizar BD:
```sh
dotnet ef database update --project src/Rotinik-Backend.csproj
```
