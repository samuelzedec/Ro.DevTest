#!/bin/bash

# Entrar no projeto Persistence
cd /app/src/RO.DevTest.Persistence

# Esperar o banco subir
sleep 2

# Aplicar migrations
dotnet ef database update

# Entrar na WebApi
cd /app/src/RO.DevTest.WebApi

# Rodar a aplicação
dotnet run