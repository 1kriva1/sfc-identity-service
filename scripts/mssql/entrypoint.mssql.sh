#!/bin/bash

# Start SQL Server in the background
/opt/mssql/bin/sqlservr &

# Wait until SQL Server is ready
echo "Waiting for SQL Server to be available..."
until /opt/mssql-tools18/bin/sqlcmd -S 127.0.0.1,1433 -U sa -P "$MSSQL_SA_PASSWORD" -C -Q "SELECT 1" > /dev/null 2>&1; do
  sleep 1
  echo "Still Waiting for SQL Server to be available..."
done

# Run init script
echo "Running init.sql..."
/opt/mssql-tools18/bin/sqlcmd -S 127.0.0.1,1433 -U sa -P "$MSSQL_SA_PASSWORD" -C -i /usr/src/app/init.sql
echo "Finished running init.sql."

# Keep container alive
wait
