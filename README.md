# Rotinik Backend API

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?logo=postgresql&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&logoColor=white)

## Prerequisites

Before running the project locally, ensure you have the following installed:

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [DBeaver](https://dbeaver.io/) or any other database client (optional, for visualizing data)

## Local Setup Tutorial (From Zero to Testing)

Follow these steps to get the server running on your local machine:

### 1. Database Configuration

The application requires a PostgreSQL database to store users, routines, and tasks. You need to configure the connection string.

1. Navigate to the `RotinikApi` directory:
   ```bash
   cd RotinikApi
   ```
2. Create a file named `appsettings.Development.json` (if it doesn't exist) and add your PostgreSQL credentials and JWT secret key:

   ```json
   {
       "Logging": {
           "LogLevel": {
               "Default": "Information",
               "Microsoft.AspNetCore": "Warning"
           }
       },
       "ConnectionStrings": {
           "RotinikConnection": "Host=localhost;Port=5432;Database=rotinik_db;Username=postgres;Password=YOUR_POSTGRES_PASSWORD"
       },
       "JwtSettings": {
           "Key": "RotinikSuperSecretKeyThatNeedsToBeAtLeast32CharactersLong!",
           "Issuer": "RotinikApi",
           "Audience": "RotinikApp",
           "ExpiresInHours": 8
       }
   }
   ```
   *(Make sure your local PostgreSQL service is running and replace `YOUR_POSTGRES_PASSWORD` with your actual password).*

### 2. Install Entity Framework Core CLI

If you haven't installed the `dotnet-ef` global tool, install it by running:
```bash
dotnet tool install --global dotnet-ef
```

### 3. Apply Database Migrations

Apply the existing migrations to create the database schema automatically:
```bash
dotnet ef database update
```
*Note: This command will create the `rotinik_db` database and all necessary tables.*

### 4. Run the Server

Start the application:
```bash
dotnet run
```
The API will be available at `http://localhost:5025`.

### 5. API Documentation (Scalar)

You can explore and test the API endpoints interactively using Scalar, which is built-in.
While the server is running, open your browser and navigate to:
👉 `http://localhost:5025/scalar/v1`

## Running Automated Tests

We have provided a bash script to test the main User/Auth endpoints automatically (including JWT authentication).

1. Ensure the API is running in one terminal window (`dotnet run`).
2. Open a new terminal window in the `RotinikApi` folder.
3. Make the script executable and run it:
   ```bash
   chmod +x scripts/test_api.sh
   bash scripts/test_api.sh
   ```
This script will sequentially test creating a user, blocking unauthorized access, logging in, retrieving authenticated user data (`/me`), and finally deleting the test user.
