#!/bin/bash
cd /app/src/RO.DevTest.Persistence

sleep 10 
dotnet ef database update

cd /app/src/RO.DevTest.WebApi
dotnet run